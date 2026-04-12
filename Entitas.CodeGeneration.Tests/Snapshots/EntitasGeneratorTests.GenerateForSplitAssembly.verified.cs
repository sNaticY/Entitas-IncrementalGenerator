// ContextObservers.g.cs
public partial class Contexts
{
#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)

    [Entitas.CodeGeneration.Attributes.PostConstructor]
    public void InitializeContextObservers()
    {
        try
        {
            CreateContextObserver(game);
        }
        catch(System.Exception e)
        {
            UnityEngine.Debug.LogError(e);
        }
    }

    public void CreateContextObserver(Entitas.IContext context)
    {
        if (UnityEngine.Application.isPlaying)
        {
            var observer = new Entitas.VisualDebugging.Unity.ContextObserver(context);
            UnityEngine.Object.DontDestroyOnLoad(observer.gameObject);
        }
    }

#endif
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


// Feature.g.cs
#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)

public class Feature : Entitas.VisualDebugging.Unity.DebugSystems
{
    public Feature(string name) : base(name)
    {
    }

    public Feature() : base(true)
    {
        var typeName = DesperateDevs.Extensions.TypeExtension.ToCompilableString(GetType());
        var shortType = DesperateDevs.Extensions.TypeExtension.ShortTypeName(typeName);
        var readableType = DesperateDevs.Extensions.StringExtension.ToSpacedCamelCase(shortType);
        initialize(readableType);
    }
}

#elif (!ENTITAS_DISABLE_DEEP_PROFILING && DEVELOPMENT_BUILD)

public class Feature : Entitas.Systems
{
    System.Collections.Generic.List<string> _initializeSystemNames;
    System.Collections.Generic.List<string> _executeSystemNames;
    System.Collections.Generic.List<string> _cleanupSystemNames;
    System.Collections.Generic.List<string> _tearDownSystemNames;

    public Feature(string name) : this()
    {
    }

    public Feature()
    {
        _initializeSystemNames = new System.Collections.Generic.List<string>();
        _executeSystemNames = new System.Collections.Generic.List<string>();
        _cleanupSystemNames = new System.Collections.Generic.List<string>();
        _tearDownSystemNames = new System.Collections.Generic.List<string>();
    }

    public override Entitas.Systems Add(Entitas.ISystem system)
    {
        var systemName = system.GetType().FullName;

        if (system is Entitas.IInitializeSystem)
        {
            _initializeSystemNames.Add(systemName);
        }

        if (system is Entitas.IExecuteSystem)
        {
            _executeSystemNames.Add(systemName);
        }

        if (system is Entitas.ICleanupSystem)
        {
            _cleanupSystemNames.Add(systemName);
        }

        if (system is Entitas.ITearDownSystem)
        {
            _tearDownSystemNames.Add(systemName);
        }

        return base.Add(system);
    }

    public override void Initialize()
    {
        for (int i = 0; i < _initializeSystems.Count; i++)
        {
            UnityEngine.Profiling.Profiler.BeginSample(_initializeSystemNames[i]);
            _initializeSystems[i].Initialize();
            UnityEngine.Profiling.Profiler.EndSample();
        }
    }

    public override void Execute()
    {
        for (int i = 0; i < _executeSystems.Count; i++)
        {
            UnityEngine.Profiling.Profiler.BeginSample(_executeSystemNames[i]);
            _executeSystems[i].Execute();
            UnityEngine.Profiling.Profiler.EndSample();
        }
    }

    public override void Cleanup()
    {
        for (int i = 0; i < _cleanupSystems.Count; i++)
        {
            UnityEngine.Profiling.Profiler.BeginSample(_cleanupSystemNames[i]);
            _cleanupSystems[i].Cleanup();
            UnityEngine.Profiling.Profiler.EndSample();
        }
    }

    public override void TearDown()
    {
        for (int i = 0; i < _tearDownSystems.Count; i++)
        {
            UnityEngine.Profiling.Profiler.BeginSample(_tearDownSystemNames[i]);
            _tearDownSystems[i].TearDown();
            UnityEngine.Profiling.Profiler.EndSample();
        }
    }
}

#else

public class Feature : Entitas.Systems
{
    public Feature(string name)
    {
    }

    public Feature()
    {
    }
}

#endif


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
public partial class GameEntity
{
    static readonly TestFlagComponent testFlagComponent = new TestFlagComponent();

    public bool isTestFlag
    {
        get { return HasComponent(GameComponentsLookup.TestFlag); }
        set 
        {
            if (value != isTestFlag)
            {
                var index = GameComponentsLookup.TestFlag;
                if (value)
                {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : testFlagComponent;

                    AddComponent(index, component);
                }
                else
                {
                    RemoveComponent(index);
                }
            }
        }
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherTestFlag;

    public static Entitas.IMatcher<GameEntity> TestFlag
    {
        get
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
