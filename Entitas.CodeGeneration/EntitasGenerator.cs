using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Entitas.CodeGeneration.Cleanup;
using Entitas.CodeGeneration.ComponentRegistry;
using Entitas.CodeGeneration.Components;
using Entitas.CodeGeneration.Components.Data;
using Entitas.CodeGeneration.ComponentsLookups;
using Entitas.CodeGeneration.Contexts;
using Entitas.CodeGeneration.Contexts.Data;
using Entitas.CodeGeneration.EntityIndex;
using Entitas.CodeGeneration.Events;
using Entitas.CodeGeneration.VisualDebugging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Entitas.CodeGeneration;

[Generator]
public class EntitasGenerator : IIncrementalGenerator
{
    // Default assembly when no configuration is provided
    const string DefaultMainAssembly = "Assembly-CSharp";
    const string TestsAssembly = "Entitas.CodeGeneration-Tests";

    // .editorconfig / MSBuild property key to configure target assemblies
    // Usage in .editorconfig:
    //   [*.cs]
    //   build_property.EntitasTargetAssemblies = Assembly-CSharp,MyGame.Gameplay,MyGame.Audio
    // Or in .csproj:
    //   <EntitasTargetAssemblies>Assembly-CSharp;MyGame.Gameplay</EntitasTargetAssemblies>
    internal const string TargetAssembliesPropertyKey = "build_property.EntitasTargetAssemblies";

    const bool VisualDebuggingGenerationEnabled = true; // TODO: Add config (in .editorconfig?)
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var configuredAssemblies = context.AnalyzerConfigOptionsProvider
            .Select(static (provider, _) => ParseTargetAssemblies(provider.GlobalOptions));

        var shouldRun = context.CompilationProvider
            .Combine(configuredAssemblies)
            .Select(static (pair, _) => ShouldRunForAssembly(pair.Left.AssemblyName, pair.Right));
    
        // Contexts defined in THIS compilation (primary mode)
        var localContextsData = ContextGenerationHelper.GetContextsData(context);
        RegisterContextsGeneration(context, shouldRun, localContextsData);
        
        var componentsData = ComponentGenerationHelper.GetComponentsData(context);
        var componentsByContextNameLookup = ComponentsLookupGenerationHelper.GetComponentsByContextNameLookup(componentsData);
        RegisterIndividualComponentsGeneration(context, shouldRun, localContextsData, componentsByContextNameLookup);
        RegisterSharedSourcesGeneration(context, shouldRun, localContextsData, componentsData, componentsByContextNameLookup);

        // Contexts defined in REFERENCED assemblies (secondary / cross-assembly mode)
        var referencedContextsData = ContextGenerationHelper.GetReferencedContextsData(context);
        RegisterSecondaryComponentsGeneration(context, shouldRun, localContextsData, referencedContextsData, componentsData);

