// Contexts.g.cs
public partial class Contexts : Entitas.IContexts
{
    public static Contexts sharedInstance
    {
        get
        {
            if (_sharedInstance == null)
            {
                _sharedInstance = new Contexts();
            }

            return _sharedInstance;
        }
        set { _sharedInstance = value; }
    }

    static Contexts _sharedInstance;

    public GameContext game { get; set; }
    public InputContext input { get; set; }

    public Entitas.IContext[] allContexts { get { return new Entitas.IContext [] { game, input }; } }

    public Contexts()
    {
        game = new GameContext();
        input = new InputContext();

        var postConstructors = System.Linq.Enumerable.Where(
            GetType().GetMethods(),
            method => System.Attribute.IsDefined(method, typeof(Entitas.CodeGeneration.Attributes.PostConstructorAttribute))
        );

        foreach (var postConstructor in postConstructors)
        {
            postConstructor.Invoke(this, null);
        }
    }

    public void Reset()
    {
        var contexts = allContexts;
        for (int i = 0; i < contexts.Length; i++)
        {
            contexts[i].Reset();
        }
    }
}


// GameComponentsLookup.g.cs
public static class GameComponentsLookup
{


    public const int TotalComponents = 0;

    public static readonly string[] componentNames = 
    {

    };

    public static readonly System.Type[] componentTypes = 
    {

    };
}


// GameContext.g.cs
public sealed partial class GameContext : Entitas.Context<GameEntity>
{
    public GameContext()
        : base(
            GameContextComponentRegistry.TotalComponents,
            0,
            new Entitas.ContextInfo(
                "Game",
                GameContextComponentRegistry.ComponentNames,
                GameContextComponentRegistry.ComponentTypes
            ),
            (entity) =>

#if (ENTITAS_FAST_AND_UNSAFE)
                new Entitas.UnsafeAERC(),
#else
                new Entitas.SafeAERC(entity),
#endif
            () => new GameEntity()
        ) 
    {
    }
}


// GameContextComponentRegistry.g.cs
public static class GameContextComponentRegistry
{
    static int _total = GameComponentsLookup.TotalComponents;
    static string[] _names = GameComponentsLookup.componentNames;
    static System.Type[] _types = GameComponentsLookup.componentTypes;

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


// GameEntity.g.cs
public sealed partial class GameEntity : Entitas.Entity
{
}


// GameMatcher.g.cs
public sealed partial class GameMatcher 
{
    public static Entitas.IAllOfMatcher<GameEntity> AllOf(params int[] indices) 
    {
        return Entitas.Matcher<GameEntity>.AllOf(indices);
    }

    public static Entitas.IAllOfMatcher<GameEntity> AllOf(params Entitas.IMatcher<GameEntity>[] matchers)
    {
        return Entitas.Matcher<GameEntity>.AllOf(matchers);
    }

    public static Entitas.IAnyOfMatcher<GameEntity> AnyOf(params int[] indices)
    {
        return Entitas.Matcher<GameEntity>.AnyOf(indices);
    }

    public static Entitas.IAnyOfMatcher<GameEntity> AnyOf(params Entitas.IMatcher<GameEntity>[] matchers)
    {
        return Entitas.Matcher<GameEntity>.AnyOf(matchers);
    }
}


// InputComponentsLookup.g.cs
public static class InputComponentsLookup
{


    public const int TotalComponents = 0;

    public static readonly string[] componentNames = 
    {

    };

    public static readonly System.Type[] componentTypes = 
    {

    };
}


// InputContext.g.cs
public sealed partial class InputContext : Entitas.Context<InputEntity>
{
    public InputContext()
        : base(
            InputContextComponentRegistry.TotalComponents,
            0,
            new Entitas.ContextInfo(
                "Input",
                InputContextComponentRegistry.ComponentNames,
                InputContextComponentRegistry.ComponentTypes
            ),
            (entity) =>

#if (ENTITAS_FAST_AND_UNSAFE)
                new Entitas.UnsafeAERC(),
#else
                new Entitas.SafeAERC(entity),
#endif
            () => new InputEntity()
        ) 
    {
    }
}


// InputContextComponentRegistry.g.cs
public static class InputContextComponentRegistry
{
    static int _total = InputComponentsLookup.TotalComponents;
    static string[] _names = InputComponentsLookup.componentNames;
    static System.Type[] _types = InputComponentsLookup.componentTypes;

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


// InputEntity.g.cs
public sealed partial class InputEntity : Entitas.Entity
{
}


// InputMatcher.g.cs
public sealed partial class InputMatcher 
{
    public static Entitas.IAllOfMatcher<InputEntity> AllOf(params int[] indices) 
    {
        return Entitas.Matcher<InputEntity>.AllOf(indices);
    }

    public static Entitas.IAllOfMatcher<InputEntity> AllOf(params Entitas.IMatcher<InputEntity>[] matchers)
    {
        return Entitas.Matcher<InputEntity>.AllOf(matchers);
    }

    public static Entitas.IAnyOfMatcher<InputEntity> AnyOf(params int[] indices)
    {
        return Entitas.Matcher<InputEntity>.AnyOf(indices);
    }

    public static Entitas.IAnyOfMatcher<InputEntity> AnyOf(params Entitas.IMatcher<InputEntity>[] matchers)
    {
        return Entitas.Matcher<InputEntity>.AnyOf(matchers);
    }
}
