// AnyTestEventEventSystem.g.cs
public sealed class AnyTestEventEventSystem : Entitas.ReactiveSystem<GameEntity>
{
    readonly Entitas.IGroup<GameEntity> _listeners;
    readonly System.Collections.Generic.List<GameEntity> _entityBuffer;
    readonly System.Collections.Generic.List<IAnyTestEventListener> _listenerBuffer;

    public AnyTestEventEventSystem(Contexts contexts) : base(contexts.game)
    {
        _listeners = contexts.game.GetGroup(GameMatcher.AnyTestEventListener());
        _entityBuffer = new System.Collections.Generic.List<GameEntity>();
        _listenerBuffer = new System.Collections.Generic.List<IAnyTestEventListener>();
    }

    protected override Entitas.ICollector<GameEntity> GetTrigger(Entitas.IContext<GameEntity> context)
    {
        return Entitas.CollectorContextExtension.CreateCollector(
            context, Entitas.TriggerOnEventMatcherExtension.Added(GameMatcher.TestEvent())
        );
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.HasTestEvent();
    }

    protected override void Execute(System.Collections.Generic.List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            var component = e.GetTestEvent();
            foreach (var listenerEntity in _listeners.GetEntities(_entityBuffer))
            {
                _listenerBuffer.Clear();
                _listenerBuffer.AddRange(listenerEntity.GetAnyTestEventListener().value);
                foreach (var listener in _listenerBuffer)
                {
                    listener.OnAnyTestEvent(e, component.Value);
                }
            }
        }
    }
}


// AnyTestEventListenerComponent.g.cs
[Entitas.CodeGeneration.Attributes.DontGenerate]
public sealed class AnyTestEventListenerComponent : Entitas.IComponent
{
    public System.Collections.Generic.List<IAnyTestEventListener> value;
}


// AnyTestEventRemovedEventSystem.g.cs
public sealed class AnyTestEventRemovedEventSystem : Entitas.ReactiveSystem<GameEntity>
{
    readonly Entitas.IGroup<GameEntity> _listeners;
    readonly System.Collections.Generic.List<GameEntity> _entityBuffer;
    readonly System.Collections.Generic.List<IAnyTestEventRemovedListener> _listenerBuffer;

    public AnyTestEventRemovedEventSystem(Contexts contexts) : base(contexts.game)
    {
        _listeners = contexts.game.GetGroup(GameMatcher.AnyTestEventRemovedListener());
        _entityBuffer = new System.Collections.Generic.List<GameEntity>();
        _listenerBuffer = new System.Collections.Generic.List<IAnyTestEventRemovedListener>();
    }

    protected override Entitas.ICollector<GameEntity> GetTrigger(Entitas.IContext<GameEntity> context)
    {
        return Entitas.CollectorContextExtension.CreateCollector(
            context, Entitas.TriggerOnEventMatcherExtension.Removed(GameMatcher.TestEvent())
        );
    }

    protected override bool Filter(GameEntity entity)
    {
        return !entity.HasTestEvent();
    }

    protected override void Execute(System.Collections.Generic.List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            
            foreach (var listenerEntity in _listeners.GetEntities(_entityBuffer))
            {
                _listenerBuffer.Clear();
                _listenerBuffer.AddRange(listenerEntity.GetAnyTestEventRemovedListener().value);
                foreach (var listener in _listenerBuffer)
                {
                    listener.OnAnyTestEventRemoved(e);
                }
            }
        }
    }
}


// AnyTestEventRemovedListenerComponent.g.cs
[Entitas.CodeGeneration.Attributes.DontGenerate]
public sealed class AnyTestEventRemovedListenerComponent : Entitas.IComponent
{
    public System.Collections.Generic.List<IAnyTestEventRemovedListener> value;
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


// GameAnyTestEventListenerComponent.g.cs
public static class GameAnyTestEventListenerEntityExtensions
{
    public static AnyTestEventListenerComponent GetAnyTestEventListener(this GameEntity entity) { return (AnyTestEventListenerComponent)entity.GetComponent(GameComponentsLookup.AnyTestEventListener); }
    public static bool HasAnyTestEventListener(this GameEntity entity) { return entity.HasComponent(GameComponentsLookup.AnyTestEventListener); }

