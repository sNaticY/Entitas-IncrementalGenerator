# Entitas Incremental Generator

Incremental Roslyn source generator for <a href="https://github.com/sschmid/Entitas">Entitas</a>. It replaces the old <a href="https://github.com/sschmid/Jenny">Jenny</a> workflow by generating Entitas code at compile time instead of writing generated `.cs` files into your Unity project.

## Overview

- Generates `Contexts`, per-context `Context`, `Entity`, and `Matcher` types.
- Generates entity APIs for normal, flag, unique, and multi-context components.
- Generates per-context `ComponentsLookup` classes.
- Generates entity index and primary entity index registration plus lookup helpers.
- Generates cleanup systems.
- Generates event listener components, listener interfaces, event systems, and per-context event system lists.
- Generates visual debugging sources for `Assembly-CSharp`.

Generated code is injected into the compilation. It is not written into the repository as regular `.cs` files.

## Repository Layout

- `Entitas.CodeGeneration/` contains the incremental source generator and templates.
- `Entitas.CodeGeneration.Attributes/` contains the public attribute types used by Unity projects.
- `Entitas.CodeGeneration.Tests/` contains Verify snapshot tests for generated output.

## Requirements

- The repo pins the .NET SDK to `10.0.202` in `global.json`. Any compatible `10.0.x` SDK should work because roll-forward is enabled.
- The generator and attributes projects target `netstandard2.0`.
- The tests target `net6.0`.
- The generator depends on `Microsoft.CodeAnalysis.CSharp` `4.0.1` for Unity compatibility.

## Unity Setup

1. Build the solution in Release mode.

```bash
dotnet build "Entitas.CodeGeneration.sln" -c Release
```

2. Copy `Entitas.CodeGeneration.dll` and `Entitas.CodeGeneration.Attributes.dll` into your Unity project's `Assets/` folder.

3. Configure `Entitas.CodeGeneration.dll` as a Roslyn analyzer in Unity.
Follow Unity's source generator setup guide: https://docs.unity3d.com/6000.1/Documentation/Manual/create-source-generator.html
Disable `Any Platform`, disable `Editor` and `Standalone`, and add the `RoslynAnalyzer` asset label.

4. Keep the code that should be generated in `Assembly-CSharp`.
The generator currently only runs for the `Assembly-CSharp` Unity assembly.

5. Define at least one context attribute in code.

```csharp
using Entitas.CodeGeneration.Attributes;

public sealed class GameAttribute : ContextAttribute
{
    public GameAttribute() : base("Game") { }
}
```

The context name is derived from the class name, so `GameAttribute` becomes the `Game` context.

## Conventions

- Contexts are discovered from classes ending with `Attribute` that inherit `ContextAttribute`.
- Components are discovered from non-abstract classes ending with `Component` that implement `Entitas.IComponent`.
- If a component has no explicit context attribute, it defaults to the `Game` context.
- Component members come from public instance fields and public auto-properties.
- Direct `[Context("Name")]` usage can tag a component for an existing context, but it does not create the context types by itself.

## Example

```csharp
using Entitas;

public sealed class HealthComponent : IComponent
{
    public int Value;
}
```

After compilation, the generated API can be used like this:

```csharp
var contexts = Contexts.sharedInstance;
var entity = contexts.game.CreateEntity();

entity.AddHealth(10);
var health = entity.GetHealth().Value;

var group = contexts.game.GetGroup(GameMatcher.Health());
```

## Static API Changes

Recent generator changes moved the per-component API generation away from generated partial-type members and into generated static extension classes.

- Entity and context component APIs are now emitted as extension methods.
- Most call sites still look the same. Entity APIs still read like `entity.AddHealth(...)` or `entity.HasHealth()`, and unique context APIs still read like `contexts.game.SetGameConfig(...)`.
- Matcher accessors are methods, not static properties. Use `GameMatcher.Health()` instead of `GameMatcher.Health`.
- Entity index helpers are extension methods on the context, for example `context.GetEntityWithPlayerId(id)` and `context.GetEntitiesWithTag(tag)`.

If you had tooling or reflection code that depended on the older generated partial-member shape, update it to the extension-based API surface.

## Current Limitations

- Generated sources are compile-time only and are not written to disk as normal source files.
- Generation currently runs only for `Assembly-CSharp` and the internal test assembly `Entitas.CodeGeneration-Tests`.
- There is no replacement for `Jenny.properties` yet. Main assembly selection, visual debugging generation, and some generator options are hardcoded.
- Cross-assembly generation is not supported.
- `CustomEntityIndexAttribute` is not supported.
- `ComponentNameAttribute` and `EntityIndexGetMethodAttribute` exist in the attributes assembly but are not consumed by the generator yet.

## Development

Build the solution:

```bash
dotnet build "Entitas.CodeGeneration.sln"
```

Run all tests:

```bash
dotnet test "Entitas.CodeGeneration.sln"
```

Run only generator tests:

```bash
dotnet test "Entitas.CodeGeneration.Tests/Entitas.CodeGeneration.Tests.csproj"
```

Snapshot files live under `Entitas.CodeGeneration.Tests/Snapshots/`.
