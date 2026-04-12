// GameDeadComponentExt.g.cs
public static class DeadGameEntityExtensions
{
    static readonly DeadComponent deadComponent = new DeadComponent();

    public static bool isDead(this GameEntity entity)
        => entity.HasComponent(GameExtComponentsLookup.Dead);

    public static void SetDead(this GameEntity entity, bool value)
    {
        if (value != isDead(entity))
        {
            var index = GameExtComponentsLookup.Dead;
            if (value)
            {
                var componentPool = entity.GetComponentPool(index);
                var component = componentPool.Count > 0
                    ? componentPool.Pop()
                    : deadComponent;
                entity.AddComponent(index, component);
            }
            else
            {
                entity.RemoveComponent(index);
            }
        }
    }
}

public static class DeadGameMatcherExtensions
{
    static Entitas.IMatcher<GameEntity> _matcherDead;

    public static Entitas.IMatcher<GameEntity> Dead
    {
        get
        {
            if (_matcherDead == null)
            {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameExtComponentsLookup.Dead);
                matcher.componentNames = GameContextComponentRegistry.ComponentNames;
                _matcherDead = matcher;
            }

            return _matcherDead;
        }
    }
}


// GameExtComponentsLookup.g.cs
internal static class GameExtComponentsLookup
{
    internal static int Speed;
    internal static int Dead;
    internal static void Register()
    {
        var baseIndex = GameContextComponentRegistry.Register(
            2,
            new string[] { "Speed", "Dead" },
            new System.Type[] { typeof(SpeedComponent), typeof(DeadComponent) }
        );
        Speed = baseIndex + 0;
        Dead = baseIndex + 1;
    }
}

internal static class GameExtComponentsLookupModuleInit
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    internal static void Init() => GameExtComponentsLookup.Register();
}


// GameSpeedComponentExt.g.cs
public static class SpeedGameEntityExtensions
{
    public static SpeedComponent speed(this GameEntity entity)
        => (SpeedComponent)entity.GetComponent(GameExtComponentsLookup.Speed);

    public static bool hasSpeed(this GameEntity entity)
        => entity.HasComponent(GameExtComponentsLookup.Speed);

    public static void AddSpeed(this GameEntity entity, float newValue)
    {
        var index = GameExtComponentsLookup.Speed;
        var component = (SpeedComponent)entity.CreateComponent(index, typeof(SpeedComponent));
        component.Value = newValue;
        entity.AddComponent(index, component);
    }

    public static void ReplaceSpeed(this GameEntity entity, float newValue)
    {
        var index = GameExtComponentsLookup.Speed;
        var component = (SpeedComponent)entity.CreateComponent(index, typeof(SpeedComponent));
        component.Value = newValue;
        entity.ReplaceComponent(index, component);
    }

    public static void RemoveSpeed(this GameEntity entity)
        => entity.RemoveComponent(GameExtComponentsLookup.Speed);
}

public static class SpeedGameMatcherExtensions
{
    static Entitas.IMatcher<GameEntity> _matcherSpeed;

    public static Entitas.IMatcher<GameEntity> Speed
    {
        get
        {
            if (_matcherSpeed == null)
            {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameExtComponentsLookup.Speed);
                matcher.componentNames = GameContextComponentRegistry.ComponentNames;
                _matcherSpeed = matcher;
            }

            return _matcherSpeed;
        }
    }
}