        if (VisualDebuggingGenerationEnabled)
            RegisterVisualDebuggingGeneration(context, localContextsData);
    }

    void RegisterContextsGeneration(
        IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<bool> shouldRun,
        in IncrementalValueProvider<ImmutableArray<ContextData>> contextsData)
    {
        var combinedInput = shouldRun.Combine(contextsData);
        
        // This will be triggered for any context change (ContextAttribute class)
        context.RegisterSourceOutput(combinedInput,
            static (spc, source) => GenerateContexts(source, spc));
    }

    static void GenerateContexts(
        (bool, ImmutableArray<ContextData>) input,
        SourceProductionContext spc)
    {
        if (!input.Item1) // check shouldRun
            return;

        var contextsData = input.Item2;

        // Skip if no local context definitions exist (secondary-assembly case).
        // Generating Contexts.g.cs in a secondary assembly would produce an empty
        // stub that conflicts with the primary assembly's Contexts class.
        if (contextsData.IsDefaultOrEmpty)
            return;

        ContextGenerationHelper.GenerateContexts(spc, contextsData);

        // Generate a ComponentRegistry for every local context so that secondary
        // assemblies can register additional components into it at runtime.
        foreach (var contextData in contextsData)
        {
            var lookupName = contextData.ContextName + ComponentGenerationHelper.ComponentsLookupName;
            ComponentRegistryGenerationHelper.GenerateContextComponentRegistry(spc, contextData, lookupName);
        }
    }

    void RegisterSharedSourcesGeneration(
        IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<bool> shouldRun,
        in IncrementalValueProvider<ImmutableArray<ContextData>> contextsData,
        in IncrementalValueProvider<ImmutableArray<ComponentData>> componentsData,
        in IncrementalValueProvider<ImmutableDictionary<string, ImmutableArray<ComponentData>>> componentsByContextNameLookup)
    {
        var contextsAndComponents = contextsData.Combine(componentsData);
        var contextsAndComponentsWithLookup = contextsAndComponents.Combine(componentsByContextNameLookup);
        var combinedInput = shouldRun.Combine(contextsAndComponentsWithLookup);

        // Warning: This will be triggered for ANY context or component change
        // Keep it as light as possible
        context.RegisterSourceOutput(combinedInput,
            static (spc, source) => GenerateSharedSources(source, spc));
    }

    static void GenerateSharedSources(
        (bool, ((ImmutableArray<ContextData>, ImmutableArray<ComponentData>), ImmutableDictionary<string, ImmutableArray<ComponentData>>)) input, 
        SourceProductionContext spc)
    {
        if (!input.Item1) // check shouldRun
            return;
        
        var contextsData = input.Item2.Item1.Item1;
        var contextLookup = contextsData.ToDictionary(ctx => ctx.ContextName);

        var componentsData = input.Item2.Item1.Item2;
        ComponentGenerationHelper.TryGenerateEntityComponentInterfaces(spc, componentsData, contextLookup);

        var componentsByContextNameLookup = input.Item2.Item2;
        ComponentsLookupGenerationHelper.GenerateComponentsLookups(spc, componentsByContextNameLookup, contextLookup);
        
        EntityIndexGenerationHelper.GenerateEntityIndices(spc, componentsByContextNameLookup, contextLookup);        
        CleanupGenerationHelper.GenerateCleanupSystems(spc, componentsByContextNameLookup, contextLookup);        
        EventsGenerationHelper.GenerateEventSystems(spc, componentsByContextNameLookup, contextLookup);
    }

    void RegisterIndividualComponentsGeneration(
        IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<bool> shouldRun,
        in IncrementalValueProvider<ImmutableArray<ContextData>> contextsData,
        in IncrementalValueProvider<ImmutableDictionary<string, ImmutableArray<ComponentData>>> componentsByContextNameLookup)
    {
        // Extract all (ContextData, ComponentData) pairs
        var componentByContext = contextsData
            .Combine(componentsByContextNameLookup)
            .SelectMany((pair, _) =>
            {
                var (contexts, componentLookup) = pair;
                return contexts.SelectMany((contextData, _) =>
                {
                    return !componentLookup.TryGetValue(contextData.ContextName, out var matchingComponents) 
                        ? Enumerable.Empty<(ContextData, ComponentData)>() 
                        : matchingComponents.Select(component => (contextData, component));
                });
            });
        
        var conditionalComponentByContext = componentByContext.Combine(shouldRun)
            .Select((pair, _) => (ShouldRun: pair.Right, pair.Left));
        
        // This is triggered for:
        // - individual components that have changed, or
        // - all components belonging to a context that has changed.
        context.RegisterSourceOutput(conditionalComponentByContext, 
            static (spc, source) => GenerateIndividualEntityComponent(source, spc));
    } 
    
    static void GenerateIndividualEntityComponent(
        (bool, (ContextData, ComponentData)) input,
        SourceProductionContext spc)
    {
        if (!input.Item1) // check shouldRun
            return;

        var componentData = input.Item2.Item2;
        var contextData = input.Item2.Item1;

        ComponentGenerationHelper.GenerateEntityComponent(spc, componentData, contextData);
        
        if (componentData.IsGenerated)
            return;
        
        EventsGenerationHelper.GenerateComponentEvents(spc, componentData, contextData);
        
        if (componentData.HasCleanupAttribute)
            CleanupGenerationHelper.GenerateComponentCleanupSystem(spc, componentData, contextData);
    }

    // -------------------------------------------------------------------------
    // Secondary-assembly (cross-assembly context) generation
    // -------------------------------------------------------------------------

    void RegisterSecondaryComponentsGeneration(
        IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<bool> shouldRun,
        in IncrementalValueProvider<ImmutableArray<ContextData>> localContextsData,
        in IncrementalValueProvider<ImmutableArray<ContextData>> referencedContextsData,
        in IncrementalValueProvider<ImmutableArray<ComponentData>> componentsData)
    {
        // We only want to generate secondary code when the current assembly has NO local context
        // definitions for a given context name but DOES have components targeting that context.
        // Combine everything needed and dispatch per (referencedContext, component) pair.
        var combined = shouldRun
            .Combine(localContextsData)
            .Combine(referencedContextsData)
            .Combine(componentsData);

        context.RegisterSourceOutput(combined,
            static (spc, source) => GenerateSecondaryComponents(source, spc));
    }

    static void GenerateSecondaryComponents(
        (((bool, ImmutableArray<ContextData>), ImmutableArray<ContextData>), ImmutableArray<ComponentData>) input,
        SourceProductionContext spc)
    {
        var shouldRun      = input.Item1.Item1.Item1;
        var localContexts  = input.Item1.Item1.Item2;
        var refContexts    = input.Item1.Item2;
        var components     = input.Item2;

        if (!shouldRun) return;
        if (refContexts.IsDefaultOrEmpty) return;

        // Build a fast set of locally-defined context names so we can skip them
        var localContextNames = new HashSet<string>(localContexts.Select(c => c.ContextName));

        // Only consider contexts that are referenced (not local)
        var secondaryContexts = refContexts
            .Where(c => !localContextNames.Contains(c.ContextName))
            .ToArray();

        if (secondaryContexts.Length == 0) return;
        
        var secondaryContextLookup = secondaryContexts.ToDictionary(c => c.ContextName);

        // Group components by context for the secondary-lookup generation
        var componentsByContext = new Dictionary<string, List<ComponentData>>();
        foreach (var component in components)
        {
            foreach (var contextName in component.ContextNames)
            {
                if (!secondaryContextLookup.ContainsKey(contextName)) continue;
                if (!componentsByContext.TryGetValue(contextName, out var list))
                    componentsByContext[contextName] = list = new List<ComponentData>();
                list.Add(component);
            }
        }

        if (componentsByContext.Count == 0) return;

        // Generate the secondary lookup + module initializer per context
        foreach (var kvp in componentsByContext)
        {
            var contextData = secondaryContextLookup[kvp.Key];
            var contextComponents = kvp.Value.ToImmutableArray();
            ComponentRegistryGenerationHelper.GenerateSecondaryComponentsLookup(
                spc, contextData, contextComponents);

            // Generate per-component extension methods
            foreach (var component in contextComponents)
                ComponentGenerationHelper.GenerateEntityComponentExtension(spc, component, contextData);
        }
    }


    void RegisterVisualDebuggingGeneration(
        IncrementalGeneratorInitializationContext context,
        in IncrementalValueProvider<ImmutableArray<ContextData>> contextsData)
    {
        var configuredAssemblies = context.AnalyzerConfigOptionsProvider
            .Select(static (provider, _) => ParseTargetAssemblies(provider.GlobalOptions));

        // skip for tests, to avoid bloating snapshots
        var shouldRun = context.CompilationProvider
            .Combine(configuredAssemblies)
            .Select(static (pair, _) => ShouldRunForAssembly(pair.Left.AssemblyName, pair.Right)
                && pair.Left.AssemblyName != TestsAssembly);

        var combinedInput = shouldRun.Combine(contextsData);
        context.RegisterSourceOutput(combinedInput, 
            static (spc, source) => GenerateVisualDebugging(source, spc));
    }

    static void GenerateVisualDebugging(
        (bool, ImmutableArray<ContextData>) input,
        SourceProductionContext spc)
    {
        if (!input.Item1) // check shouldRun
            return;

        var contextsData = input.Item2;

        // Skip visual debugging generation for secondary assemblies (no local contexts).
        // Feature.g.cs / ContextObservers.g.cs would conflict with the primary assembly.
        if (contextsData.IsDefaultOrEmpty)
            return;

        // Context Observers, Feature
        VisualDebuggingGenerationHelper.Generate(spc, contextsData);
    }

    /// <summary>
    /// Parses the target assemblies from the analyzer config options.
    /// Reads the <c>build_property.EntitasTargetAssemblies</c> MSBuild property,
    /// which can be set via a <c>&lt;EntitasTargetAssemblies&gt;</c> property in the .csproj file
    /// or via <c>build_property.EntitasTargetAssemblies</c> in an .editorconfig.
    /// </summary>
    internal static ImmutableArray<string> ParseTargetAssemblies(AnalyzerConfigOptions options)
    {
        if (options.TryGetValue(TargetAssembliesPropertyKey, out var assembliesValue)
            && !string.IsNullOrWhiteSpace(assembliesValue))
        {
            var assemblies = assembliesValue.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            var builder = ImmutableArray.CreateBuilder<string>(assemblies.Length);
            foreach (var a in assemblies)
            {
                var trimmed = a.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                    builder.Add(trimmed);
            }
            return builder.ToImmutable();
        }
        return ImmutableArray<string>.Empty;
    }

    /// <summary>
    /// Determines whether the generator should run for the given assembly.
    /// If no assemblies are configured, defaults to <see cref="DefaultMainAssembly"/> ("Assembly-CSharp").
    /// The test assembly (<see cref="TestsAssembly"/>) is always included.
    /// </summary>
    internal static bool ShouldRunForAssembly(string? assemblyName, ImmutableArray<string> configuredAssemblies)
    {
        if (assemblyName == null) return false;
        
        // Test assembly is always included regardless of configuration
        if (assemblyName == TestsAssembly) return true;

        // If assemblies are explicitly configured, check against the list
        if (!configuredAssemblies.IsDefaultOrEmpty)
            return configuredAssemblies.Contains(assemblyName);

        // Default behavior: only run for the main Unity assembly
        return assemblyName == DefaultMainAssembly;
    }
}