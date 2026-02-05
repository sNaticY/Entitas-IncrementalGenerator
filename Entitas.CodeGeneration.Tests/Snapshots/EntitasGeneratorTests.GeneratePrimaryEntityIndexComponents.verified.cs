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
    public const int TestMember2 = 1;

    public const int TotalComponents = 2;

    public static readonly string[] componentNames = 
    {
        "TestMember",
        "TestMember2"
    };

    public static readonly System.Type[] componentTypes = 
    {
        typeof(TestMemberComponent),
        typeof(TestMember2Component)
    };
}


// GameContext.g.cs
public sealed partial class GameContext : Entitas.Context<GameEntity>
{
    public GameContext()
        : base(
            GameComponentsLookup.TotalComponents,
            0,
            new Entitas.ContextInfo(
                "Game",
                GameComponentsLookup.componentNames,
                GameComponentsLookup.componentTypes
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


// GameEntity.g.cs
public sealed partial class GameEntity : Entitas.Entity
{
}


// GameEntityIndices.g.cs
public partial class Contexts
{
    public const string TestMember = "TestMember";
    public const string TestMember2 = "TestMember2";


    [Entitas.CodeGeneration.Attributes.PostConstructor]
    public void InitializeEntityIndices()
    {
        game.AddEntityIndex(new Entitas.PrimaryEntityIndex<GameEntity, string>(
            TestMember,
            game.GetGroup(GameMatcher.TestMember),
            (e, c) => ((TestMemberComponent)c).Value));

        game.AddEntityIndex(new Entitas.PrimaryEntityIndex<GameEntity, string>(
            TestMember2,
            game.GetGroup(GameMatcher.TestMember2),
            (e, c) => ((TestMember2Component)c).Value));
    }
}

public static class ContextsExtensions
{
    public static GameEntity GetEntityWithTestMember(this GameContext context, string Value) {
        return ((Entitas.PrimaryEntityIndex<GameEntity, string>)context.GetEntityIndex(Contexts.TestMember)).GetEntity(Value);
    }

    public static GameEntity GetEntityWithTestMember2(this GameContext context, string Value) {
        return ((Entitas.PrimaryEntityIndex<GameEntity, string>)context.GetEntityIndex(Contexts.TestMember2)).GetEntity(Value);
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


// GameTestMember2Component.g.cs
public partial class GameEntity
{
    public TestMember2Component testMember2 { get { return (TestMember2Component)GetComponent(GameComponentsLookup.TestMember2); } }
    public bool hasTestMember2 { get { return HasComponent(GameComponentsLookup.TestMember2); } }

    public void AddTestMember2(string newValue)
    {
        var index = GameComponentsLookup.TestMember2;
        var component = (TestMember2Component)CreateComponent(index, typeof(TestMember2Component));
        component.Value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceTestMember2(string newValue)
    {
        var index = GameComponentsLookup.TestMember2;
        var component = (TestMember2Component)CreateComponent(index, typeof(TestMember2Component));
        component.Value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveTestMember2()
    {
        RemoveComponent(GameComponentsLookup.TestMember2);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherTestMember2;

    public static Entitas.IMatcher<GameEntity> TestMember2
    {
        get
        {
            if (_matcherTestMember2 == null)
            {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.TestMember2);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherTestMember2 = matcher;
            }

            return _matcherTestMember2;
        }
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
