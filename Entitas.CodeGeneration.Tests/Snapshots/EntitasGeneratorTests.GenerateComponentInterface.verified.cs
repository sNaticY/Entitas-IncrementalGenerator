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


// InputCleanupSystems.g.cs
public sealed class InputCleanupSystems : Feature
{
    public InputCleanupSystems(Contexts contexts)
    {

    }
}


// InputComponentsLookup.g.cs
public static class InputComponentsLookup
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


// InputContext.g.cs
public sealed partial class InputContext : Entitas.Context<InputEntity>
{
    public InputContext()
        : base(
            InputComponentsLookup.TotalComponents,
            0,
            new Entitas.ContextInfo(
                "Input",
                InputComponentsLookup.componentNames,
                InputComponentsLookup.componentTypes
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


// InputEntity.g.cs
public sealed partial class InputEntity : Entitas.Entity
{
}


// InputEventSystems.g.cs
public sealed class InputEventSystems : Feature
{
    public InputEventSystems(Contexts contexts)
    {

    }
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


// InputTestFlagComponent.g.cs
public static class InputTestFlagEntityExtensions
{
    static readonly TestFlagComponent testFlagComponent = new TestFlagComponent();

    public static bool IsTestFlag(this InputEntity entity)
    {
        return entity.HasComponent(InputComponentsLookup.TestFlag);
    }

    public static void SetTestFlag(this InputEntity entity, bool value)
    {
        if (value != entity.IsTestFlag())
        {
            var index = InputComponentsLookup.TestFlag;
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

public sealed partial class InputMatcher
{
    static Entitas.IMatcher<InputEntity> _matcherTestFlag;

    public static Entitas.IMatcher<InputEntity> TestFlag()
    {
        if (_matcherTestFlag == null)
        {
            var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.TestFlag);
            matcher.componentNames = InputComponentsLookup.componentNames;
            _matcherTestFlag = matcher;
        }

        return _matcherTestFlag;
    }
}


// InputTestMemberComponent.g.cs
public static class InputTestMemberEntityExtensions
{
    public static TestMemberComponent GetTestMember(this InputEntity entity) { return (TestMemberComponent)entity.GetComponent(InputComponentsLookup.TestMember); }
    public static bool HasTestMember(this InputEntity entity) { return entity.HasComponent(InputComponentsLookup.TestMember); }

    public static void AddTestMember(this InputEntity entity, string newValue)
    {
        var index = InputComponentsLookup.TestMember;
        var component = (TestMemberComponent)entity.CreateComponent(index, typeof(TestMemberComponent));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember(this InputEntity entity, string newValue)
    {
        var index = InputComponentsLookup.TestMember;
        var component = (TestMemberComponent)entity.CreateComponent(index, typeof(TestMemberComponent));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember(this InputEntity entity)
    {
        entity.RemoveComponent(InputComponentsLookup.TestMember);
    }
}

public sealed partial class InputMatcher
{
    static Entitas.IMatcher<InputEntity> _matcherTestMember;

    public static Entitas.IMatcher<InputEntity> TestMember()
    {
        if (_matcherTestMember == null)
        {
            var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.TestMember);
            matcher.componentNames = InputComponentsLookup.componentNames;
            _matcherTestMember = matcher;
        }

        return _matcherTestMember;
    }
}
