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

    public Entitas.IContext[] allContexts { get { return new Entitas.IContext [] { game }; } }

    public Contexts()
    {
        game = new GameContext();

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


// GameCleanupSystems.g.cs
public sealed class GameCleanupSystems : Feature
{
    public GameCleanupSystems(Contexts contexts)
    {

    }
}


// GameComponentsLookup.g.cs
public static class GameComponentsLookup
{
    public const int TestMember = 0;

    public const int TotalComponents = 1;

    public static readonly string[] componentNames = 
    {
        "TestMember"
    };

    public static readonly System.Type[] componentTypes = 
    {
        typeof(TestMemberComponent)
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


// GameEntityIndices.g.cs
public partial class Contexts
{
    public const string TestMember = "TestMember";

    [Entitas.CodeGeneration.Attributes.PostConstructor]
    public void InitializeEntityIndices()
    {
        game.AddEntityIndex(new Entitas.EntityIndex<GameEntity, string>(
            TestMember,
            game.GetGroup(GameMatcher.TestMember),
            (e, c) => ((TestMemberComponent)c).Value));
    }
}

public static class ContextsExtensions
{
    public static System.Collections.Generic.HashSet<GameEntity> GetEntitiesWithTestMember(this GameContext context, string Value) {
        return ((Entitas.EntityIndex<GameEntity, string>)context.GetEntityIndex(Contexts.TestMember)).GetEntities(Value);
    }
}

// GameEventSystems.g.cs
public sealed class GameEventSystems : Feature
{
    public GameEventSystems(Contexts contexts)
    {

    }
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


// GameTestMemberComponent.g.cs
public partial class GameEntity
{
    public TestMemberComponent testMember { get { return (TestMemberComponent)GetComponent(GameComponentsLookup.TestMember); } }
    public bool hasTestMember { get { return HasComponent(GameComponentsLookup.TestMember); } }

    public void AddTestMember(string newValue)
    {
        var index = GameComponentsLookup.TestMember;
        var component = (TestMemberComponent)CreateComponent(index, typeof(TestMemberComponent));
        component.Value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceTestMember(string newValue)
    {
        var index = GameComponentsLookup.TestMember;
        var component = (TestMemberComponent)CreateComponent(index, typeof(TestMemberComponent));
        component.Value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveTestMember()
    {
        RemoveComponent(GameComponentsLookup.TestMember);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherTestMember;

    public static Entitas.IMatcher<GameEntity> TestMember
    {
        get
        {
            if (_matcherTestMember == null)
            {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.TestMember);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherTestMember = matcher;
            }

            return _matcherTestMember;
        }
    }
}