    public static void AddAnyTestEventListener(this GameEntity entity, System.Collections.Generic.List<IAnyTestEventListener> newValue)
    {
        var index = GameComponentsLookup.AnyTestEventListener;
        var component = (AnyTestEventListenerComponent)entity.CreateComponent(index, typeof(AnyTestEventListenerComponent));
        component.value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceAnyTestEventListener(this GameEntity entity, System.Collections.Generic.List<IAnyTestEventListener> newValue)
    {
        var index = GameComponentsLookup.AnyTestEventListener;
        var component = (AnyTestEventListenerComponent)entity.CreateComponent(index, typeof(AnyTestEventListenerComponent));
        component.value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveAnyTestEventListener(this GameEntity entity)
    {
        entity.RemoveComponent(GameComponentsLookup.AnyTestEventListener);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherAnyTestEventListener;

    public static Entitas.IMatcher<GameEntity> AnyTestEventListener()
    {
        if (_matcherAnyTestEventListener == null)
        {
            var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.AnyTestEventListener);
            matcher.componentNames = GameComponentsLookup.componentNames;
            _matcherAnyTestEventListener = matcher;
        }

        return _matcherAnyTestEventListener;
    }
}


// GameAnyTestEventListenerComponentEvent.g.cs
public static class GameAnyTestEventListenerComponentEventExtensions
{
    public static void AddAnyTestEventListener(this GameEntity entity, IAnyTestEventListener value)
    {
        var listeners = entity.HasAnyTestEventListener()
            ? entity.GetAnyTestEventListener().value
            : new System.Collections.Generic.List<IAnyTestEventListener>();
        listeners.Add(value);
        entity.ReplaceAnyTestEventListener(listeners);
    }

    public static void RemoveAnyTestEventListener(this GameEntity entity, IAnyTestEventListener value, bool removeComponentWhenEmpty = true)
    {
        var listeners = entity.GetAnyTestEventListener().value;
        listeners.Remove(value);
        if (removeComponentWhenEmpty && listeners.Count == 0)
        {
            entity.RemoveAnyTestEventListener();
        }
        else
        {
            entity.ReplaceAnyTestEventListener(listeners);
        }
    }
}


// GameAnyTestEventRemovedListenerComponent.g.cs
public static class GameAnyTestEventRemovedListenerEntityExtensions
{
    public static AnyTestEventRemovedListenerComponent GetAnyTestEventRemovedListener(this GameEntity entity) { return (AnyTestEventRemovedListenerComponent)entity.GetComponent(GameComponentsLookup.AnyTestEventRemovedListener); }
    public static bool HasAnyTestEventRemovedListener(this GameEntity entity) { return entity.HasComponent(GameComponentsLookup.AnyTestEventRemovedListener); }

    public static void AddAnyTestEventRemovedListener(this GameEntity entity, System.Collections.Generic.List<IAnyTestEventRemovedListener> newValue)
    {
        var index = GameComponentsLookup.AnyTestEventRemovedListener;
        var component = (AnyTestEventRemovedListenerComponent)entity.CreateComponent(index, typeof(AnyTestEventRemovedListenerComponent));
        component.value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceAnyTestEventRemovedListener(this GameEntity entity, System.Collections.Generic.List<IAnyTestEventRemovedListener> newValue)
    {
        var index = GameComponentsLookup.AnyTestEventRemovedListener;
        var component = (AnyTestEventRemovedListenerComponent)entity.CreateComponent(index, typeof(AnyTestEventRemovedListenerComponent));
        component.value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveAnyTestEventRemovedListener(this GameEntity entity)
    {
        entity.RemoveComponent(GameComponentsLookup.AnyTestEventRemovedListener);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherAnyTestEventRemovedListener;

    public static Entitas.IMatcher<GameEntity> AnyTestEventRemovedListener()
    {
        if (_matcherAnyTestEventRemovedListener == null)
        {
            var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.AnyTestEventRemovedListener);
            matcher.componentNames = GameComponentsLookup.componentNames;
            _matcherAnyTestEventRemovedListener = matcher;
        }

        return _matcherAnyTestEventRemovedListener;
    }
}


// GameAnyTestEventRemovedListenerComponentEvent.g.cs
public static class GameAnyTestEventRemovedListenerComponentEventExtensions
{
    public static void AddAnyTestEventRemovedListener(this GameEntity entity, IAnyTestEventRemovedListener value)
    {
        var listeners = entity.HasAnyTestEventRemovedListener()
            ? entity.GetAnyTestEventRemovedListener().value
            : new System.Collections.Generic.List<IAnyTestEventRemovedListener>();
        listeners.Add(value);
        entity.ReplaceAnyTestEventRemovedListener(listeners);
    }

    public static void RemoveAnyTestEventRemovedListener(this GameEntity entity, IAnyTestEventRemovedListener value, bool removeComponentWhenEmpty = true)
    {
        var listeners = entity.GetAnyTestEventRemovedListener().value;
        listeners.Remove(value);
        if (removeComponentWhenEmpty && listeners.Count == 0)
        {
            entity.RemoveAnyTestEventRemovedListener();
        }
        else
        {
            entity.ReplaceAnyTestEventRemovedListener(listeners);
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
    public const int TestEvent = 0;
    public const int NamespaceTestEvent2 = 1;
    public const int NamespaceTestEvent3 = 2;
    public const int AnyTestEventListener = 3;
    public const int AnyTestEventRemovedListener = 4;
    public const int TestEventListener = 5;
    public const int TestEventRemovedListener = 6;
    public const int NamespaceAnyTestEvent2Listener = 7;
    public const int NamespaceAnyTestEvent2RemovedListener = 8;
    public const int GameNamespaceAnyTestEvent3Listener = 9;
    public const int GameNamespaceAnyTestEvent3RemovedListener = 10;

    public const int TotalComponents = 11;

    public static readonly string[] componentNames = 
    {
        "TestEvent",
        "NamespaceTestEvent2",
        "NamespaceTestEvent3",
        "AnyTestEventListener",
        "AnyTestEventRemovedListener",
        "TestEventListener",
        "TestEventRemovedListener",
        "NamespaceAnyTestEvent2Listener",
        "NamespaceAnyTestEvent2RemovedListener",
        "GameNamespaceAnyTestEvent3Listener",
        "GameNamespaceAnyTestEvent3RemovedListener"
    };

    public static readonly System.Type[] componentTypes = 
    {
        typeof(TestEventComponent),
        typeof(Namespace.TestEvent2Component),
        typeof(Namespace.TestEvent3Component),
        typeof(AnyTestEventListenerComponent),
        typeof(AnyTestEventRemovedListenerComponent),
        typeof(TestEventListenerComponent),
        typeof(TestEventRemovedListenerComponent),
        typeof(NamespaceAnyTestEvent2ListenerComponent),
        typeof(NamespaceAnyTestEvent2RemovedListenerComponent),
        typeof(GameNamespaceAnyTestEvent3ListenerComponent),
        typeof(GameNamespaceAnyTestEvent3RemovedListenerComponent)
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
        Add(new NamespaceAnyTestEvent2EventSystem(contexts)); // priority: 0
        Add(new NamespaceAnyTestEvent2RemovedEventSystem(contexts)); // priority: 0
        Add(new GameNamespaceAnyTestEvent3EventSystem(contexts)); // priority: 0
        Add(new GameNamespaceAnyTestEvent3RemovedEventSystem(contexts)); // priority: 0
        Add(new AnyTestEventEventSystem(contexts)); // priority: 0
        Add(new AnyTestEventRemovedEventSystem(contexts)); // priority: 0
        Add(new TestEventEventSystem(contexts)); // priority: 0
        Add(new TestEventRemovedEventSystem(contexts)); // priority: 0
    }
}


// GameGameNamespaceAnyTestEvent3ListenerComponent.g.cs
public static class GameGameNamespaceAnyTestEvent3ListenerEntityExtensions
{
    public static GameNamespaceAnyTestEvent3ListenerComponent GetGameNamespaceAnyTestEvent3Listener(this GameEntity entity) { return (GameNamespaceAnyTestEvent3ListenerComponent)entity.GetComponent(GameComponentsLookup.GameNamespaceAnyTestEvent3Listener); }
    public static bool HasGameNamespaceAnyTestEvent3Listener(this GameEntity entity) { return entity.HasComponent(GameComponentsLookup.GameNamespaceAnyTestEvent3Listener); }

    public static void AddGameNamespaceAnyTestEvent3Listener(this GameEntity entity, System.Collections.Generic.List<IGameNamespaceAnyTestEvent3Listener> newValue)
    {
        var index = GameComponentsLookup.GameNamespaceAnyTestEvent3Listener;
        var component = (GameNamespaceAnyTestEvent3ListenerComponent)entity.CreateComponent(index, typeof(GameNamespaceAnyTestEvent3ListenerComponent));
        component.value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceGameNamespaceAnyTestEvent3Listener(this GameEntity entity, System.Collections.Generic.List<IGameNamespaceAnyTestEvent3Listener> newValue)
    {
        var index = GameComponentsLookup.GameNamespaceAnyTestEvent3Listener;
        var component = (GameNamespaceAnyTestEvent3ListenerComponent)entity.CreateComponent(index, typeof(GameNamespaceAnyTestEvent3ListenerComponent));
        component.value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveGameNamespaceAnyTestEvent3Listener(this GameEntity entity)
    {
        entity.RemoveComponent(GameComponentsLookup.GameNamespaceAnyTestEvent3Listener);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherGameNamespaceAnyTestEvent3Listener;

    public static Entitas.IMatcher<GameEntity> GameNamespaceAnyTestEvent3Listener()
    {
        if (_matcherGameNamespaceAnyTestEvent3Listener == null)
        {
            var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.GameNamespaceAnyTestEvent3Listener);
            matcher.componentNames = GameComponentsLookup.componentNames;
            _matcherGameNamespaceAnyTestEvent3Listener = matcher;
        }

        return _matcherGameNamespaceAnyTestEvent3Listener;
    }
}


// GameGameNamespaceAnyTestEvent3ListenerComponentEvent.g.cs
public static class GameGameNamespaceAnyTestEvent3ListenerComponentEventExtensions
{
    public static void AddGameNamespaceAnyTestEvent3Listener(this GameEntity entity, IGameNamespaceAnyTestEvent3Listener value)
    {
        var listeners = entity.HasGameNamespaceAnyTestEvent3Listener()
            ? entity.GetGameNamespaceAnyTestEvent3Listener().value
            : new System.Collections.Generic.List<IGameNamespaceAnyTestEvent3Listener>();
        listeners.Add(value);
        entity.ReplaceGameNamespaceAnyTestEvent3Listener(listeners);
    }

    public static void RemoveGameNamespaceAnyTestEvent3Listener(this GameEntity entity, IGameNamespaceAnyTestEvent3Listener value, bool removeComponentWhenEmpty = true)
    {
        var listeners = entity.GetGameNamespaceAnyTestEvent3Listener().value;
        listeners.Remove(value);
        if (removeComponentWhenEmpty && listeners.Count == 0)
        {
            entity.RemoveGameNamespaceAnyTestEvent3Listener();
        }
        else
        {
            entity.ReplaceGameNamespaceAnyTestEvent3Listener(listeners);
        }
    }
}


// GameGameNamespaceAnyTestEvent3RemovedListenerComponent.g.cs
public static class GameGameNamespaceAnyTestEvent3RemovedListenerEntityExtensions
{
    public static GameNamespaceAnyTestEvent3RemovedListenerComponent GetGameNamespaceAnyTestEvent3RemovedListener(this GameEntity entity) { return (GameNamespaceAnyTestEvent3RemovedListenerComponent)entity.GetComponent(GameComponentsLookup.GameNamespaceAnyTestEvent3RemovedListener); }
    public static bool HasGameNamespaceAnyTestEvent3RemovedListener(this GameEntity entity) { return entity.HasComponent(GameComponentsLookup.GameNamespaceAnyTestEvent3RemovedListener); }

    public static void AddGameNamespaceAnyTestEvent3RemovedListener(this GameEntity entity, System.Collections.Generic.List<IGameNamespaceAnyTestEvent3RemovedListener> newValue)
    {
        var index = GameComponentsLookup.GameNamespaceAnyTestEvent3RemovedListener;
        var component = (GameNamespaceAnyTestEvent3RemovedListenerComponent)entity.CreateComponent(index, typeof(GameNamespaceAnyTestEvent3RemovedListenerComponent));
        component.value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceGameNamespaceAnyTestEvent3RemovedListener(this GameEntity entity, System.Collections.Generic.List<IGameNamespaceAnyTestEvent3RemovedListener> newValue)
    {
        var index = GameComponentsLookup.GameNamespaceAnyTestEvent3RemovedListener;
        var component = (GameNamespaceAnyTestEvent3RemovedListenerComponent)entity.CreateComponent(index, typeof(GameNamespaceAnyTestEvent3RemovedListenerComponent));
        component.value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveGameNamespaceAnyTestEvent3RemovedListener(this GameEntity entity)
    {
        entity.RemoveComponent(GameComponentsLookup.GameNamespaceAnyTestEvent3RemovedListener);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherGameNamespaceAnyTestEvent3RemovedListener;

    public static Entitas.IMatcher<GameEntity> GameNamespaceAnyTestEvent3RemovedListener()
    {
        if (_matcherGameNamespaceAnyTestEvent3RemovedListener == null)
        {
            var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.GameNamespaceAnyTestEvent3RemovedListener);
            matcher.componentNames = GameComponentsLookup.componentNames;
            _matcherGameNamespaceAnyTestEvent3RemovedListener = matcher;
        }

        return _matcherGameNamespaceAnyTestEvent3RemovedListener;
    }
}


// GameGameNamespaceAnyTestEvent3RemovedListenerComponentEvent.g.cs
public static class GameGameNamespaceAnyTestEvent3RemovedListenerComponentEventExtensions
{
    public static void AddGameNamespaceAnyTestEvent3RemovedListener(this GameEntity entity, IGameNamespaceAnyTestEvent3RemovedListener value)
    {
        var listeners = entity.HasGameNamespaceAnyTestEvent3RemovedListener()
            ? entity.GetGameNamespaceAnyTestEvent3RemovedListener().value
            : new System.Collections.Generic.List<IGameNamespaceAnyTestEvent3RemovedListener>();
        listeners.Add(value);
        entity.ReplaceGameNamespaceAnyTestEvent3RemovedListener(listeners);
    }

    public static void RemoveGameNamespaceAnyTestEvent3RemovedListener(this GameEntity entity, IGameNamespaceAnyTestEvent3RemovedListener value, bool removeComponentWhenEmpty = true)
    {
        var listeners = entity.GetGameNamespaceAnyTestEvent3RemovedListener().value;
        listeners.Remove(value);
        if (removeComponentWhenEmpty && listeners.Count == 0)
        {
            entity.RemoveGameNamespaceAnyTestEvent3RemovedListener();
        }
        else
        {
            entity.ReplaceGameNamespaceAnyTestEvent3RemovedListener(listeners);
        }
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


// GameNamespaceAnyTestEvent2ListenerComponent.g.cs
public static class GameNamespaceAnyTestEvent2ListenerEntityExtensions
{
    public static NamespaceAnyTestEvent2ListenerComponent GetNamespaceAnyTestEvent2Listener(this GameEntity entity) { return (NamespaceAnyTestEvent2ListenerComponent)entity.GetComponent(GameComponentsLookup.NamespaceAnyTestEvent2Listener); }
    public static bool HasNamespaceAnyTestEvent2Listener(this GameEntity entity) { return entity.HasComponent(GameComponentsLookup.NamespaceAnyTestEvent2Listener); }

    public static void AddNamespaceAnyTestEvent2Listener(this GameEntity entity, System.Collections.Generic.List<INamespaceAnyTestEvent2Listener> newValue)
    {
        var index = GameComponentsLookup.NamespaceAnyTestEvent2Listener;
        var component = (NamespaceAnyTestEvent2ListenerComponent)entity.CreateComponent(index, typeof(NamespaceAnyTestEvent2ListenerComponent));
        component.value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceNamespaceAnyTestEvent2Listener(this GameEntity entity, System.Collections.Generic.List<INamespaceAnyTestEvent2Listener> newValue)
    {
        var index = GameComponentsLookup.NamespaceAnyTestEvent2Listener;
        var component = (NamespaceAnyTestEvent2ListenerComponent)entity.CreateComponent(index, typeof(NamespaceAnyTestEvent2ListenerComponent));
        component.value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveNamespaceAnyTestEvent2Listener(this GameEntity entity)
    {
        entity.RemoveComponent(GameComponentsLookup.NamespaceAnyTestEvent2Listener);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherNamespaceAnyTestEvent2Listener;

    public static Entitas.IMatcher<GameEntity> NamespaceAnyTestEvent2Listener()
    {
        if (_matcherNamespaceAnyTestEvent2Listener == null)
        {
            var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.NamespaceAnyTestEvent2Listener);
            matcher.componentNames = GameComponentsLookup.componentNames;
            _matcherNamespaceAnyTestEvent2Listener = matcher;
        }

        return _matcherNamespaceAnyTestEvent2Listener;
    }
}


// GameNamespaceAnyTestEvent2ListenerComponentEvent.g.cs
public static class GameNamespaceAnyTestEvent2ListenerComponentEventExtensions
{
    public static void AddNamespaceAnyTestEvent2Listener(this GameEntity entity, INamespaceAnyTestEvent2Listener value)
    {
        var listeners = entity.HasNamespaceAnyTestEvent2Listener()
            ? entity.GetNamespaceAnyTestEvent2Listener().value
            : new System.Collections.Generic.List<INamespaceAnyTestEvent2Listener>();
        listeners.Add(value);
        entity.ReplaceNamespaceAnyTestEvent2Listener(listeners);
    }

    public static void RemoveNamespaceAnyTestEvent2Listener(this GameEntity entity, INamespaceAnyTestEvent2Listener value, bool removeComponentWhenEmpty = true)
    {
        var listeners = entity.GetNamespaceAnyTestEvent2Listener().value;
        listeners.Remove(value);
        if (removeComponentWhenEmpty && listeners.Count == 0)
        {
            entity.RemoveNamespaceAnyTestEvent2Listener();
        }
        else
        {
            entity.ReplaceNamespaceAnyTestEvent2Listener(listeners);
        }
    }
}


// GameNamespaceAnyTestEvent2RemovedListenerComponent.g.cs
public static class GameNamespaceAnyTestEvent2RemovedListenerEntityExtensions
{
    public static NamespaceAnyTestEvent2RemovedListenerComponent GetNamespaceAnyTestEvent2RemovedListener(this GameEntity entity) { return (NamespaceAnyTestEvent2RemovedListenerComponent)entity.GetComponent(GameComponentsLookup.NamespaceAnyTestEvent2RemovedListener); }
    public static bool HasNamespaceAnyTestEvent2RemovedListener(this GameEntity entity) { return entity.HasComponent(GameComponentsLookup.NamespaceAnyTestEvent2RemovedListener); }

    public static void AddNamespaceAnyTestEvent2RemovedListener(this GameEntity entity, System.Collections.Generic.List<INamespaceAnyTestEvent2RemovedListener> newValue)
    {
        var index = GameComponentsLookup.NamespaceAnyTestEvent2RemovedListener;
        var component = (NamespaceAnyTestEvent2RemovedListenerComponent)entity.CreateComponent(index, typeof(NamespaceAnyTestEvent2RemovedListenerComponent));
        component.value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceNamespaceAnyTestEvent2RemovedListener(this GameEntity entity, System.Collections.Generic.List<INamespaceAnyTestEvent2RemovedListener> newValue)
    {
        var index = GameComponentsLookup.NamespaceAnyTestEvent2RemovedListener;
        var component = (NamespaceAnyTestEvent2RemovedListenerComponent)entity.CreateComponent(index, typeof(NamespaceAnyTestEvent2RemovedListenerComponent));
        component.value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveNamespaceAnyTestEvent2RemovedListener(this GameEntity entity)
    {
        entity.RemoveComponent(GameComponentsLookup.NamespaceAnyTestEvent2RemovedListener);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherNamespaceAnyTestEvent2RemovedListener;

    public static Entitas.IMatcher<GameEntity> NamespaceAnyTestEvent2RemovedListener()
    {
        if (_matcherNamespaceAnyTestEvent2RemovedListener == null)
        {
            var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.NamespaceAnyTestEvent2RemovedListener);
            matcher.componentNames = GameComponentsLookup.componentNames;
            _matcherNamespaceAnyTestEvent2RemovedListener = matcher;
        }

        return _matcherNamespaceAnyTestEvent2RemovedListener;
    }
}


// GameNamespaceAnyTestEvent2RemovedListenerComponentEvent.g.cs
public static class GameNamespaceAnyTestEvent2RemovedListenerComponentEventExtensions
{
    public static void AddNamespaceAnyTestEvent2RemovedListener(this GameEntity entity, INamespaceAnyTestEvent2RemovedListener value)
    {
        var listeners = entity.HasNamespaceAnyTestEvent2RemovedListener()
            ? entity.GetNamespaceAnyTestEvent2RemovedListener().value
            : new System.Collections.Generic.List<INamespaceAnyTestEvent2RemovedListener>();
        listeners.Add(value);
        entity.ReplaceNamespaceAnyTestEvent2RemovedListener(listeners);
    }

    public static void RemoveNamespaceAnyTestEvent2RemovedListener(this GameEntity entity, INamespaceAnyTestEvent2RemovedListener value, bool removeComponentWhenEmpty = true)
    {
        var listeners = entity.GetNamespaceAnyTestEvent2RemovedListener().value;
        listeners.Remove(value);
        if (removeComponentWhenEmpty && listeners.Count == 0)
        {
            entity.RemoveNamespaceAnyTestEvent2RemovedListener();
        }
        else
        {
            entity.ReplaceNamespaceAnyTestEvent2RemovedListener(listeners);
        }
    }
}


// GameNamespaceAnyTestEvent3EventSystem.g.cs
public sealed class GameNamespaceAnyTestEvent3EventSystem : Entitas.ReactiveSystem<GameEntity>
{
    readonly Entitas.IGroup<GameEntity> _listeners;
    readonly System.Collections.Generic.List<GameEntity> _entityBuffer;
    readonly System.Collections.Generic.List<IGameNamespaceAnyTestEvent3Listener> _listenerBuffer;

    public GameNamespaceAnyTestEvent3EventSystem(Contexts contexts) : base(contexts.game)
    {
        _listeners = contexts.game.GetGroup(GameMatcher.GameNamespaceAnyTestEvent3Listener());
        _entityBuffer = new System.Collections.Generic.List<GameEntity>();
        _listenerBuffer = new System.Collections.Generic.List<IGameNamespaceAnyTestEvent3Listener>();
    }

    protected override Entitas.ICollector<GameEntity> GetTrigger(Entitas.IContext<GameEntity> context)
    {
        return Entitas.CollectorContextExtension.CreateCollector(
            context, Entitas.TriggerOnEventMatcherExtension.Added(GameMatcher.NamespaceTestEvent3())
        );
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.HasNamespaceTestEvent3();
    }

    protected override void Execute(System.Collections.Generic.List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            var component = e.GetNamespaceTestEvent3();
            foreach (var listenerEntity in _listeners.GetEntities(_entityBuffer))
            {
                _listenerBuffer.Clear();
                _listenerBuffer.AddRange(listenerEntity.GetGameNamespaceAnyTestEvent3Listener().value);
                foreach (var listener in _listenerBuffer)
                {
                    listener.OnNamespaceAnyTestEvent3(e, component.Value);
                }
            }
        }
    }
}


// GameNamespaceAnyTestEvent3ListenerComponent.g.cs
[Entitas.CodeGeneration.Attributes.DontGenerate]
public sealed class GameNamespaceAnyTestEvent3ListenerComponent : Entitas.IComponent
{
    public System.Collections.Generic.List<IGameNamespaceAnyTestEvent3Listener> value;
}


// GameNamespaceAnyTestEvent3RemovedEventSystem.g.cs
public sealed class GameNamespaceAnyTestEvent3RemovedEventSystem : Entitas.ReactiveSystem<GameEntity>
{
    readonly Entitas.IGroup<GameEntity> _listeners;
    readonly System.Collections.Generic.List<GameEntity> _entityBuffer;
    readonly System.Collections.Generic.List<IGameNamespaceAnyTestEvent3RemovedListener> _listenerBuffer;

    public GameNamespaceAnyTestEvent3RemovedEventSystem(Contexts contexts) : base(contexts.game)
    {
        _listeners = contexts.game.GetGroup(GameMatcher.GameNamespaceAnyTestEvent3RemovedListener());
        _entityBuffer = new System.Collections.Generic.List<GameEntity>();
        _listenerBuffer = new System.Collections.Generic.List<IGameNamespaceAnyTestEvent3RemovedListener>();
    }

    protected override Entitas.ICollector<GameEntity> GetTrigger(Entitas.IContext<GameEntity> context)
    {
        return Entitas.CollectorContextExtension.CreateCollector(
            context, Entitas.TriggerOnEventMatcherExtension.Removed(GameMatcher.NamespaceTestEvent3())
        );
    }

    protected override bool Filter(GameEntity entity)
    {
        return !entity.HasNamespaceTestEvent3();
    }

    protected override void Execute(System.Collections.Generic.List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            
            foreach (var listenerEntity in _listeners.GetEntities(_entityBuffer))
            {
                _listenerBuffer.Clear();
                _listenerBuffer.AddRange(listenerEntity.GetGameNamespaceAnyTestEvent3RemovedListener().value);
                foreach (var listener in _listenerBuffer)
                {
                    listener.OnNamespaceAnyTestEvent3Removed(e);
                }
            }
        }
    }
}


// GameNamespaceAnyTestEvent3RemovedListenerComponent.g.cs
[Entitas.CodeGeneration.Attributes.DontGenerate]
public sealed class GameNamespaceAnyTestEvent3RemovedListenerComponent : Entitas.IComponent
{
    public System.Collections.Generic.List<IGameNamespaceAnyTestEvent3RemovedListener> value;
}


// GameNamespaceTestEvent2Component.g.cs
public static class GameNamespaceTestEvent2EntityExtensions
{
    public static Namespace.TestEvent2Component GetNamespaceTestEvent2(this GameEntity entity) { return (Namespace.TestEvent2Component)entity.GetComponent(GameComponentsLookup.NamespaceTestEvent2); }
    public static bool HasNamespaceTestEvent2(this GameEntity entity) { return entity.HasComponent(GameComponentsLookup.NamespaceTestEvent2); }

    public static void AddNamespaceTestEvent2(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.NamespaceTestEvent2;
        var component = (Namespace.TestEvent2Component)entity.CreateComponent(index, typeof(Namespace.TestEvent2Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceNamespaceTestEvent2(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.NamespaceTestEvent2;
        var component = (Namespace.TestEvent2Component)entity.CreateComponent(index, typeof(Namespace.TestEvent2Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveNamespaceTestEvent2(this GameEntity entity)
    {
        entity.RemoveComponent(GameComponentsLookup.NamespaceTestEvent2);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherNamespaceTestEvent2;

    public static Entitas.IMatcher<GameEntity> NamespaceTestEvent2()
    {
        if (_matcherNamespaceTestEvent2 == null)
        {
            var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.NamespaceTestEvent2);
            matcher.componentNames = GameComponentsLookup.componentNames;
            _matcherNamespaceTestEvent2 = matcher;
        }

        return _matcherNamespaceTestEvent2;
    }
}


// GameNamespaceTestEvent3Component.g.cs
public static class GameNamespaceTestEvent3EntityExtensions
{
    public static Namespace.TestEvent3Component GetNamespaceTestEvent3(this GameEntity entity) { return (Namespace.TestEvent3Component)entity.GetComponent(GameComponentsLookup.NamespaceTestEvent3); }
    public static bool HasNamespaceTestEvent3(this GameEntity entity) { return entity.HasComponent(GameComponentsLookup.NamespaceTestEvent3); }

    public static void AddNamespaceTestEvent3(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.NamespaceTestEvent3;
        var component = (Namespace.TestEvent3Component)entity.CreateComponent(index, typeof(Namespace.TestEvent3Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceNamespaceTestEvent3(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.NamespaceTestEvent3;
        var component = (Namespace.TestEvent3Component)entity.CreateComponent(index, typeof(Namespace.TestEvent3Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveNamespaceTestEvent3(this GameEntity entity)
    {
        entity.RemoveComponent(GameComponentsLookup.NamespaceTestEvent3);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherNamespaceTestEvent3;

    public static Entitas.IMatcher<GameEntity> NamespaceTestEvent3()
    {
        if (_matcherNamespaceTestEvent3 == null)
        {
            var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.NamespaceTestEvent3);
            matcher.componentNames = GameComponentsLookup.componentNames;
            _matcherNamespaceTestEvent3 = matcher;
        }

        return _matcherNamespaceTestEvent3;
    }
}


// GameTestEventComponent.g.cs
public static class GameTestEventEntityExtensions
{
    public static TestEventComponent GetTestEvent(this GameEntity entity) { return (TestEventComponent)entity.GetComponent(GameComponentsLookup.TestEvent); }
    public static bool HasTestEvent(this GameEntity entity) { return entity.HasComponent(GameComponentsLookup.TestEvent); }

    public static void AddTestEvent(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.TestEvent;
        var component = (TestEventComponent)entity.CreateComponent(index, typeof(TestEventComponent));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestEvent(this GameEntity entity, string newValue)
    {
        var index = GameComponentsLookup.TestEvent;
        var component = (TestEventComponent)entity.CreateComponent(index, typeof(TestEventComponent));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestEvent(this GameEntity entity)
    {
        entity.RemoveComponent(GameComponentsLookup.TestEvent);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherTestEvent;

    public static Entitas.IMatcher<GameEntity> TestEvent()
    {
        if (_matcherTestEvent == null)
        {
            var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.TestEvent);
            matcher.componentNames = GameComponentsLookup.componentNames;
            _matcherTestEvent = matcher;
        }

        return _matcherTestEvent;
    }
}


// GameTestEventListenerComponent.g.cs
public static class GameTestEventListenerEntityExtensions
{
    public static TestEventListenerComponent GetTestEventListener(this GameEntity entity) { return (TestEventListenerComponent)entity.GetComponent(GameComponentsLookup.TestEventListener); }
    public static bool HasTestEventListener(this GameEntity entity) { return entity.HasComponent(GameComponentsLookup.TestEventListener); }

    public static void AddTestEventListener(this GameEntity entity, System.Collections.Generic.List<ITestEventListener> newValue)
    {
        var index = GameComponentsLookup.TestEventListener;
        var component = (TestEventListenerComponent)entity.CreateComponent(index, typeof(TestEventListenerComponent));
        component.value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestEventListener(this GameEntity entity, System.Collections.Generic.List<ITestEventListener> newValue)
    {
        var index = GameComponentsLookup.TestEventListener;
        var component = (TestEventListenerComponent)entity.CreateComponent(index, typeof(TestEventListenerComponent));
        component.value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestEventListener(this GameEntity entity)
    {
        entity.RemoveComponent(GameComponentsLookup.TestEventListener);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherTestEventListener;

    public static Entitas.IMatcher<GameEntity> TestEventListener()
    {
        if (_matcherTestEventListener == null)
        {
            var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.TestEventListener);
            matcher.componentNames = GameComponentsLookup.componentNames;
            _matcherTestEventListener = matcher;
        }

        return _matcherTestEventListener;
    }
}


// GameTestEventListenerComponentEvent.g.cs
public static class GameTestEventListenerComponentEventExtensions
{
    public static void AddTestEventListener(this GameEntity entity, ITestEventListener value)
    {
        var listeners = entity.HasTestEventListener()
            ? entity.GetTestEventListener().value
            : new System.Collections.Generic.List<ITestEventListener>();
        listeners.Add(value);
        entity.ReplaceTestEventListener(listeners);
    }

    public static void RemoveTestEventListener(this GameEntity entity, ITestEventListener value, bool removeComponentWhenEmpty = true)
    {
        var listeners = entity.GetTestEventListener().value;
        listeners.Remove(value);
        if (removeComponentWhenEmpty && listeners.Count == 0)
        {
            entity.RemoveTestEventListener();
        }
        else
        {
            entity.ReplaceTestEventListener(listeners);
        }
    }
}


// GameTestEventRemovedListenerComponent.g.cs
public static class GameTestEventRemovedListenerEntityExtensions
{
    public static TestEventRemovedListenerComponent GetTestEventRemovedListener(this GameEntity entity) { return (TestEventRemovedListenerComponent)entity.GetComponent(GameComponentsLookup.TestEventRemovedListener); }
    public static bool HasTestEventRemovedListener(this GameEntity entity) { return entity.HasComponent(GameComponentsLookup.TestEventRemovedListener); }

    public static void AddTestEventRemovedListener(this GameEntity entity, System.Collections.Generic.List<ITestEventRemovedListener> newValue)
    {
        var index = GameComponentsLookup.TestEventRemovedListener;
        var component = (TestEventRemovedListenerComponent)entity.CreateComponent(index, typeof(TestEventRemovedListenerComponent));
        component.value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceTestEventRemovedListener(this GameEntity entity, System.Collections.Generic.List<ITestEventRemovedListener> newValue)
    {
        var index = GameComponentsLookup.TestEventRemovedListener;
        var component = (TestEventRemovedListenerComponent)entity.CreateComponent(index, typeof(TestEventRemovedListenerComponent));
        component.value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveTestEventRemovedListener(this GameEntity entity)
    {
        entity.RemoveComponent(GameComponentsLookup.TestEventRemovedListener);
    }
}

public sealed partial class GameMatcher
{
    static Entitas.IMatcher<GameEntity> _matcherTestEventRemovedListener;

    public static Entitas.IMatcher<GameEntity> TestEventRemovedListener()
    {
        if (_matcherTestEventRemovedListener == null)
        {
            var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.TestEventRemovedListener);
            matcher.componentNames = GameComponentsLookup.componentNames;
            _matcherTestEventRemovedListener = matcher;
        }

        return _matcherTestEventRemovedListener;
    }
}


// GameTestEventRemovedListenerComponentEvent.g.cs
public static class GameTestEventRemovedListenerComponentEventExtensions
{
    public static void AddTestEventRemovedListener(this GameEntity entity, ITestEventRemovedListener value)
    {
        var listeners = entity.HasTestEventRemovedListener()
            ? entity.GetTestEventRemovedListener().value
            : new System.Collections.Generic.List<ITestEventRemovedListener>();
        listeners.Add(value);
        entity.ReplaceTestEventRemovedListener(listeners);
    }

    public static void RemoveTestEventRemovedListener(this GameEntity entity, ITestEventRemovedListener value, bool removeComponentWhenEmpty = true)
    {
        var listeners = entity.GetTestEventRemovedListener().value;
        listeners.Remove(value);
        if (removeComponentWhenEmpty && listeners.Count == 0)
        {
            entity.RemoveTestEventRemovedListener();
        }
        else
        {
            entity.ReplaceTestEventRemovedListener(listeners);
        }
    }
}


// IAnyTestEventListener.g.cs
public interface IAnyTestEventListener
{
    void OnAnyTestEvent(GameEntity entity, string value);
}


// IAnyTestEventRemovedListener.g.cs
public interface IAnyTestEventRemovedListener
{
    void OnAnyTestEventRemoved(GameEntity entity);
}


// IGameNamespaceAnyTestEvent3Listener.g.cs
public interface IGameNamespaceAnyTestEvent3Listener
{
    void OnNamespaceAnyTestEvent3(GameEntity entity, string value);
}


// IGameNamespaceAnyTestEvent3RemovedListener.g.cs
public interface IGameNamespaceAnyTestEvent3RemovedListener
{
    void OnNamespaceAnyTestEvent3Removed(GameEntity entity);
}


// IInputNamespaceAnyTestEvent3Listener.g.cs
public interface IInputNamespaceAnyTestEvent3Listener
{
    void OnNamespaceAnyTestEvent3(InputEntity entity, string value);
}


// IInputNamespaceAnyTestEvent3RemovedListener.g.cs
public interface IInputNamespaceAnyTestEvent3RemovedListener
{
    void OnNamespaceAnyTestEvent3Removed(InputEntity entity);
}


// INamespaceAnyTestEvent2Listener.g.cs
public interface INamespaceAnyTestEvent2Listener
{
    void OnNamespaceAnyTestEvent2(GameEntity entity, string value);
}


// INamespaceAnyTestEvent2RemovedListener.g.cs
public interface INamespaceAnyTestEvent2RemovedListener
{
    void OnNamespaceAnyTestEvent2Removed(GameEntity entity);
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
    public const int NamespaceTestEvent3 = 0;
    public const int InputNamespaceAnyTestEvent3Listener = 1;
    public const int InputNamespaceAnyTestEvent3RemovedListener = 2;

    public const int TotalComponents = 3;

    public static readonly string[] componentNames = 
    {
        "NamespaceTestEvent3",
        "InputNamespaceAnyTestEvent3Listener",
        "InputNamespaceAnyTestEvent3RemovedListener"
    };

    public static readonly System.Type[] componentTypes = 
    {
        typeof(Namespace.TestEvent3Component),
        typeof(InputNamespaceAnyTestEvent3ListenerComponent),
        typeof(InputNamespaceAnyTestEvent3RemovedListenerComponent)
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
        Add(new InputNamespaceAnyTestEvent3EventSystem(contexts)); // priority: 0
        Add(new InputNamespaceAnyTestEvent3RemovedEventSystem(contexts)); // priority: 0
    }
}


// InputInputNamespaceAnyTestEvent3ListenerComponent.g.cs
public static class InputInputNamespaceAnyTestEvent3ListenerEntityExtensions
{
    public static InputNamespaceAnyTestEvent3ListenerComponent GetInputNamespaceAnyTestEvent3Listener(this InputEntity entity) { return (InputNamespaceAnyTestEvent3ListenerComponent)entity.GetComponent(InputComponentsLookup.InputNamespaceAnyTestEvent3Listener); }
    public static bool HasInputNamespaceAnyTestEvent3Listener(this InputEntity entity) { return entity.HasComponent(InputComponentsLookup.InputNamespaceAnyTestEvent3Listener); }

    public static void AddInputNamespaceAnyTestEvent3Listener(this InputEntity entity, System.Collections.Generic.List<IInputNamespaceAnyTestEvent3Listener> newValue)
    {
        var index = InputComponentsLookup.InputNamespaceAnyTestEvent3Listener;
        var component = (InputNamespaceAnyTestEvent3ListenerComponent)entity.CreateComponent(index, typeof(InputNamespaceAnyTestEvent3ListenerComponent));
        component.value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceInputNamespaceAnyTestEvent3Listener(this InputEntity entity, System.Collections.Generic.List<IInputNamespaceAnyTestEvent3Listener> newValue)
    {
        var index = InputComponentsLookup.InputNamespaceAnyTestEvent3Listener;
        var component = (InputNamespaceAnyTestEvent3ListenerComponent)entity.CreateComponent(index, typeof(InputNamespaceAnyTestEvent3ListenerComponent));
        component.value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveInputNamespaceAnyTestEvent3Listener(this InputEntity entity)
    {
        entity.RemoveComponent(InputComponentsLookup.InputNamespaceAnyTestEvent3Listener);
    }
}

public sealed partial class InputMatcher
{
    static Entitas.IMatcher<InputEntity> _matcherInputNamespaceAnyTestEvent3Listener;

    public static Entitas.IMatcher<InputEntity> InputNamespaceAnyTestEvent3Listener()
    {
        if (_matcherInputNamespaceAnyTestEvent3Listener == null)
        {
            var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.InputNamespaceAnyTestEvent3Listener);
            matcher.componentNames = InputComponentsLookup.componentNames;
            _matcherInputNamespaceAnyTestEvent3Listener = matcher;
        }

        return _matcherInputNamespaceAnyTestEvent3Listener;
    }
}


// InputInputNamespaceAnyTestEvent3ListenerComponentEvent.g.cs
public static class InputInputNamespaceAnyTestEvent3ListenerComponentEventExtensions
{
    public static void AddInputNamespaceAnyTestEvent3Listener(this InputEntity entity, IInputNamespaceAnyTestEvent3Listener value)
    {
        var listeners = entity.HasInputNamespaceAnyTestEvent3Listener()
            ? entity.GetInputNamespaceAnyTestEvent3Listener().value
            : new System.Collections.Generic.List<IInputNamespaceAnyTestEvent3Listener>();
        listeners.Add(value);
        entity.ReplaceInputNamespaceAnyTestEvent3Listener(listeners);
    }

    public static void RemoveInputNamespaceAnyTestEvent3Listener(this InputEntity entity, IInputNamespaceAnyTestEvent3Listener value, bool removeComponentWhenEmpty = true)
    {
        var listeners = entity.GetInputNamespaceAnyTestEvent3Listener().value;
        listeners.Remove(value);
        if (removeComponentWhenEmpty && listeners.Count == 0)
        {
            entity.RemoveInputNamespaceAnyTestEvent3Listener();
        }
        else
        {
            entity.ReplaceInputNamespaceAnyTestEvent3Listener(listeners);
        }
    }
}


// InputInputNamespaceAnyTestEvent3RemovedListenerComponent.g.cs
public static class InputInputNamespaceAnyTestEvent3RemovedListenerEntityExtensions
{
    public static InputNamespaceAnyTestEvent3RemovedListenerComponent GetInputNamespaceAnyTestEvent3RemovedListener(this InputEntity entity) { return (InputNamespaceAnyTestEvent3RemovedListenerComponent)entity.GetComponent(InputComponentsLookup.InputNamespaceAnyTestEvent3RemovedListener); }
    public static bool HasInputNamespaceAnyTestEvent3RemovedListener(this InputEntity entity) { return entity.HasComponent(InputComponentsLookup.InputNamespaceAnyTestEvent3RemovedListener); }

    public static void AddInputNamespaceAnyTestEvent3RemovedListener(this InputEntity entity, System.Collections.Generic.List<IInputNamespaceAnyTestEvent3RemovedListener> newValue)
    {
        var index = InputComponentsLookup.InputNamespaceAnyTestEvent3RemovedListener;
        var component = (InputNamespaceAnyTestEvent3RemovedListenerComponent)entity.CreateComponent(index, typeof(InputNamespaceAnyTestEvent3RemovedListenerComponent));
        component.value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceInputNamespaceAnyTestEvent3RemovedListener(this InputEntity entity, System.Collections.Generic.List<IInputNamespaceAnyTestEvent3RemovedListener> newValue)
    {
        var index = InputComponentsLookup.InputNamespaceAnyTestEvent3RemovedListener;
        var component = (InputNamespaceAnyTestEvent3RemovedListenerComponent)entity.CreateComponent(index, typeof(InputNamespaceAnyTestEvent3RemovedListenerComponent));
        component.value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveInputNamespaceAnyTestEvent3RemovedListener(this InputEntity entity)
    {
        entity.RemoveComponent(InputComponentsLookup.InputNamespaceAnyTestEvent3RemovedListener);
    }
}

public sealed partial class InputMatcher
{
    static Entitas.IMatcher<InputEntity> _matcherInputNamespaceAnyTestEvent3RemovedListener;

    public static Entitas.IMatcher<InputEntity> InputNamespaceAnyTestEvent3RemovedListener()
    {
        if (_matcherInputNamespaceAnyTestEvent3RemovedListener == null)
        {
            var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.InputNamespaceAnyTestEvent3RemovedListener);
            matcher.componentNames = InputComponentsLookup.componentNames;
            _matcherInputNamespaceAnyTestEvent3RemovedListener = matcher;
        }

        return _matcherInputNamespaceAnyTestEvent3RemovedListener;
    }
}


// InputInputNamespaceAnyTestEvent3RemovedListenerComponentEvent.g.cs
public static class InputInputNamespaceAnyTestEvent3RemovedListenerComponentEventExtensions
{
    public static void AddInputNamespaceAnyTestEvent3RemovedListener(this InputEntity entity, IInputNamespaceAnyTestEvent3RemovedListener value)
    {
        var listeners = entity.HasInputNamespaceAnyTestEvent3RemovedListener()
            ? entity.GetInputNamespaceAnyTestEvent3RemovedListener().value
            : new System.Collections.Generic.List<IInputNamespaceAnyTestEvent3RemovedListener>();
        listeners.Add(value);
        entity.ReplaceInputNamespaceAnyTestEvent3RemovedListener(listeners);
    }

    public static void RemoveInputNamespaceAnyTestEvent3RemovedListener(this InputEntity entity, IInputNamespaceAnyTestEvent3RemovedListener value, bool removeComponentWhenEmpty = true)
    {
        var listeners = entity.GetInputNamespaceAnyTestEvent3RemovedListener().value;
        listeners.Remove(value);
        if (removeComponentWhenEmpty && listeners.Count == 0)
        {
            entity.RemoveInputNamespaceAnyTestEvent3RemovedListener();
        }
        else
        {
            entity.ReplaceInputNamespaceAnyTestEvent3RemovedListener(listeners);
        }
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


// InputNamespaceAnyTestEvent3EventSystem.g.cs
public sealed class InputNamespaceAnyTestEvent3EventSystem : Entitas.ReactiveSystem<InputEntity>
{
    readonly Entitas.IGroup<InputEntity> _listeners;
    readonly System.Collections.Generic.List<InputEntity> _entityBuffer;
    readonly System.Collections.Generic.List<IInputNamespaceAnyTestEvent3Listener> _listenerBuffer;

    public InputNamespaceAnyTestEvent3EventSystem(Contexts contexts) : base(contexts.input)
    {
        _listeners = contexts.input.GetGroup(InputMatcher.InputNamespaceAnyTestEvent3Listener());
        _entityBuffer = new System.Collections.Generic.List<InputEntity>();
        _listenerBuffer = new System.Collections.Generic.List<IInputNamespaceAnyTestEvent3Listener>();
    }

    protected override Entitas.ICollector<InputEntity> GetTrigger(Entitas.IContext<InputEntity> context)
    {
        return Entitas.CollectorContextExtension.CreateCollector(
            context, Entitas.TriggerOnEventMatcherExtension.Added(InputMatcher.NamespaceTestEvent3())
        );
    }

    protected override bool Filter(InputEntity entity)
    {
        return entity.HasNamespaceTestEvent3();
    }

    protected override void Execute(System.Collections.Generic.List<InputEntity> entities)
    {
        foreach (var e in entities)
        {
            var component = e.GetNamespaceTestEvent3();
            foreach (var listenerEntity in _listeners.GetEntities(_entityBuffer))
            {
                _listenerBuffer.Clear();
                _listenerBuffer.AddRange(listenerEntity.GetInputNamespaceAnyTestEvent3Listener().value);
                foreach (var listener in _listenerBuffer)
                {
                    listener.OnNamespaceAnyTestEvent3(e, component.Value);
                }
            }
        }
    }
}


// InputNamespaceAnyTestEvent3ListenerComponent.g.cs
[Entitas.CodeGeneration.Attributes.DontGenerate]
public sealed class InputNamespaceAnyTestEvent3ListenerComponent : Entitas.IComponent
{
    public System.Collections.Generic.List<IInputNamespaceAnyTestEvent3Listener> value;
}


// InputNamespaceAnyTestEvent3RemovedEventSystem.g.cs
public sealed class InputNamespaceAnyTestEvent3RemovedEventSystem : Entitas.ReactiveSystem<InputEntity>
{
    readonly Entitas.IGroup<InputEntity> _listeners;
    readonly System.Collections.Generic.List<InputEntity> _entityBuffer;
    readonly System.Collections.Generic.List<IInputNamespaceAnyTestEvent3RemovedListener> _listenerBuffer;

    public InputNamespaceAnyTestEvent3RemovedEventSystem(Contexts contexts) : base(contexts.input)
    {
        _listeners = contexts.input.GetGroup(InputMatcher.InputNamespaceAnyTestEvent3RemovedListener());
        _entityBuffer = new System.Collections.Generic.List<InputEntity>();
        _listenerBuffer = new System.Collections.Generic.List<IInputNamespaceAnyTestEvent3RemovedListener>();
    }

    protected override Entitas.ICollector<InputEntity> GetTrigger(Entitas.IContext<InputEntity> context)
    {
        return Entitas.CollectorContextExtension.CreateCollector(
            context, Entitas.TriggerOnEventMatcherExtension.Removed(InputMatcher.NamespaceTestEvent3())
        );
    }

    protected override bool Filter(InputEntity entity)
    {
        return !entity.HasNamespaceTestEvent3();
    }

    protected override void Execute(System.Collections.Generic.List<InputEntity> entities)
    {
        foreach (var e in entities)
        {
            
            foreach (var listenerEntity in _listeners.GetEntities(_entityBuffer))
            {
                _listenerBuffer.Clear();
                _listenerBuffer.AddRange(listenerEntity.GetInputNamespaceAnyTestEvent3RemovedListener().value);
                foreach (var listener in _listenerBuffer)
                {
                    listener.OnNamespaceAnyTestEvent3Removed(e);
                }
            }
        }
    }
}


// InputNamespaceAnyTestEvent3RemovedListenerComponent.g.cs
[Entitas.CodeGeneration.Attributes.DontGenerate]
public sealed class InputNamespaceAnyTestEvent3RemovedListenerComponent : Entitas.IComponent
{
    public System.Collections.Generic.List<IInputNamespaceAnyTestEvent3RemovedListener> value;
}


// InputNamespaceTestEvent3Component.g.cs
public static class InputNamespaceTestEvent3EntityExtensions
{
    public static Namespace.TestEvent3Component GetNamespaceTestEvent3(this InputEntity entity) { return (Namespace.TestEvent3Component)entity.GetComponent(InputComponentsLookup.NamespaceTestEvent3); }
    public static bool HasNamespaceTestEvent3(this InputEntity entity) { return entity.HasComponent(InputComponentsLookup.NamespaceTestEvent3); }

    public static void AddNamespaceTestEvent3(this InputEntity entity, string newValue)
    {
        var index = InputComponentsLookup.NamespaceTestEvent3;
        var component = (Namespace.TestEvent3Component)entity.CreateComponent(index, typeof(Namespace.TestEvent3Component));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceNamespaceTestEvent3(this InputEntity entity, string newValue)
    {
        var index = InputComponentsLookup.NamespaceTestEvent3;
        var component = (Namespace.TestEvent3Component)entity.CreateComponent(index, typeof(Namespace.TestEvent3Component));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveNamespaceTestEvent3(this InputEntity entity)
    {
        entity.RemoveComponent(InputComponentsLookup.NamespaceTestEvent3);
    }
}

public sealed partial class InputMatcher
{
    static Entitas.IMatcher<InputEntity> _matcherNamespaceTestEvent3;

    public static Entitas.IMatcher<InputEntity> NamespaceTestEvent3()
    {
        if (_matcherNamespaceTestEvent3 == null)
        {
            var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.NamespaceTestEvent3);
            matcher.componentNames = InputComponentsLookup.componentNames;
            _matcherNamespaceTestEvent3 = matcher;
        }

        return _matcherNamespaceTestEvent3;
    }
}


// ITestEventListener.g.cs
public interface ITestEventListener
{
    void OnTestEvent(GameEntity entity, string value);
}


// ITestEventRemovedListener.g.cs
public interface ITestEventRemovedListener
{
    void OnTestEventRemoved(GameEntity entity);
}


// NamespaceAnyTestEvent2EventSystem.g.cs
public sealed class NamespaceAnyTestEvent2EventSystem : Entitas.ReactiveSystem<GameEntity>
{
    readonly Entitas.IGroup<GameEntity> _listeners;
    readonly System.Collections.Generic.List<GameEntity> _entityBuffer;
    readonly System.Collections.Generic.List<INamespaceAnyTestEvent2Listener> _listenerBuffer;

    public NamespaceAnyTestEvent2EventSystem(Contexts contexts) : base(contexts.game)
    {
        _listeners = contexts.game.GetGroup(GameMatcher.NamespaceAnyTestEvent2Listener());
        _entityBuffer = new System.Collections.Generic.List<GameEntity>();
        _listenerBuffer = new System.Collections.Generic.List<INamespaceAnyTestEvent2Listener>();
    }

    protected override Entitas.ICollector<GameEntity> GetTrigger(Entitas.IContext<GameEntity> context)
    {
        return Entitas.CollectorContextExtension.CreateCollector(
            context, Entitas.TriggerOnEventMatcherExtension.Added(GameMatcher.NamespaceTestEvent2())
        );
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.HasNamespaceTestEvent2();
    }

    protected override void Execute(System.Collections.Generic.List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            var component = e.GetNamespaceTestEvent2();
            foreach (var listenerEntity in _listeners.GetEntities(_entityBuffer))
            {
                _listenerBuffer.Clear();
                _listenerBuffer.AddRange(listenerEntity.GetNamespaceAnyTestEvent2Listener().value);
                foreach (var listener in _listenerBuffer)
                {
                    listener.OnNamespaceAnyTestEvent2(e, component.Value);
                }
            }
        }
    }
}


// NamespaceAnyTestEvent2ListenerComponent.g.cs
[Entitas.CodeGeneration.Attributes.DontGenerate]
public sealed class NamespaceAnyTestEvent2ListenerComponent : Entitas.IComponent
{
    public System.Collections.Generic.List<INamespaceAnyTestEvent2Listener> value;
}


// NamespaceAnyTestEvent2RemovedEventSystem.g.cs
public sealed class NamespaceAnyTestEvent2RemovedEventSystem : Entitas.ReactiveSystem<GameEntity>
{
    readonly Entitas.IGroup<GameEntity> _listeners;
    readonly System.Collections.Generic.List<GameEntity> _entityBuffer;
    readonly System.Collections.Generic.List<INamespaceAnyTestEvent2RemovedListener> _listenerBuffer;

    public NamespaceAnyTestEvent2RemovedEventSystem(Contexts contexts) : base(contexts.game)
    {
        _listeners = contexts.game.GetGroup(GameMatcher.NamespaceAnyTestEvent2RemovedListener());
        _entityBuffer = new System.Collections.Generic.List<GameEntity>();
        _listenerBuffer = new System.Collections.Generic.List<INamespaceAnyTestEvent2RemovedListener>();
    }

    protected override Entitas.ICollector<GameEntity> GetTrigger(Entitas.IContext<GameEntity> context)
    {
        return Entitas.CollectorContextExtension.CreateCollector(
            context, Entitas.TriggerOnEventMatcherExtension.Removed(GameMatcher.NamespaceTestEvent2())
        );
    }

    protected override bool Filter(GameEntity entity)
    {
        return !entity.HasNamespaceTestEvent2();
    }

    protected override void Execute(System.Collections.Generic.List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            
            foreach (var listenerEntity in _listeners.GetEntities(_entityBuffer))
            {
                _listenerBuffer.Clear();
                _listenerBuffer.AddRange(listenerEntity.GetNamespaceAnyTestEvent2RemovedListener().value);
                foreach (var listener in _listenerBuffer)
                {
                    listener.OnNamespaceAnyTestEvent2Removed(e);
                }
            }
        }
    }
}


// NamespaceAnyTestEvent2RemovedListenerComponent.g.cs
[Entitas.CodeGeneration.Attributes.DontGenerate]
public sealed class NamespaceAnyTestEvent2RemovedListenerComponent : Entitas.IComponent
{
    public System.Collections.Generic.List<INamespaceAnyTestEvent2RemovedListener> value;
}


// TestEventEventSystem.g.cs
public sealed class TestEventEventSystem : Entitas.ReactiveSystem<GameEntity>
{
    readonly System.Collections.Generic.List<ITestEventListener> _listenerBuffer;

    public TestEventEventSystem(Contexts contexts) : base(contexts.game)
    {
        _listenerBuffer = new System.Collections.Generic.List<ITestEventListener>();
    }

    protected override Entitas.ICollector<GameEntity> GetTrigger(Entitas.IContext<GameEntity> context)
    {
        return Entitas.CollectorContextExtension.CreateCollector(
            context, Entitas.TriggerOnEventMatcherExtension.Added(GameMatcher.TestEvent())
        );
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.HasTestEvent() && entity.HasTestEventListener();
    }

    protected override void Execute(System.Collections.Generic.List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            var component = e.GetTestEvent();
            _listenerBuffer.Clear();
            _listenerBuffer.AddRange(e.GetTestEventListener().value);
            foreach (var listener in _listenerBuffer)
            {
                listener.OnTestEvent(e, component.Value);
            }
        }
    }
}


// TestEventListenerComponent.g.cs
[Entitas.CodeGeneration.Attributes.DontGenerate]
public sealed class TestEventListenerComponent : Entitas.IComponent
{
    public System.Collections.Generic.List<ITestEventListener> value;
}


// TestEventRemovedEventSystem.g.cs
public sealed class TestEventRemovedEventSystem : Entitas.ReactiveSystem<GameEntity>
{
    readonly System.Collections.Generic.List<ITestEventRemovedListener> _listenerBuffer;

    public TestEventRemovedEventSystem(Contexts contexts) : base(contexts.game)
    {
        _listenerBuffer = new System.Collections.Generic.List<ITestEventRemovedListener>();
    }

    protected override Entitas.ICollector<GameEntity> GetTrigger(Entitas.IContext<GameEntity> context)
    {
        return Entitas.CollectorContextExtension.CreateCollector(
            context, Entitas.TriggerOnEventMatcherExtension.Removed(GameMatcher.TestEvent())
        );
    }

    protected override bool Filter(GameEntity entity)
    {
        return !entity.HasTestEvent() && entity.HasTestEventRemovedListener();
    }

    protected override void Execute(System.Collections.Generic.List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            
            _listenerBuffer.Clear();
            _listenerBuffer.AddRange(e.GetTestEventRemovedListener().value);
            foreach (var listener in _listenerBuffer)
            {
                listener.OnTestEventRemoved(e);
            }
        }
    }
}


// TestEventRemovedListenerComponent.g.cs
[Entitas.CodeGeneration.Attributes.DontGenerate]
public sealed class TestEventRemovedListenerComponent : Entitas.IComponent
{
    public System.Collections.Generic.List<ITestEventRemovedListener> value;
}
