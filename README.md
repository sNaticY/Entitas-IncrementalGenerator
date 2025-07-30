This repository provides a new code generator for <a href="https://github.com/sschmid/Entitas">Entitas</a>, eliminating the need for <a href="https://github.com/sschmid/Jenny">Jenny</a>.

.NET's Incremental Generators perform code generation at compile time, adding source code to your project automatically. You don't need to trigger generations manually anymore.

Support:
- Contexts, components, events, entity index, cleanup systems...
  - Almost everything from the original Entitas code generator (it uses the same generated source templates)
  - Tests are provided in Entitas.CodeGeneration.Tests

Limitations:
- Generated code is not visible in source control.
  - By default, incremental generators inject new source code directly into dlls, at compile time, instead of writing .cs files on disk.
- Not supported: CustomEntityIndex attribute (don't see the need)
- Not supported: Generation of components from non-components classes (when tagging any class with a ContextAttribute).
  - It's quick enough to write a component instead.
- No custom config is passed to the generator:
  - No more Jenny.properties.. so contexts must be defined in code (see Usage section).
  - "Assembly-CSharp" is hardcoded in EntitasGenerator.cs
  - Default Context name is "Game" (when you don't specify the context on a component, it goes to the default context - Game).
- Generation is not supported across multiple assemblies (due to 'partial' usage in Entities and Contexts)

Setup:
- Build the solution
- Place Entitas.CodeGeneration.dll and Entitas.CodeGeneration.Attributes.dll somewhere within Assembly-CSharp.

Usage:
- Define the Game Context attribute (it's the default context) as follow:
```
using Entitas;
using Entitas.CodeGeneration.Attributes;

public sealed class GameAttribute : Entitas.CodeGeneration.Attributes.ContextAttribute
{
    public GameAttribute() : base("Game") {}
}
```
- Then, write a component as usual
```
public class TestComponent : IComponent
{
    public string Value;
}
```
- After the compilation, you should magically be able to interact with this component on any GameEntity
