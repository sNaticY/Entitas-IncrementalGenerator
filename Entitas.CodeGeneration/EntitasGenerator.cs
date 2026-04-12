using System.Collections.Immutable;
using Entitas.CodeGeneration.Cleanup;
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
    
        var contextsData = ContextGenerationHelper.GetContextsData(context);
        RegisterContextsGeneration(context, shouldRun, contextsData);
        
        var componentsData = ComponentGenerationHelper.GetComponentsData(context);
        var componentsByContextNameLookup = ComponentsLookupGenerationHelper.GetComponentsByContextNameLookup(componentsData);
        RegisterIndividualComponentsGeneration(context, shouldRun, contextsData, componentsByContextNameLookup);
        RegisterSharedSourcesGeneration(context, shouldRun, contextsData, componentsData, componentsByContextNameLookup);

        if (VisualDebuggingGenerationEnabled)
            RegisterVisualDebuggingGeneration(context, contextsData);
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
        ContextGenerationHelper.GenerateContexts(spc, contextsData);
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