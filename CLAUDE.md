# CLAUDE.md

## Project Summary

This repository contains an incremental Roslyn source generator for Entitas ECS. It replaces the old Jenny workflow by generating code at compile time instead of writing `.cs` files to disk.

The solution has three projects:

- `Entitas.CodeGeneration/` - the generator itself, targets `netstandard2.0`
- `Entitas.CodeGeneration.Attributes/` - attribute types consumed by Unity projects and tests, targets `netstandard2.0`
- `Entitas.CodeGeneration.Tests/` - snapshot tests for generator output, targets `net6.0`

The repo pins the .NET SDK with `global.json` to `10.0.202`. Use that SDK, or a compatible `10.0.x` SDK via roll-forward, before running `dotnet build` or `dotnet test`.

The generator depends on Roslyn `4.0.1` because Unity 6 only supports older CodeAnalysis packages.

## Commands

- Build solution: `dotnet build "Entitas.CodeGeneration.sln"`
- Build Release configuration: `dotnet build "Entitas.CodeGeneration.sln" -c Release`
- Run all tests: `dotnet test "Entitas.CodeGeneration.sln"`
- Run only generator tests: `dotnet test "Entitas.CodeGeneration.Tests/Entitas.CodeGeneration.Tests.csproj"`
- Clean solution: `dotnet clean "Entitas.CodeGeneration.sln"`

## Key Files

- `Entitas.CodeGeneration/EntitasGenerator.cs` - generator entry point and pipeline registration
- `Entitas.CodeGeneration/Contexts/` - context discovery and generation of `Contexts`, per-context classes, matchers, entities
- `Entitas.CodeGeneration/Components/` - component discovery, component APIs, multi-context interfaces
- `Entitas.CodeGeneration/ComponentsLookups/` - per-context `ComponentsLookup`
- `Entitas.CodeGeneration/EntityIndex/` - entity index generation
- `Entitas.CodeGeneration/Events/` - event listener components, interfaces, systems, event system lists
- `Entitas.CodeGeneration/Cleanup/` - cleanup system generation
- `Entitas.CodeGeneration/VisualDebugging/` - `Feature` and `ContextObserver` generation
- `Entitas.CodeGeneration.Attributes/` - public attribute surface used by consuming projects
- `Entitas.CodeGeneration.Tests/TestSources.cs` - inline source snippets used as generator inputs
- `Entitas.CodeGeneration.Tests/TestHelper.cs` - compilation setup and double-run incremental test harness
- `Entitas.CodeGeneration.Tests/ModuleInitializer.cs` - Verify snapshot path configuration
- `Entitas.CodeGeneration.Tests/Snapshots/` - `.verified.cs` snapshots for generated output

## Generator Pipeline

`EntitasGenerator.Initialize()` sets up four main flows:

1. Context discovery via `ContextGenerationHelper.GetContextsData()`
2. Component discovery via `ComponentGenerationHelper.GetComponentsData()`
3. Shared source generation for lookups, indices, cleanup systems, and event system aggregators
4. Per-component generation for entity APIs, per-component event artifacts, and per-component cleanup systems

Important pipeline details:

- The generator only runs when `compilation.AssemblyName` is `Assembly-CSharp` or `Entitas.CodeGeneration-Tests`
- Visual debugging generation is enabled by a hardcoded flag and only runs for `Assembly-CSharp`, not tests
- Event support adds synthetic listener components into the discovered component set before shared generation runs, so downstream lookup/index generation sees them as regular components
- Shared generation is intentionally the heavy path and reruns for any context or component change; keep changes there lightweight

## Discovery Rules

These rules matter more than the attribute API surface when debugging why something did or did not generate.

### Contexts

- Contexts are discovered from `class` declarations ending with `Attribute`
- The syntax filter only checks whether the base list mentions `ContextAttribute`
- `TryGetContextData()` derives the context name from the class name, not the constructor argument
- Example: `public sealed class InputAttribute : ContextAttribute` becomes context `Input`

Implication: if the attribute class name and `base("...")` value disagree, generation follows the class name.

### Components

- Only non-abstract `class` declarations are considered
- The class name must end with `Component`
- `[DontGenerate]` is filtered out syntactically before semantic analysis
- The semantic check requires the type to implement `Entitas.IComponent`
- If a component has no explicit context attributes, it defaults to the `Game` context

