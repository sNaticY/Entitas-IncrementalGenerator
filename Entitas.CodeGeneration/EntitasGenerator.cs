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
    /// <summary>
    /// Default Unity main assembly name used when <see cref="AssembliesConfigKey"/> is not set.
    /// </summary>
    const string DefaultMainAssembly = "Assembly-CSharp";

    /// <summary>
    /// Internal test assembly; always allowed to run regardless of configuration.
    /// </summary>
    const string TestsAssembly = "Entitas.CodeGeneration-Tests";

    /// <summary>
    /// MSBuild / .editorconfig key used to configure the list of assembly names for which
    /// Entitas code generation should run.
    ///
    /// Set it in your .editorconfig (global section) or in a Directory.Build.props file:
    ///   is_global = true
    ///   build_property.EntitasAssemblies = Assembly-CSharp, MyGameplay, MyUI
    ///
    /// When not set, defaults to "Assembly-CSharp".
    /// Multiple assembly names must be separated by commas or semicolons.
    /// </summary>
    public const string AssembliesConfigKey = "build_property.EntitasAssemblies";
    
    const bool VisualDebuggingGenerationEnabled = true; // TODO: Add config (in .editorconfig?)
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var configuredAssemblies = GetConfiguredAssemblies(context.AnalyzerConfigOptionsProvider);

        var shouldRun = context.CompilationProvider
            .Combine(configuredAssemblies)
            .Select(static (pair, _) =>
            {
                var assemblyName = pair.Left.AssemblyName;
                // Always allow the internal test assembly
                if (assemblyName == TestsAssembly)
                    return true;
                return pair.Right.Contains(assemblyName ?? string.Empty);
            });
    
        var contextsData = ContextGenerationHelper.GetContextsData(context);
        RegisterContextsGeneration(context, shouldRun, contextsData);
        
        var componentsData = ComponentGenerationHelper.GetComponentsData(context);
        var componentsByContextNameLookup = ComponentsLookupGenerationHelper.GetComponentsByContextNameLookup(componentsData);
        RegisterIndividualComponentsGeneration(context, shouldRun, contextsData, componentsByContextNameLookup);
        RegisterSharedSourcesGeneration(context, shouldRun, contextsData, componentsData, componentsByContextNameLookup);

        if (VisualDebuggingGenerationEnabled)
            RegisterVisualDebuggingGeneration(context, contextsData, configuredAssemblies);
    }

    static IncrementalValueProvider<ImmutableHashSet<string>> GetConfiguredAssemblies(
        IncrementalValueProvider<AnalyzerConfigOptionsProvider> optionsProvider)
    {
        return optionsProvider.Select(static (options, _) =>
        {
            if (options.GlobalOptions.TryGetValue(AssembliesConfigKey, out var value)
                && !string.IsNullOrWhiteSpace(value))
            {
                var assemblies = value
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(static a => a.Trim())
                    .Where(static a => !string.IsNullOrEmpty(a))
                    .ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);

                if (assemblies.Count > 0)
                    return assemblies;
            }

            return ImmutableHashSet.Create(StringComparer.OrdinalIgnoreCase, DefaultMainAssembly);
        });
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
        in IncrementalValueProvider<ImmutableArray<ContextData>> contextsData,
        IncrementalValueProvider<ImmutableHashSet<string>> configuredAssemblies)
    {
        // skip for tests, to avoid bloating snapshots
        var shouldRun = context.CompilationProvider
            .Combine(configuredAssemblies)
            .Select(static (pair, _) =>
            {
                var assemblyName = pair.Left.AssemblyName;
                return pair.Right.Contains(assemblyName ?? string.Empty);
            });

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
}