using System.Collections.Immutable;
using Xunit;

namespace Entitas.CodeGeneration.Tests;

public class EntitasGeneratorSplitAssemblyTests
{
    // --- ShouldRunForAssembly tests ---

    [Fact]
    public void ShouldRun_WhenNoConfig_RunsForDefaultAssembly()
    {
        var result = EntitasGenerator.ShouldRunForAssembly("Assembly-CSharp", ImmutableArray<string>.Empty);
        Assert.True(result);
    }

    [Fact]
    public void ShouldRun_WhenNoConfig_DoesNotRunForOtherAssembly()
    {
        var result = EntitasGenerator.ShouldRunForAssembly("MyGame.Audio", ImmutableArray<string>.Empty);
        Assert.False(result);
    }

    [Fact]
    public void ShouldRun_AlwaysRunsForTestAssembly()
    {
        var result = EntitasGenerator.ShouldRunForAssembly("Entitas.CodeGeneration-Tests", ImmutableArray<string>.Empty);
        Assert.True(result);
    }

    [Fact]
    public void ShouldRun_WhenConfigured_RunsForConfiguredAssembly()
    {
        var configured = ImmutableArray.Create("Assembly-CSharp", "MyGame.Audio");
        Assert.True(EntitasGenerator.ShouldRunForAssembly("Assembly-CSharp", configured));
        Assert.True(EntitasGenerator.ShouldRunForAssembly("MyGame.Audio", configured));
    }

    [Fact]
    public void ShouldRun_WhenConfigured_DoesNotRunForUnconfiguredAssembly()
    {
        var configured = ImmutableArray.Create("Assembly-CSharp", "MyGame.Audio");
        Assert.False(EntitasGenerator.ShouldRunForAssembly("MyGame.Gameplay", configured));
    }

    [Fact]
    public void ShouldRun_WhenConfigured_TestAssemblyStillRuns()
    {
        // Even if test assembly is not in the list, it should still run
        var configured = ImmutableArray.Create("Assembly-CSharp");
        Assert.True(EntitasGenerator.ShouldRunForAssembly("Entitas.CodeGeneration-Tests", configured));
    }

    [Fact]
    public void ShouldRun_NullAssemblyName_ReturnsFalse()
    {
        Assert.False(EntitasGenerator.ShouldRunForAssembly(null, ImmutableArray<string>.Empty));
    }

    // --- ParseTargetAssemblies tests ---

    [Fact]
    public void ParseTargetAssemblies_WithCommaDelimitedList_ParsesCorrectly()
    {
        var options = new TestAnalyzerConfigOptions(new Dictionary<string, string>
        {
            [EntitasGenerator.TargetAssembliesPropertyKey] = "Assembly-CSharp,MyGame.Audio,MyGame.Gameplay"
        });

        var result = EntitasGenerator.ParseTargetAssemblies(options);

        Assert.Equal(3, result.Length);
        Assert.Contains("Assembly-CSharp", result);
        Assert.Contains("MyGame.Audio", result);
        Assert.Contains("MyGame.Gameplay", result);
    }

    [Fact]
    public void ParseTargetAssemblies_WithSemicolonDelimitedList_ParsesCorrectly()
    {
        var options = new TestAnalyzerConfigOptions(new Dictionary<string, string>
        {
            [EntitasGenerator.TargetAssembliesPropertyKey] = "Assembly-CSharp;MyGame.Audio"
        });

        var result = EntitasGenerator.ParseTargetAssemblies(options);

        Assert.Equal(2, result.Length);
        Assert.Contains("Assembly-CSharp", result);
        Assert.Contains("MyGame.Audio", result);
    }

    [Fact]
    public void ParseTargetAssemblies_WithSpacesAroundNames_TrimsCorrectly()
    {
        var options = new TestAnalyzerConfigOptions(new Dictionary<string, string>
        {
            [EntitasGenerator.TargetAssembliesPropertyKey] = " Assembly-CSharp , MyGame.Audio "
        });

        var result = EntitasGenerator.ParseTargetAssemblies(options);

        Assert.Equal(2, result.Length);
        Assert.Contains("Assembly-CSharp", result);
        Assert.Contains("MyGame.Audio", result);
    }

    [Fact]
    public void ParseTargetAssemblies_WhenNoConfig_ReturnsEmpty()
    {
        var options = new TestAnalyzerConfigOptions(new Dictionary<string, string>());

        var result = EntitasGenerator.ParseTargetAssemblies(options);

        Assert.True(result.IsEmpty);
    }

    [Fact]
    public void ParseTargetAssemblies_WhenEmptyValue_ReturnsEmpty()
    {
        var options = new TestAnalyzerConfigOptions(new Dictionary<string, string>
        {
            [EntitasGenerator.TargetAssembliesPropertyKey] = "   "
        });

        var result = EntitasGenerator.ParseTargetAssemblies(options);

        Assert.True(result.IsEmpty);
    }
}
