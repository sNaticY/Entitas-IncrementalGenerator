// AudioCleanupSystems.g.cs
public sealed class AudioCleanupSystems : Feature
{
    public AudioCleanupSystems(Contexts contexts)
    {

    }
}


// AudioComponentsLookup.g.cs
public static class AudioComponentsLookup
{
    public const int TestMember = 0;
    public const int TestFlag = 1;
    public const int TestMember3 = 2;
    public const int TestMember2 = 3;
    public const int TestMember4 = 4;
    public const int TestMember5 = 5;
    public const int TestMember6 = 6;
    public const int TestMember7 = 7;
    public const int TestMember8 = 8;
    public const int TestMember99 = 9;

    public const int TotalComponents = 10;

    public static readonly string[] componentNames = 
    {
        "TestMember",
        "TestFlag",
        "TestMember3",
        "TestMember2",
        "TestMember4",
        "TestMember5",
        "TestMember6",
        "TestMember7",
        "TestMember8",
        "TestMember99"
    };

    public static readonly System.Type[] componentTypes = 
    {
        typeof(TestMemberComponent),
        typeof(TestFlagComponent),
        typeof(TestMember3Component),
        typeof(TestMember2Component),
        typeof(TestMember4Component),
        typeof(TestMember5Component),
        typeof(TestMember6Component),
        typeof(TestMember7Component),
        typeof(TestMember8Component),
        typeof(TestMember99Component)
    };
}


// AudioContext.g.cs
public sealed partial class AudioContext : Entitas.Context<AudioEntity>
{
    public AudioContext()
        : base(
            AudioComponentsLookup.TotalComponents,
            0,
            new Entitas.ContextInfo(
                "Audio",
                AudioComponentsLookup.componentNames,
                AudioComponentsLookup.componentTypes
            ),
            (entity) =>

#if (ENTITAS_FAST_AND_UNSAFE)
                new Entitas.UnsafeAERC(),
#else
                new Entitas.SafeAERC(entity),
#endif
            () => new AudioEntity()
        ) 
    {
    }
}


// AudioEntity.g.cs
public sealed partial class AudioEntity : Entitas.Entity
{
}


// AudioEventSystems.g.cs
public sealed class AudioEventSystems : Feature
{
    public AudioEventSystems(Contexts contexts)
    {

    }
}


// AudioMatcher.g.cs
public sealed partial class AudioMatcher 
{
    public static Entitas.IAllOfMatcher<AudioEntity> AllOf(params int[] indices) 
    {
        return Entitas.Matcher<AudioEntity>.AllOf(indices);
    }

    public static Entitas.IAllOfMatcher<AudioEntity> AllOf(params Entitas.IMatcher<AudioEntity>[] matchers)
    {
        return Entitas.Matcher<AudioEntity>.AllOf(matchers);
    }

    public static Entitas.IAnyOfMatcher<AudioEntity> AnyOf(params int[] indices)
    {
        return Entitas.Matcher<AudioEntity>.AnyOf(indices);
    }

    public static Entitas.IAnyOfMatcher<AudioEntity> AnyOf(params Entitas.IMatcher<AudioEntity>[] matchers)
    {
        return Entitas.Matcher<AudioEntity>.AnyOf(matchers);
    }
}


// AudioTestFlagComponent.g.cs
public static class AudioTestFlagEntityExtensions
{
    static readonly TestFlagComponent testFlagComponent = new TestFlagComponent();

    public static bool IsTestFlag(this AudioEntity entity)
    {
        return entity.HasComponent(AudioComponentsLookup.TestFlag);
    }

