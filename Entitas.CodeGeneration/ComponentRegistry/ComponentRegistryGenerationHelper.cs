using System.Collections.Immutable;
using System.Text;
using Entitas.CodeGeneration.Components;
using Entitas.CodeGeneration.Components.Data;
using Entitas.CodeGeneration.Contexts.Data;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Entitas.CodeGeneration.ComponentRegistry;

public static class ComponentRegistryGenerationHelper
{
    /// <summary>
    /// Generates the ${ContextName}ContextComponentRegistry class in the primary assembly.
    /// </summary>
    public static void GenerateContextComponentRegistry(
        SourceProductionContext spc,
        in ContextData contextData,
        string componentsLookupName)
    {
        var source = ComponentRegistryTemplates.ContextComponentRegistryTemplate
            .Replace("${ContextName}", contextData.ContextName)
            .Replace("${Lookup}", componentsLookupName);

        spc.AddSource(contextData.ContextName + "ContextComponentRegistry.g.cs",
            SourceText.From(source, Encoding.UTF8));
    }

    /// <summary>
    /// Generates the secondary-assembly lookup + ModuleInitializer registration for all
    /// components in a secondary assembly that belong to a referenced context.
    /// </summary>
    public static void GenerateSecondaryComponentsLookup(
        SourceProductionContext spc,
        in ContextData contextData,
        in ImmutableArray<ComponentData> components)
    {
        if (components.IsDefaultOrEmpty)
            return;

        var secondaryLookupName = contextData.ContextName + "ExtComponentsLookup";
        var registryName = contextData.ContextName + "ContextComponentRegistry";

        var componentFieldsList = string.Join("\n", components
            .Select(c => ComponentRegistryTemplates.SecondaryComponentFieldTemplate
                .Replace("${ComponentName}", c.GetComponentName())));

        var componentNamesList = string.Join(", ", components
            .Select(c => ComponentRegistryTemplates.SecondaryComponentNameTemplate
                .Replace("${ComponentName}", c.GetComponentName())));

        var componentTypesList = string.Join(", ", components
            .Select(c => ComponentRegistryTemplates.SecondaryComponentTypeTemplate
                .Replace("${ComponentType}", c.FullTypeName)));

        var componentIndexAssignmentsList = string.Join("\n", components
            .Select((c, index) => ComponentRegistryTemplates.SecondaryComponentIndexAssignmentTemplate
                .Replace("${ComponentName}", c.GetComponentName())
                .Replace("${RelativeIndex}", index.ToString())));

        var source = ComponentRegistryTemplates.SecondaryComponentsLookupTemplate
            .Replace("${SecondaryLookupName}", secondaryLookupName)
            .Replace("${RegistryName}", registryName)
            .Replace("${count}", components.Length.ToString())
            .Replace("${componentFieldsList}", componentFieldsList)
            .Replace("${componentNamesList}", componentNamesList)
            .Replace("${componentTypesList}", componentTypesList)
            .Replace("${componentIndexAssignmentsList}", componentIndexAssignmentsList);

        spc.AddSource(secondaryLookupName + ".g.cs", SourceText.From(source, Encoding.UTF8));
    }
}
