namespace Entitas.CodeGeneration.ComponentRegistry;

public static class ComponentRegistryTemplates
{
    /// <summary>
    /// Generated in the PRIMARY assembly (where the ContextAttribute subclass is defined).
    /// Holds the total component count and arrays for names/types, and exposes a Register()
    /// method that secondary assemblies call (via ModuleInitializer) to append their components.
    /// </summary>
    public const string ContextComponentRegistryTemplate =
        @"public static class ${ContextName}ContextComponentRegistry
{
    static int _total = ${Lookup}.TotalComponents;
    static string[] _names = ${Lookup}.componentNames;
    static System.Type[] _types = ${Lookup}.componentTypes;

    /// <summary>
    /// Called by secondary assemblies to register additional components into this context.
    /// Must be called before the context is first instantiated (i.e., before Contexts.sharedInstance
    /// is accessed). Using [ModuleInitializer] in the secondary assembly guarantees this ordering.
    /// </summary>
    /// <returns>The base component index assigned to the calling assembly.</returns>
    public static int Register(int count, string[] names, System.Type[] types)
    {
        var baseIndex = _total;
        _total += count;

        var newNames = new string[_names.Length + names.Length];
        _names.CopyTo(newNames, 0);
        names.CopyTo(newNames, _names.Length);
        _names = newNames;

        var newTypes = new System.Type[_types.Length + types.Length];
        _types.CopyTo(newTypes, 0);
        types.CopyTo(newTypes, _types.Length);
        _types = newTypes;

        return baseIndex;
    }

    public static int TotalComponents => _total;
    public static string[] ComponentNames => _names;
    public static System.Type[] ComponentTypes => _types;
}
";

    /// <summary>
    /// Generated in SECONDARY assemblies (assemblies that define components belonging to a context
    /// whose ContextAttribute is in a referenced assembly).
    /// Holds the dynamically assigned indices for this assembly's components.
    /// </summary>
    public const string SecondaryComponentsLookupTemplate =
        @"internal static class ${SecondaryLookupName}
{
${componentFieldsList}
    internal static void Register()
    {
        var baseIndex = ${RegistryName}.Register(
            ${count},
            new string[] { ${componentNamesList} },
            new System.Type[] { ${componentTypesList} }
        );
${componentIndexAssignmentsList}
    }
}

internal static class ${SecondaryLookupName}ModuleInit
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    internal static void Init() => ${SecondaryLookupName}.Register();
}
";

    public const string SecondaryComponentFieldTemplate = @"    internal static int ${ComponentName};";
    public const string SecondaryComponentNameTemplate = @"""${ComponentName}""";
    public const string SecondaryComponentTypeTemplate = @"typeof(${ComponentType})";
    public const string SecondaryComponentIndexAssignmentTemplate =
        @"        ${ComponentName} = baseIndex + ${RelativeIndex};";
}
