namespace Entitas.CodeGeneration.Tests;

public static class TestSources
{
    public const string ContextsTestSource = @"
public sealed class GameAttribute : Entitas.CodeGeneration.Attributes.ContextAttribute
{
    public GameAttribute() : base(""Game"") {}
}

public sealed class InputAttribute : Entitas.CodeGeneration.Attributes.ContextAttribute
{
    public InputAttribute() : base(""Input"") {}
}
";
    
    public const string SimpleComponentTestSource = @"
using Entitas;
using Entitas.CodeGeneration.Attributes;

public sealed class GameAttribute : Entitas.CodeGeneration.Attributes.ContextAttribute
{
    public GameAttribute() : base(""Game"") {}
}

public class TestFlagComponent : IComponent
{
}

public class TestMemberComponent : IComponent
{
    public string Value;
}
";
    
    public const string IgnoreGeneratedComponentsTestSource = @"
using Entitas;
using Entitas.CodeGeneration.Attributes;

public sealed class GameAttribute : Entitas.CodeGeneration.Attributes.ContextAttribute
{
    public GameAttribute() : base(""Game"") {}
}

public class NormalComponent : IComponent
{
}

[Entitas.CodeGeneration.Attributes.DontGenerate]
public class GeneratedComponent : IComponent
{
}
";
    
    public const string FlagPrefixTestSource = @"
using Entitas;
using Entitas.CodeGeneration.Attributes;

public sealed class GameAttribute : Entitas.CodeGeneration.Attributes.ContextAttribute
{
    public GameAttribute() : base(""Game"") {}
}

[FlagPrefix(""has"")]
public class TestFlagComponent : IComponent
{
}
";
    
    public const string UniqueComponentTestSource = @"
using Entitas;
using Entitas.CodeGeneration.Attributes;

public sealed class GameAttribute : Entitas.CodeGeneration.Attributes.ContextAttribute
{
    public GameAttribute() : base(""Game"") {}
}

[Unique]
public class TestUniqueComponent : IComponent
{
    public string Value;
}

[Unique]
public class TestUniqueFlagComponent : IComponent
{
}
";
    
    // This test needs a component assigned to 2 contexts
    public const string ComponentInterfaceTestSource = @"
using Entitas;
using Entitas.CodeGeneration.Attributes;

public sealed class GameAttribute : Entitas.CodeGeneration.Attributes.ContextAttribute
{
    public GameAttribute() : base(""Game"") {}
}

public sealed class InputAttribute : Entitas.CodeGeneration.Attributes.ContextAttribute
{
    public InputAttribute() : base(""Input"") {}
}

[Game, Input]
public class TestFlagComponent : IComponent
{
}

[Game, Input]
public class TestMemberComponent : IComponent
{
    public string Value;
}
";
    
    public const string GenerateManyComponentsTestSource = @"
using Entitas;
using Entitas.CodeGeneration.Attributes;

public sealed class GameAttribute : Entitas.CodeGeneration.Attributes.ContextAttribute
{
    public GameAttribute() : base(""Game"") {}
}

public sealed class InputAttribute : Entitas.CodeGeneration.Attributes.ContextAttribute
{
    public InputAttribute() : base(""Input"") {}
}

public sealed class AudioAttribute : Entitas.CodeGeneration.Attributes.ContextAttribute
{
    public AudioAttribute() : base(""Audio"") {}
}

[Game, Audio, Input]
public class TestMemberComponent : IComponent
{
    public string Value;
}
[Game, Audio, Input]
public class TestFlagComponent : IComponent
{
}
[Game, Audio, Input]
public class TestMember3Component : IComponent
{
    public string Value;
}
[Game, Audio, Input]
public class TestMember2Component : IComponent
{
    public string Value;
}
[Game, Audio, Input]
public class TestMember4Component : IComponent
{
    public string Value;
}
[Game, Audio, Input]
public class TestMember5Component : IComponent
{
    public string Value;
}
[Game, Audio, Input]
public class TestMember6Component : IComponent
{
    public string Value;
}
[Game, Audio, Input]
public class TestMember7Component : IComponent
{
    public string Value;
}
[Game, Audio, Input]
public class TestMember8Component : IComponent
{
    public string Value;
}
";
    