### Component Members

- Component members are collected from public instance fields and public auto-properties
- Public members inherited from base types are included
- Non-auto properties, static members, and non-public members are ignored

### Context Assignment

- Components can use derived context attributes such as `[Game]` and `[Input]`
- `ComponentAttributesHelper` also understands direct `[Context("Name")]` usage on components
- Direct `[Context("Name")]` only tags the component; it does not create the corresponding generated context types
- For context classes to exist, there still needs to be a `NameAttribute : ContextAttribute` declaration that passes the context syntax filter

## Generated Output

The generator produces these categories of files:

- `Contexts.g.cs`
- `{Context}Context.g.cs`, `{Context}Matcher.g.cs`, `{Context}Entity.g.cs`
- `{Context}{Component}.g.cs` for entity component APIs
- `I{Component}Entity.g.cs` when a component belongs to more than one valid context
- `{Context}ComponentsLookup.g.cs`
- `{Context}EntityIndices.g.cs`
- `{Context}CleanupSystems.g.cs`
- per-component cleanup systems for `[Cleanup]`
- per-component event listener components, listener interfaces, event entity APIs, and event systems for `[Event]`
- `{Context}EventSystems.g.cs`
- visual debugging `Feature` and context observers for main Unity assembly generation

Generated code is injected into the compilation. It is not written into the repository.

## Testing Workflow

Tests use Verify snapshot testing around an in-memory Roslyn compilation.

- `TestHelper.Verify()` creates a compilation named `Entitas.CodeGeneration-Tests`
- The generator is executed twice to exercise incremental caching behavior
- `GenerateManyComponents` mutates the second compilation to confirm only new work is redone
- Generated sources are combined into a single ordered output before snapshot comparison
- Snapshots live in `Entitas.CodeGeneration.Tests/Snapshots/`
- Accept snapshot updates by reviewing `.received.cs` files and renaming them to `.verified.cs`

Important testing gotchas:

- The hardcoded test assembly name in `TestHelper.cs` must stay aligned with the assembly-name gate in `EntitasGenerator.cs`
- Tests reference a checked-in `Entitas.dll` at `Entitas.CodeGeneration.Tests/References/Entitas.dll`
- Visual debugging generation is intentionally skipped in tests to keep snapshots smaller, so that area has no normal snapshot coverage

## Constraints And Gotchas

- Assembly support is hardcoded to `Assembly-CSharp` and `Entitas.CodeGeneration-Tests`
- There is no real configuration system yet; `MainAssembly`, `VisualDebuggingGenerationEnabled`, and `IgnoreNamespaces` are hardcoded
- Cross-assembly generation is not supported because generated contexts and entities rely on `partial` types in the same assembly
- `CustomEntityIndexAttribute` exists but is explicitly unsupported by the generator
- `ComponentNameAttribute` exists in the attributes project but is not consumed anywhere in generator code today
- `EntityIndexGetMethodAttribute` exists in the attributes project but is not consumed anywhere in generator code today
- Attribute `AttributeUsage` declarations are broader than the real implementation; the generator only discovers component classes, not structs/interfaces/enums
- `EntityIndex` and `PrimaryEntityIndex` generation currently comes from member attributes on component fields/properties; class-level attribute targets exist in the attributes assembly but are not used by generator code
- `Contexts` generation reflects over `[PostConstructor]` methods and invokes them after creating all contexts

## Editing Guidance For Future Sessions

- Start from `EntitasGenerator.cs` to understand whether a change belongs in context discovery, component discovery, shared generation, or per-component generation
- When adding a new generation feature, prefer a new helper namespace plus a matching `*Templates.cs` file rather than crowding existing helpers
- If you change syntax filters, re-check incremental behavior carefully; those filters are intentionally aggressive for performance
- If you touch event generation, remember that event listener components are synthesized and then participate in later shared-generation stages
- If you rename the tests assembly or change the generator's assembly gate, update both `EntitasGenerator.cs` and `Entitas.CodeGeneration.Tests/TestHelper.cs`
- If you change generated output shape, expect snapshot updates under `Entitas.CodeGeneration.Tests/Snapshots/`