    public static void SetTestFlag(this AudioEntity entity, bool value)
    {
        if (value != entity.IsTestFlag())
        {
            var index = AudioComponentsLookup.TestFlag;
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

public sealed partial class AudioMatcher
{
    static Entitas.IMatcher<AudioEntity> _matcherTestFlag;

    public static Entitas.IMatcher<AudioEntity> TestFlag()
    {
        if (_matcherTestFlag == null)
        {
            var matcher = (Entitas.Matcher<AudioEntity>)Entitas.Matcher<AudioEntity>.AllOf(AudioComponentsLookup.TestFlag);
            matcher.componentNames = AudioComponentsLookup.componentNames;
            _matcherTestFlag = matcher;
        }

        return _matcherTestFlag;
    }
}


// AudioTestMember2Component.g.cs
public static class AudioTestMember2EntityExtensions
{
    public static TestMember2Component GetTestMember2(this AudioEntity entity) { return (TestMember2Component)entity.GetComponent(AudioComponentsLookup.TestMember2); }
    public static bool HasTestMember2(this AudioEntity entity) { return entity.HasComponent(AudioComponentsLookup.TestMember2); }

    public static void AddTestMember2(this AudioEntity entity, string newValue)
    {
        var index = AudioComponentsLookup.TestMember2;
        var component = (TestMember2Component)entity.CreateComponent(index, typeof(TestMember2Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember2(this AudioEntity entity, string newValue)
    {
        var index = AudioComponentsLookup.TestMember2;
        var component = (TestMember2Component)entity.CreateComponent(index, typeof(TestMember2Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember2(this AudioEntity entity)
    {
        entity.RemoveComponent(AudioComponentsLookup.TestMember2);
    }
}

public sealed partial class AudioMatcher
{
    static Entitas.IMatcher<AudioEntity> _matcherTestMember2;

    public static Entitas.IMatcher<AudioEntity> TestMember2()
    {
        if (_matcherTestMember2 == null)
        {
            var matcher = (Entitas.Matcher<AudioEntity>)Entitas.Matcher<AudioEntity>.AllOf(AudioComponentsLookup.TestMember2);
            matcher.componentNames = AudioComponentsLookup.componentNames;
            _matcherTestMember2 = matcher;
        }

        return _matcherTestMember2;
    }
}


// AudioTestMember3Component.g.cs
public static class AudioTestMember3EntityExtensions
{
    public static TestMember3Component GetTestMember3(this AudioEntity entity) { return (TestMember3Component)entity.GetComponent(AudioComponentsLookup.TestMember3); }
    public static bool HasTestMember3(this AudioEntity entity) { return entity.HasComponent(AudioComponentsLookup.TestMember3); }

    public static void AddTestMember3(this AudioEntity entity, string newValue)
    {
        var index = AudioComponentsLookup.TestMember3;
        var component = (TestMember3Component)entity.CreateComponent(index, typeof(TestMember3Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember3(this AudioEntity entity, string newValue)
    {
        var index = AudioComponentsLookup.TestMember3;
        var component = (TestMember3Component)entity.CreateComponent(index, typeof(TestMember3Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember3(this AudioEntity entity)
    {
        entity.RemoveComponent(AudioComponentsLookup.TestMember3);
    }
}

public sealed partial class AudioMatcher
{
    static Entitas.IMatcher<AudioEntity> _matcherTestMember3;

    public static Entitas.IMatcher<AudioEntity> TestMember3()
    {
        if (_matcherTestMember3 == null)
        {
            var matcher = (Entitas.Matcher<AudioEntity>)Entitas.Matcher<AudioEntity>.AllOf(AudioComponentsLookup.TestMember3);
            matcher.componentNames = AudioComponentsLookup.componentNames;
            _matcherTestMember3 = matcher;
        }

        return _matcherTestMember3;
    }
}


// AudioTestMember4Component.g.cs
public static class AudioTestMember4EntityExtensions
{
    public static TestMember4Component GetTestMember4(this AudioEntity entity) { return (TestMember4Component)entity.GetComponent(AudioComponentsLookup.TestMember4); }
    public static bool HasTestMember4(this AudioEntity entity) { return entity.HasComponent(AudioComponentsLookup.TestMember4); }

    public static void AddTestMember4(this AudioEntity entity, string newValue)
    {
        var index = AudioComponentsLookup.TestMember4;
        var component = (TestMember4Component)entity.CreateComponent(index, typeof(TestMember4Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember4(this AudioEntity entity, string newValue)
    {
        var index = AudioComponentsLookup.TestMember4;
        var component = (TestMember4Component)entity.CreateComponent(index, typeof(TestMember4Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember4(this AudioEntity entity)
    {
        entity.RemoveComponent(AudioComponentsLookup.TestMember4);
    }
}

public sealed partial class AudioMatcher
{
    static Entitas.IMatcher<AudioEntity> _matcherTestMember4;

    public static Entitas.IMatcher<AudioEntity> TestMember4()
    {
        if (_matcherTestMember4 == null)
        {
            var matcher = (Entitas.Matcher<AudioEntity>)Entitas.Matcher<AudioEntity>.AllOf(AudioComponentsLookup.TestMember4);
            matcher.componentNames = AudioComponentsLookup.componentNames;
            _matcherTestMember4 = matcher;
        }

        return _matcherTestMember4;
    }
}


// AudioTestMember5Component.g.cs
public static class AudioTestMember5EntityExtensions
{
    public static TestMember5Component GetTestMember5(this AudioEntity entity) { return (TestMember5Component)entity.GetComponent(AudioComponentsLookup.TestMember5); }
    public static bool HasTestMember5(this AudioEntity entity) { return entity.HasComponent(AudioComponentsLookup.TestMember5); }

    public static void AddTestMember5(this AudioEntity entity, string newValue)
    {
        var index = AudioComponentsLookup.TestMember5;
        var component = (TestMember5Component)entity.CreateComponent(index, typeof(TestMember5Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember5(this AudioEntity entity, string newValue)
    {
        var index = AudioComponentsLookup.TestMember5;
        var component = (TestMember5Component)entity.CreateComponent(index, typeof(TestMember5Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember5(this AudioEntity entity)
    {
        entity.RemoveComponent(AudioComponentsLookup.TestMember5);
    }
}

public sealed partial class AudioMatcher
{
    static Entitas.IMatcher<AudioEntity> _matcherTestMember5;

    public static Entitas.IMatcher<AudioEntity> TestMember5()
    {
        if (_matcherTestMember5 == null)
        {
            var matcher = (Entitas.Matcher<AudioEntity>)Entitas.Matcher<AudioEntity>.AllOf(AudioComponentsLookup.TestMember5);
            matcher.componentNames = AudioComponentsLookup.componentNames;
            _matcherTestMember5 = matcher;
        }

        return _matcherTestMember5;
    }
}


// AudioTestMember6Component.g.cs
public static class AudioTestMember6EntityExtensions
{
    public static TestMember6Component GetTestMember6(this AudioEntity entity) { return (TestMember6Component)entity.GetComponent(AudioComponentsLookup.TestMember6); }
    public static bool HasTestMember6(this AudioEntity entity) { return entity.HasComponent(AudioComponentsLookup.TestMember6); }

    public static void AddTestMember6(this AudioEntity entity, string newValue)
    {
        var index = AudioComponentsLookup.TestMember6;
        var component = (TestMember6Component)entity.CreateComponent(index, typeof(TestMember6Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember6(this AudioEntity entity, string newValue)
    {
        var index = AudioComponentsLookup.TestMember6;
        var component = (TestMember6Component)entity.CreateComponent(index, typeof(TestMember6Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember6(this AudioEntity entity)
    {
        entity.RemoveComponent(AudioComponentsLookup.TestMember6);
    }
}

public sealed partial class AudioMatcher
{
    static Entitas.IMatcher<AudioEntity> _matcherTestMember6;

    public static Entitas.IMatcher<AudioEntity> TestMember6()
    {
        if (_matcherTestMember6 == null)
        {
            var matcher = (Entitas.Matcher<AudioEntity>)Entitas.Matcher<AudioEntity>.AllOf(AudioComponentsLookup.TestMember6);
            matcher.componentNames = AudioComponentsLookup.componentNames;
            _matcherTestMember6 = matcher;
        }

        return _matcherTestMember6;
    }
}


// AudioTestMember7Component.g.cs
public static class AudioTestMember7EntityExtensions
{
    public static TestMember7Component GetTestMember7(this AudioEntity entity) { return (TestMember7Component)entity.GetComponent(AudioComponentsLookup.TestMember7); }
    public static bool HasTestMember7(this AudioEntity entity) { return entity.HasComponent(AudioComponentsLookup.TestMember7); }

    public static void AddTestMember7(this AudioEntity entity, string newValue)
    {
        var index = AudioComponentsLookup.TestMember7;
        var component = (TestMember7Component)entity.CreateComponent(index, typeof(TestMember7Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember7(this AudioEntity entity, string newValue)
    {
        var index = AudioComponentsLookup.TestMember7;
        var component = (TestMember7Component)entity.CreateComponent(index, typeof(TestMember7Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember7(this AudioEntity entity)
    {
        entity.RemoveComponent(AudioComponentsLookup.TestMember7);
    }
}

public sealed partial class AudioMatcher
{
    static Entitas.IMatcher<AudioEntity> _matcherTestMember7;

    public static Entitas.IMatcher<AudioEntity> TestMember7()
    {
        if (_matcherTestMember7 == null)
        {
            var matcher = (Entitas.Matcher<AudioEntity>)Entitas.Matcher<AudioEntity>.AllOf(AudioComponentsLookup.TestMember7);
            matcher.componentNames = AudioComponentsLookup.componentNames;
            _matcherTestMember7 = matcher;
        }

        return _matcherTestMember7;
    }
}


// AudioTestMember8Component.g.cs
public static class AudioTestMember8EntityExtensions
{
    public static TestMember8Component GetTestMember8(this AudioEntity entity) { return (TestMember8Component)entity.GetComponent(AudioComponentsLookup.TestMember8); }
    public static bool HasTestMember8(this AudioEntity entity) { return entity.HasComponent(AudioComponentsLookup.TestMember8); }

    public static void AddTestMember8(this AudioEntity entity, string newValue)
    {
        var index = AudioComponentsLookup.TestMember8;
        var component = (TestMember8Component)entity.CreateComponent(index, typeof(TestMember8Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember8(this AudioEntity entity, string newValue)
    {
        var index = AudioComponentsLookup.TestMember8;
        var component = (TestMember8Component)entity.CreateComponent(index, typeof(TestMember8Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember8(this AudioEntity entity)
    {
        entity.RemoveComponent(AudioComponentsLookup.TestMember8);
    }
}

public sealed partial class AudioMatcher
{
    static Entitas.IMatcher<AudioEntity> _matcherTestMember8;

    public static Entitas.IMatcher<AudioEntity> TestMember8()
    {
        if (_matcherTestMember8 == null)
        {
            var matcher = (Entitas.Matcher<AudioEntity>)Entitas.Matcher<AudioEntity>.AllOf(AudioComponentsLookup.TestMember8);
            matcher.componentNames = AudioComponentsLookup.componentNames;
            _matcherTestMember8 = matcher;
        }

        return _matcherTestMember8;
    }
}


// AudioTestMember99Component.g.cs
public static class AudioTestMember99EntityExtensions
{
    public static TestMember99Component GetTestMember99(this AudioEntity entity) { return (TestMember99Component)entity.GetComponent(AudioComponentsLookup.TestMember99); }
    public static bool HasTestMember99(this AudioEntity entity) { return entity.HasComponent(AudioComponentsLookup.TestMember99); }

    public static void AddTestMember99(this AudioEntity entity, string newValue)
    {
        var index = AudioComponentsLookup.TestMember99;
        var component = (TestMember99Component)entity.CreateComponent(index, typeof(TestMember99Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember99(this AudioEntity entity, string newValue)
    {
        var index = AudioComponentsLookup.TestMember99;
        var component = (TestMember99Component)entity.CreateComponent(index, typeof(TestMember99Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember99(this AudioEntity entity)
    {
        entity.RemoveComponent(AudioComponentsLookup.TestMember99);
    }
}

public sealed partial class AudioMatcher
{
    static Entitas.IMatcher<AudioEntity> _matcherTestMember99;

    public static Entitas.IMatcher<AudioEntity> TestMember99()
    {
        if (_matcherTestMember99 == null)
        {
            var matcher = (Entitas.Matcher<AudioEntity>)Entitas.Matcher<AudioEntity>.AllOf(AudioComponentsLookup.TestMember99);
            matcher.componentNames = AudioComponentsLookup.componentNames;
            _matcherTestMember99 = matcher;
        }

        return _matcherTestMember99;
    }
}


// AudioTestMemberComponent.g.cs
public static class AudioTestMemberEntityExtensions
{
    public static TestMemberComponent GetTestMember(this AudioEntity entity) { return (TestMemberComponent)entity.GetComponent(AudioComponentsLookup.TestMember); }
    public static bool HasTestMember(this AudioEntity entity) { return entity.HasComponent(AudioComponentsLookup.TestMember); }

    public static void AddTestMember(this AudioEntity entity, string newValue)
    {
        var index = AudioComponentsLookup.TestMember;
        var component = (TestMemberComponent)entity.CreateComponent(index, typeof(TestMemberComponent));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember(this AudioEntity entity, string newValue)
    {
        var index = AudioComponentsLookup.TestMember;
        var component = (TestMemberComponent)entity.CreateComponent(index, typeof(TestMemberComponent));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember(this AudioEntity entity)
    {
        entity.RemoveComponent(AudioComponentsLookup.TestMember);
    }
}

public sealed partial class AudioMatcher
{
    static Entitas.IMatcher<AudioEntity> _matcherTestMember;

    public static Entitas.IMatcher<AudioEntity> TestMember()
    {
        if (_matcherTestMember == null)
        {
            var matcher = (Entitas.Matcher<AudioEntity>)Entitas.Matcher<AudioEntity>.AllOf(AudioComponentsLookup.TestMember);
            matcher.componentNames = AudioComponentsLookup.componentNames;
            _matcherTestMember = matcher;
        }

        return _matcherTestMember;
    }
}


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
    public AudioContext audio { get; set; }

    public Entitas.IContext[] allContexts { get { return new Entitas.IContext [] { game, input, audio }; } }

    public Contexts()
    {
        game = new GameContext();
        input = new InputContext();
        audio = new AudioContext();

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
    public const int TestFlag = 1;
    public const int TestMember3 = 2;
    public const int TestMember2 = 3;
    public const int TestMember4 = 4;
    public const int TestMember5 = 5;
    public const int TestMember6 = 6;
    public const int TestMember7 = 7;
    public const int TestMember8 = 8;
    public const int TestMember99 = 9;

    public const int TotalComponents = 10;

    public static readonly string[] componentNames = 
    {
        "TestMember",
        "TestFlag",
        "TestMember3",
        "TestMember2",
        "TestMember4",
        "TestMember5",
        "TestMember6",
        "TestMember7",
        "TestMember8",
        "TestMember99"
    };

    public static readonly System.Type[] componentTypes = 
    {
        typeof(TestMemberComponent),
        typeof(TestFlagComponent),
        typeof(TestMember3Component),
        typeof(TestMember2Component),
        typeof(TestMember4Component),
        typeof(TestMember5Component),
        typeof(TestMember6Component),
        typeof(TestMember7Component),
        typeof(TestMember8Component),
        typeof(TestMember99Component)
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


// GameTestMember2Component.g.cs
public static class GameTestMember2EntityExtensions
{
    public static TestMember2Component GetTestMember2(this GameEntity entity) { return (TestMember2Component)entity.GetComponent(GameComponentsLookup.TestMember2); }
    public static bool HasTestMember2(this GameEntity entity) { return entity.HasComponent(GameComponentsLookup.TestMember2); }

    public static void AddTestMember2(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.TestMember2;
        var component = (TestMember2Component)entity.CreateComponent(index, typeof(TestMember2Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember2(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.TestMember2;
        var component = (TestMember2Component)entity.CreateComponent(index, typeof(TestMember2Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember2(this GameEntity entity)
    {
        entity.RemoveComponent(GameComponentsLookup.TestMember2);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherTestMember2;

    public static Entitas.IMatcher<GameEntity> TestMember2()
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


// GameTestMember3Component.g.cs
public static class GameTestMember3EntityExtensions
{
    public static TestMember3Component GetTestMember3(this GameEntity entity) { return (TestMember3Component)entity.GetComponent(GameComponentsLookup.TestMember3); }
    public static bool HasTestMember3(this GameEntity entity) { return entity.HasComponent(GameComponentsLookup.TestMember3); }

    public static void AddTestMember3(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.TestMember3;
        var component = (TestMember3Component)entity.CreateComponent(index, typeof(TestMember3Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember3(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.TestMember3;
        var component = (TestMember3Component)entity.CreateComponent(index, typeof(TestMember3Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember3(this GameEntity entity)
    {
        entity.RemoveComponent(GameComponentsLookup.TestMember3);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherTestMember3;

    public static Entitas.IMatcher<GameEntity> TestMember3()
    {
        if (_matcherTestMember3 == null)
        {
            var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.TestMember3);
            matcher.componentNames = GameComponentsLookup.componentNames;
            _matcherTestMember3 = matcher;
        }

        return _matcherTestMember3;
    }
}


// GameTestMember4Component.g.cs
public static class GameTestMember4EntityExtensions
{
    public static TestMember4Component GetTestMember4(this GameEntity entity) { return (TestMember4Component)entity.GetComponent(GameComponentsLookup.TestMember4); }
    public static bool HasTestMember4(this GameEntity entity) { return entity.HasComponent(GameComponentsLookup.TestMember4); }

    public static void AddTestMember4(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.TestMember4;
        var component = (TestMember4Component)entity.CreateComponent(index, typeof(TestMember4Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember4(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.TestMember4;
        var component = (TestMember4Component)entity.CreateComponent(index, typeof(TestMember4Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember4(this GameEntity entity)
    {
        entity.RemoveComponent(GameComponentsLookup.TestMember4);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherTestMember4;

    public static Entitas.IMatcher<GameEntity> TestMember4()
    {
        if (_matcherTestMember4 == null)
        {
            var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.TestMember4);
            matcher.componentNames = GameComponentsLookup.componentNames;
            _matcherTestMember4 = matcher;
        }

        return _matcherTestMember4;
    }
}


// GameTestMember5Component.g.cs
public static class GameTestMember5EntityExtensions
{
    public static TestMember5Component GetTestMember5(this GameEntity entity) { return (TestMember5Component)entity.GetComponent(GameComponentsLookup.TestMember5); }
    public static bool HasTestMember5(this GameEntity entity) { return entity.HasComponent(GameComponentsLookup.TestMember5); }

    public static void AddTestMember5(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.TestMember5;
        var component = (TestMember5Component)entity.CreateComponent(index, typeof(TestMember5Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember5(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.TestMember5;
        var component = (TestMember5Component)entity.CreateComponent(index, typeof(TestMember5Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember5(this GameEntity entity)
    {
        entity.RemoveComponent(GameComponentsLookup.TestMember5);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherTestMember5;

    public static Entitas.IMatcher<GameEntity> TestMember5()
    {
        if (_matcherTestMember5 == null)
        {
            var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.TestMember5);
            matcher.componentNames = GameComponentsLookup.componentNames;
            _matcherTestMember5 = matcher;
        }

        return _matcherTestMember5;
    }
}


// GameTestMember6Component.g.cs
public static class GameTestMember6EntityExtensions
{
    public static TestMember6Component GetTestMember6(this GameEntity entity) { return (TestMember6Component)entity.GetComponent(GameComponentsLookup.TestMember6); }
    public static bool HasTestMember6(this GameEntity entity) { return entity.HasComponent(GameComponentsLookup.TestMember6); }

    public static void AddTestMember6(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.TestMember6;
        var component = (TestMember6Component)entity.CreateComponent(index, typeof(TestMember6Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember6(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.TestMember6;
        var component = (TestMember6Component)entity.CreateComponent(index, typeof(TestMember6Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember6(this GameEntity entity)
    {
        entity.RemoveComponent(GameComponentsLookup.TestMember6);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherTestMember6;

    public static Entitas.IMatcher<GameEntity> TestMember6()
    {
        if (_matcherTestMember6 == null)
        {
            var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.TestMember6);
            matcher.componentNames = GameComponentsLookup.componentNames;
            _matcherTestMember6 = matcher;
        }

        return _matcherTestMember6;
    }
}


// GameTestMember7Component.g.cs
public static class GameTestMember7EntityExtensions
{
    public static TestMember7Component GetTestMember7(this GameEntity entity) { return (TestMember7Component)entity.GetComponent(GameComponentsLookup.TestMember7); }
    public static bool HasTestMember7(this GameEntity entity) { return entity.HasComponent(GameComponentsLookup.TestMember7); }

    public static void AddTestMember7(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.TestMember7;
        var component = (TestMember7Component)entity.CreateComponent(index, typeof(TestMember7Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember7(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.TestMember7;
        var component = (TestMember7Component)entity.CreateComponent(index, typeof(TestMember7Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember7(this GameEntity entity)
    {
        entity.RemoveComponent(GameComponentsLookup.TestMember7);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherTestMember7;

    public static Entitas.IMatcher<GameEntity> TestMember7()
    {
        if (_matcherTestMember7 == null)
        {
            var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.TestMember7);
            matcher.componentNames = GameComponentsLookup.componentNames;
            _matcherTestMember7 = matcher;
        }

        return _matcherTestMember7;
    }
}


// GameTestMember8Component.g.cs
public static class GameTestMember8EntityExtensions
{
    public static TestMember8Component GetTestMember8(this GameEntity entity) { return (TestMember8Component)entity.GetComponent(GameComponentsLookup.TestMember8); }
    public static bool HasTestMember8(this GameEntity entity) { return entity.HasComponent(GameComponentsLookup.TestMember8); }

    public static void AddTestMember8(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.TestMember8;
        var component = (TestMember8Component)entity.CreateComponent(index, typeof(TestMember8Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember8(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.TestMember8;
        var component = (TestMember8Component)entity.CreateComponent(index, typeof(TestMember8Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember8(this GameEntity entity)
    {
        entity.RemoveComponent(GameComponentsLookup.TestMember8);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherTestMember8;

    public static Entitas.IMatcher<GameEntity> TestMember8()
    {
        if (_matcherTestMember8 == null)
        {
            var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.TestMember8);
            matcher.componentNames = GameComponentsLookup.componentNames;
            _matcherTestMember8 = matcher;
        }

        return _matcherTestMember8;
    }
}


// GameTestMember99Component.g.cs
public static class GameTestMember99EntityExtensions
{
    public static TestMember99Component GetTestMember99(this GameEntity entity) { return (TestMember99Component)entity.GetComponent(GameComponentsLookup.TestMember99); }
    public static bool HasTestMember99(this GameEntity entity) { return entity.HasComponent(GameComponentsLookup.TestMember99); }

    public static void AddTestMember99(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.TestMember99;
        var component = (TestMember99Component)entity.CreateComponent(index, typeof(TestMember99Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember99(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.TestMember99;
        var component = (TestMember99Component)entity.CreateComponent(index, typeof(TestMember99Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember99(this GameEntity entity)
    {
        entity.RemoveComponent(GameComponentsLookup.TestMember99);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherTestMember99;

    public static Entitas.IMatcher<GameEntity> TestMember99()
    {
        if (_matcherTestMember99 == null)
        {
            var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.TestMember99);
            matcher.componentNames = GameComponentsLookup.componentNames;
            _matcherTestMember99 = matcher;
        }

        return _matcherTestMember99;
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
    public const int TestMember = 0;
    public const int TestFlag = 1;
    public const int TestMember3 = 2;
    public const int TestMember2 = 3;
    public const int TestMember4 = 4;
    public const int TestMember5 = 5;
    public const int TestMember6 = 6;
    public const int TestMember7 = 7;
    public const int TestMember8 = 8;
    public const int TestMember99 = 9;

    public const int TotalComponents = 10;

    public static readonly string[] componentNames = 
    {
        "TestMember",
        "TestFlag",
        "TestMember3",
        "TestMember2",
        "TestMember4",
        "TestMember5",
        "TestMember6",
        "TestMember7",
        "TestMember8",
        "TestMember99"
    };

    public static readonly System.Type[] componentTypes = 
    {
        typeof(TestMemberComponent),
        typeof(TestFlagComponent),
        typeof(TestMember3Component),
        typeof(TestMember2Component),
        typeof(TestMember4Component),
        typeof(TestMember5Component),
        typeof(TestMember6Component),
        typeof(TestMember7Component),
        typeof(TestMember8Component),
        typeof(TestMember99Component)
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


// InputTestMember2Component.g.cs
public static class InputTestMember2EntityExtensions
{
    public static TestMember2Component GetTestMember2(this InputEntity entity) { return (TestMember2Component)entity.GetComponent(InputComponentsLookup.TestMember2); }
    public static bool HasTestMember2(this InputEntity entity) { return entity.HasComponent(InputComponentsLookup.TestMember2); }

    public static void AddTestMember2(this InputEntity entity, string newValue)
    {
        var index = InputComponentsLookup.TestMember2;
        var component = (TestMember2Component)entity.CreateComponent(index, typeof(TestMember2Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember2(this InputEntity entity, string newValue)
    {
        var index = InputComponentsLookup.TestMember2;
        var component = (TestMember2Component)entity.CreateComponent(index, typeof(TestMember2Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember2(this InputEntity entity)
    {
        entity.RemoveComponent(InputComponentsLookup.TestMember2);
    }
}

public sealed partial class InputMatcher
{
    static Entitas.IMatcher<InputEntity> _matcherTestMember2;

    public static Entitas.IMatcher<InputEntity> TestMember2()
    {
        if (_matcherTestMember2 == null)
        {
            var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.TestMember2);
            matcher.componentNames = InputComponentsLookup.componentNames;
            _matcherTestMember2 = matcher;
        }

        return _matcherTestMember2;
    }
}


// InputTestMember3Component.g.cs
public static class InputTestMember3EntityExtensions
{
    public static TestMember3Component GetTestMember3(this InputEntity entity) { return (TestMember3Component)entity.GetComponent(InputComponentsLookup.TestMember3); }
    public static bool HasTestMember3(this InputEntity entity) { return entity.HasComponent(InputComponentsLookup.TestMember3); }

    public static void AddTestMember3(this InputEntity entity, string newValue)
    {
        var index = InputComponentsLookup.TestMember3;
        var component = (TestMember3Component)entity.CreateComponent(index, typeof(TestMember3Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember3(this InputEntity entity, string newValue)
    {
        var index = InputComponentsLookup.TestMember3;
        var component = (TestMember3Component)entity.CreateComponent(index, typeof(TestMember3Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember3(this InputEntity entity)
    {
        entity.RemoveComponent(InputComponentsLookup.TestMember3);
    }
}

public sealed partial class InputMatcher
{
    static Entitas.IMatcher<InputEntity> _matcherTestMember3;

    public static Entitas.IMatcher<InputEntity> TestMember3()
    {
        if (_matcherTestMember3 == null)
        {
            var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.TestMember3);
            matcher.componentNames = InputComponentsLookup.componentNames;
            _matcherTestMember3 = matcher;
        }

        return _matcherTestMember3;
    }
}


// InputTestMember4Component.g.cs
public static class InputTestMember4EntityExtensions
{
    public static TestMember4Component GetTestMember4(this InputEntity entity) { return (TestMember4Component)entity.GetComponent(InputComponentsLookup.TestMember4); }
    public static bool HasTestMember4(this InputEntity entity) { return entity.HasComponent(InputComponentsLookup.TestMember4); }

    public static void AddTestMember4(this InputEntity entity, string newValue)
    {
        var index = InputComponentsLookup.TestMember4;
        var component = (TestMember4Component)entity.CreateComponent(index, typeof(TestMember4Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember4(this InputEntity entity, string newValue)
    {
        var index = InputComponentsLookup.TestMember4;
        var component = (TestMember4Component)entity.CreateComponent(index, typeof(TestMember4Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember4(this InputEntity entity)
    {
        entity.RemoveComponent(InputComponentsLookup.TestMember4);
    }
}

public sealed partial class InputMatcher
{
    static Entitas.IMatcher<InputEntity> _matcherTestMember4;

    public static Entitas.IMatcher<InputEntity> TestMember4()
    {
        if (_matcherTestMember4 == null)
        {
            var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.TestMember4);
            matcher.componentNames = InputComponentsLookup.componentNames;
            _matcherTestMember4 = matcher;
        }

        return _matcherTestMember4;
    }
}


// InputTestMember5Component.g.cs
public static class InputTestMember5EntityExtensions
{
    public static TestMember5Component GetTestMember5(this InputEntity entity) { return (TestMember5Component)entity.GetComponent(InputComponentsLookup.TestMember5); }
    public static bool HasTestMember5(this InputEntity entity) { return entity.HasComponent(InputComponentsLookup.TestMember5); }

    public static void AddTestMember5(this InputEntity entity, string newValue)
    {
        var index = InputComponentsLookup.TestMember5;
        var component = (TestMember5Component)entity.CreateComponent(index, typeof(TestMember5Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember5(this InputEntity entity, string newValue)
    {
        var index = InputComponentsLookup.TestMember5;
        var component = (TestMember5Component)entity.CreateComponent(index, typeof(TestMember5Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember5(this InputEntity entity)
    {
        entity.RemoveComponent(InputComponentsLookup.TestMember5);
    }
}

public sealed partial class InputMatcher
{
    static Entitas.IMatcher<InputEntity> _matcherTestMember5;

    public static Entitas.IMatcher<InputEntity> TestMember5()
    {
        if (_matcherTestMember5 == null)
        {
            var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.TestMember5);
            matcher.componentNames = InputComponentsLookup.componentNames;
            _matcherTestMember5 = matcher;
        }

        return _matcherTestMember5;
    }
}


// InputTestMember6Component.g.cs
public static class InputTestMember6EntityExtensions
{
    public static TestMember6Component GetTestMember6(this InputEntity entity) { return (TestMember6Component)entity.GetComponent(InputComponentsLookup.TestMember6); }
    public static bool HasTestMember6(this InputEntity entity) { return entity.HasComponent(InputComponentsLookup.TestMember6); }

    public static void AddTestMember6(this InputEntity entity, string newValue)
    {
        var index = InputComponentsLookup.TestMember6;
        var component = (TestMember6Component)entity.CreateComponent(index, typeof(TestMember6Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember6(this InputEntity entity, string newValue)
    {
        var index = InputComponentsLookup.TestMember6;
        var component = (TestMember6Component)entity.CreateComponent(index, typeof(TestMember6Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember6(this InputEntity entity)
    {
        entity.RemoveComponent(InputComponentsLookup.TestMember6);
    }
}

public sealed partial class InputMatcher
{
    static Entitas.IMatcher<InputEntity> _matcherTestMember6;

    public static Entitas.IMatcher<InputEntity> TestMember6()
    {
        if (_matcherTestMember6 == null)
        {
            var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.TestMember6);
            matcher.componentNames = InputComponentsLookup.componentNames;
            _matcherTestMember6 = matcher;
        }

        return _matcherTestMember6;
    }
}


// InputTestMember7Component.g.cs
public static class InputTestMember7EntityExtensions
{
    public static TestMember7Component GetTestMember7(this InputEntity entity) { return (TestMember7Component)entity.GetComponent(InputComponentsLookup.TestMember7); }
    public static bool HasTestMember7(this InputEntity entity) { return entity.HasComponent(InputComponentsLookup.TestMember7); }

    public static void AddTestMember7(this InputEntity entity, string newValue)
    {
        var index = InputComponentsLookup.TestMember7;
        var component = (TestMember7Component)entity.CreateComponent(index, typeof(TestMember7Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember7(this InputEntity entity, string newValue)
    {
        var index = InputComponentsLookup.TestMember7;
        var component = (TestMember7Component)entity.CreateComponent(index, typeof(TestMember7Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember7(this InputEntity entity)
    {
        entity.RemoveComponent(InputComponentsLookup.TestMember7);
    }
}

public sealed partial class InputMatcher
{
    static Entitas.IMatcher<InputEntity> _matcherTestMember7;

    public static Entitas.IMatcher<InputEntity> TestMember7()
    {
        if (_matcherTestMember7 == null)
        {
            var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.TestMember7);
            matcher.componentNames = InputComponentsLookup.componentNames;
            _matcherTestMember7 = matcher;
        }

        return _matcherTestMember7;
    }
}


// InputTestMember8Component.g.cs
public static class InputTestMember8EntityExtensions
{
    public static TestMember8Component GetTestMember8(this InputEntity entity) { return (TestMember8Component)entity.GetComponent(InputComponentsLookup.TestMember8); }
    public static bool HasTestMember8(this InputEntity entity) { return entity.HasComponent(InputComponentsLookup.TestMember8); }

    public static void AddTestMember8(this InputEntity entity, string newValue)
    {
        var index = InputComponentsLookup.TestMember8;
        var component = (TestMember8Component)entity.CreateComponent(index, typeof(TestMember8Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember8(this InputEntity entity, string newValue)
    {
        var index = InputComponentsLookup.TestMember8;
        var component = (TestMember8Component)entity.CreateComponent(index, typeof(TestMember8Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember8(this InputEntity entity)
    {
        entity.RemoveComponent(InputComponentsLookup.TestMember8);
    }
}

public sealed partial class InputMatcher
{
    static Entitas.IMatcher<InputEntity> _matcherTestMember8;

    public static Entitas.IMatcher<InputEntity> TestMember8()
    {
        if (_matcherTestMember8 == null)
        {
            var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.TestMember8);
            matcher.componentNames = InputComponentsLookup.componentNames;
            _matcherTestMember8 = matcher;
        }

        return _matcherTestMember8;
    }
}


// InputTestMember99Component.g.cs
public static class InputTestMember99EntityExtensions
{
    public static TestMember99Component GetTestMember99(this InputEntity entity) { return (TestMember99Component)entity.GetComponent(InputComponentsLookup.TestMember99); }
    public static bool HasTestMember99(this InputEntity entity) { return entity.HasComponent(InputComponentsLookup.TestMember99); }

    public static void AddTestMember99(this InputEntity entity, string newValue)
    {
        var index = InputComponentsLookup.TestMember99;
        var component = (TestMember99Component)entity.CreateComponent(index, typeof(TestMember99Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestMember99(this InputEntity entity, string newValue)
    {
        var index = InputComponentsLookup.TestMember99;
        var component = (TestMember99Component)entity.CreateComponent(index, typeof(TestMember99Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestMember99(this InputEntity entity)
    {
        entity.RemoveComponent(InputComponentsLookup.TestMember99);
    }
}

public sealed partial class InputMatcher
{
    static Entitas.IMatcher<InputEntity> _matcherTestMember99;

    public static Entitas.IMatcher<InputEntity> TestMember99()
    {
        if (_matcherTestMember99 == null)
        {
            var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.TestMember99);
            matcher.componentNames = InputComponentsLookup.componentNames;
            _matcherTestMember99 = matcher;
        }

        return _matcherTestMember99;
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