    public const string EntityIndexTestSource = @"
using Entitas;
using Entitas.CodeGeneration.Attributes;

public sealed class GameAttribute : Entitas.CodeGeneration.Attributes.ContextAttribute
{
    public GameAttribute() : base(""Game"") {}
}

public class TestMemberComponent : IComponent
{
    [EntityIndex]
    public string Value;
}
";
    
    public const string PrimaryEntityIndexTestSource = @"
using Entitas;
using Entitas.CodeGeneration.Attributes;

public sealed class GameAttribute : Entitas.CodeGeneration.Attributes.ContextAttribute
{
    public GameAttribute() : base(""Game"") {}
}

public class TestMemberComponent : IComponent
{
    [PrimaryEntityIndex]
    public string Value;
}

public class TestMember2Component : IComponent
{
    [PrimaryEntityIndex]
    public string Value;
}
";
    
    public const string DestroyComponentCleanupSystemTestSource = @"
using Entitas;
using Entitas.CodeGeneration.Attributes;

public sealed class GameAttribute : Entitas.CodeGeneration.Attributes.ContextAttribute
{
    public GameAttribute() : base(""Game"") {}
}

[Cleanup(CleanupMode.DestroyEntity)]
public class TestMemberComponent : IComponent
{
    public string Value;
}
";
    
    public const string RemoveEntityCleanupSystemTestSource = @"
using Entitas;
using Entitas.CodeGeneration.Attributes;

public sealed class GameAttribute : Entitas.CodeGeneration.Attributes.ContextAttribute
{
    public GameAttribute() : base(""Game"") {}
}

[Cleanup(CleanupMode.RemoveComponent)]
public class TestMemberComponent : IComponent
{
    public string Value;
}
";
    
    public const string EventsTestSource = @"
using Entitas;
using Entitas.CodeGeneration.Attributes;

public sealed class GameAttribute : Entitas.CodeGeneration.Attributes.ContextAttribute
{
    public GameAttribute() : base(""Game"") {}
}

public sealed class InputAttribute : Entitas.CodeGeneration.Attributes.ContextAttribute
{
    public InputAttribute() : base(""Input"") {}
}

[Event(EventTarget.Any), Event(EventTarget.Any, EventType.Removed)]
[Event(EventTarget.Self), Event(EventTarget.Self, EventType.Removed)]
public class TestEventComponent : IComponent
{
    public string Value;
}

namespace Namespace
{
    [Event(EventTarget.Any), Event(EventTarget.Any, EventType.Removed)]
    public class TestEvent2Component : IComponent
    {
        public string Value;
    }

    [Game, Input]
    [Event(EventTarget.Any), Event(EventTarget.Any, EventType.Removed)]
    public class TestEvent3Component : IComponent
    {
        public string Value;
    }
}
";

    public const string SimpleEventTestSource = @"
using Entitas;
using Entitas.CodeGeneration.Attributes;

public sealed class GameAttribute : Entitas.CodeGeneration.Attributes.ContextAttribute
{
    public GameAttribute() : base(""Game"") {}
}

[Event(EventTarget.Self)]
public class TestEventComponent : IComponent
{
}
";

    /// <summary>
    /// Primary assembly source: defines the Game context + one component.
    /// The secondary assembly source below adds more components to the same context.
    /// </summary>
    public const string CrossAssemblyPrimarySource = @"
using Entitas;
using Entitas.CodeGeneration.Attributes;

public sealed class GameAttribute : Entitas.CodeGeneration.Attributes.ContextAttribute
{
    public GameAttribute() : base(""Game"") {}
}

public class PositionComponent : IComponent
{
    public float X;
    public float Y;
}
";

    /// <summary>
    /// Secondary assembly source: defines components targeting the Game context from the primary assembly.
    /// Contains a flag component and a member component.
    /// </summary>
    public const string CrossAssemblySecondarySource = @"
using Entitas;

public class SpeedComponent : IComponent
{
    public float Value;
}

public class DeadComponent : IComponent
{
}
";
}
