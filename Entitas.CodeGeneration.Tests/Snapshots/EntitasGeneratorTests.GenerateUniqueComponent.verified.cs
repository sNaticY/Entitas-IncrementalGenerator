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
    public const int TestUnique = 0;
    public const int TestUniqueFlag = 1;

    public const int TotalComponents = 2;

    public static readonly string[] componentNames = 
    {
        "TestUnique",
        "TestUniqueFlag"
    };

    public static readonly System.Type[] componentTypes = 
    {
        typeof(TestUniqueComponent),
        typeof(TestUniqueFlagComponent)
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


// GameTestUniqueComponent.g.cs
public static class GameTestUniqueContextExtensions
{
    public static GameEntity GetTestUniqueEntity(this GameContext context) { return context.GetGroup(GameMatcher.TestUnique()).GetSingleEntity(); }
    public static TestUniqueComponent GetTestUnique(this GameContext context) { return context.GetTestUniqueEntity().GetTestUnique(); }
    public static bool HasTestUnique(this GameContext context) { return context.GetTestUniqueEntity() != null; }

    public static GameEntity SetTestUnique(this GameContext context, string newValue)
    {
        if (context.HasTestUnique())
        {
            throw new Entitas.EntitasException("Could not set TestUnique!\n" + context + " already has an entity with TestUniqueComponent!",
                "You should check if the context already has a GetTestUniqueEntity() before setting it or use context.ReplaceTestUnique().");
        }
        var entity = context.CreateEntity();
        entity.AddTestUnique(newValue);
        return entity;
    }

    public static void ReplaceTestUnique(this GameContext context, string newValue)
    {
        var entity = context.GetTestUniqueEntity();
        if (entity == null)
        {
            entity = context.SetTestUnique(newValue);
        }
        else
        {
            entity.ReplaceTestUnique(newValue);
        }
    }

    public static void RemoveTestUnique(this GameContext context)
    {
        context.GetTestUniqueEntity().Destroy();
    }
}

public static class GameTestUniqueEntityExtensions
{
    public static TestUniqueComponent GetTestUnique(this GameEntity entity) { return (TestUniqueComponent)entity.GetComponent(GameComponentsLookup.TestUnique); }
    public static bool HasTestUnique(this GameEntity entity) { return entity.HasComponent(GameComponentsLookup.TestUnique); }

    public static void AddTestUnique(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.TestUnique;
        var component = (TestUniqueComponent)entity.CreateComponent(index, typeof(TestUniqueComponent));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestUnique(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.TestUnique;
        var component = (TestUniqueComponent)entity.CreateComponent(index, typeof(TestUniqueComponent));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestUnique(this GameEntity entity)
    {
        entity.RemoveComponent(GameComponentsLookup.TestUnique);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherTestUnique;

    public static Entitas.IMatcher<GameEntity> TestUnique()
    {
        if (_matcherTestUnique == null)
        {
            var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.TestUnique);
            matcher.componentNames = GameComponentsLookup.componentNames;
            _matcherTestUnique = matcher;
        }

        return _matcherTestUnique;
    }
}


// GameTestUniqueFlagComponent.g.cs
public static class GameTestUniqueFlagContextExtensions
{
    public static GameEntity GetTestUniqueFlagEntity(this GameContext context) { return context.GetGroup(GameMatcher.TestUniqueFlag()).GetSingleEntity(); }

    public static bool IsTestUniqueFlag(this GameContext context)
    {
        return context.GetTestUniqueFlagEntity() != null;
    }

    public static void SetTestUniqueFlag(this GameContext context, bool value)
    {
        var entity = context.GetTestUniqueFlagEntity();
        if (value != (entity != null))
        {
            if (value)
            {
                context.CreateEntity().SetTestUniqueFlag(true);
            }
            else
            {
                entity.Destroy();
            }
        }
    }
}

public static class GameTestUniqueFlagEntityExtensions
{
    static readonly TestUniqueFlagComponent testUniqueFlagComponent = new TestUniqueFlagComponent();

    public static bool IsTestUniqueFlag(this GameEntity entity)
    {
        return entity.HasComponent(GameComponentsLookup.TestUniqueFlag);
    }

    public static void SetTestUniqueFlag(this GameEntity entity, bool value)
    {
        if (value != entity.IsTestUniqueFlag())
        {
            var index = GameComponentsLookup.TestUniqueFlag;
            if (value)
            {
                var componentPool = entity.GetComponentPool(index);
                var component = componentPool.Count > 0
                        ? componentPool.Pop()
                        : testUniqueFlagComponent;

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
    static Entitas.IMatcher<GameEntity> _matcherTestUniqueFlag;

    public static Entitas.IMatcher<GameEntity> TestUniqueFlag()
    {
        if (_matcherTestUniqueFlag == null)
        {
            var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.TestUniqueFlag);
            matcher.componentNames = GameComponentsLookup.componentNames;
            _matcherTestUniqueFlag = matcher;
        }

        return _matcherTestUniqueFlag;
    }
}
