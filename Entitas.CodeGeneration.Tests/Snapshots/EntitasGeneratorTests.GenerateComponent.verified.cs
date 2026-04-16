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
    public const int TestFlag = 0;
    public const int TestMember = 1;

    public const int TotalComponents = 2;

    public static readonly string[] componentNames = 
    {
        "TestFlag",
        "TestMember"
    };

    public static readonly System.Type[] componentTypes = 
    {
        typeof(TestFlagComponent),
        typeof(TestMemberComponent)
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


// GameTestFlagComponent.g.cs
public static class GameTestFlagEntityExtensions
{
    static readonly TestFlagComponent testFlagComponent = new TestFlagComponent();

    public static bool IsTestFlag(this GameEntity entity)
    {
        return entity.HasComponent(GameComponentsLookup.TestFlag);
    }

    public static void SetTestFlag(this GameEntity entity, bool value)
    {
        if (value != entity.IsTestFlag())
        {
            var index = GameComponentsLookup.TestFlag;
            if (value)
            {
                var componentPool = entity.GetComponentPool(index);
                var component = componentPool.Count > 0
                        ? componentPool.Pop()
                        : testFlagComponent;

                entity.AddComponent(index, component);
            }
            else
            {
                entity.RemoveComponent(index);
            }
        }
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherTestFlag;

    public static Entitas.IMatcher<GameEntity> TestFlag()
    {
        if (_matcherTestFlag == null)
        {
            var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.TestFlag);
            matcher.componentNames = GameComponentsLookup.componentNames;
            _matcherTestFlag = matcher;
        }

        return _matcherTestFlag;
    }
}


// GameTestMemberComponent.g.cs
public static class GameTestMemberEntityExtensions
{
    public static TestMemberComponent GetTestMember(this GameEntity entity) { return (TestMemberComponent)entity.GetComponent(GameComponentsLookup.TestMember); }
    public static bool HasTestMember(this GameEntity entity) { return entity.HasComponent(GameComponentsLookup.TestMember); }

    public static void AddTestMember(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.TestMember;
        var component = (TestMemberComponent)entity.CreateComponent(index, typeof(TestMemberComponent));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.TestMember;
        var component = (TestMemberComponent)entity.CreateComponent(index, typeof(TestMemberComponent));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember(this GameEntity entity)
    {
        entity.RemoveComponent(GameComponentsLookup.TestMember);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherTestMember;

    public static Entitas.IMatcher<GameEntity> TestMember()
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
