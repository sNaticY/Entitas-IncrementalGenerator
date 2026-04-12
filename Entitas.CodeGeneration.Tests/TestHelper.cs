using System.Diagnostics;
using Entitas.CodeGeneration.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using VerifyXunit;
using Xunit.Abstractions;

namespace Entitas.CodeGeneration.Tests;

public static class TestHelper
{
    public delegate CSharpCompilation CompilationModifier(CSharpCompilation original);
    
    public static Task Verify(string source, ITestOutputHelper outputHelper, 
        CompilationModifier? secondCompilationModifier = null,
        string assemblyName = "Entitas.CodeGeneration-Tests",
        Dictionary<string, string>? analyzerConfigOptions = null)
    {
        // Parse the provided string into a C# syntax tree
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        var references = GetCompilationReferences();
        
        // Create a Roslyn compilation for the syntax tree.
        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: assemblyName,
            syntaxTrees: new[] { syntaxTree },
            references: references);
        
        // Create an instance of our generator incremental source generator
        var generator = new EntitasGenerator();

        // Build the generator driver, optionally with analyzer config options
        GeneratorDriver driver;
        if (analyzerConfigOptions != null && analyzerConfigOptions.Count > 0)
        {
            var optionsProvider = new TestAnalyzerConfigOptionsProvider(analyzerConfigOptions);
            driver = CSharpGeneratorDriver.Create(
                generators: new[] { generator.AsSourceGenerator() },
                optionsProvider: optionsProvider);
        }
        else
        {
            driver = CSharpGeneratorDriver.Create(generator);
        }

        var sw = Stopwatch.StartNew();
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out var diagnostics);
        
        sw.Stop();
        outputHelper.WriteLine("Elapsed test time: "+sw.Elapsed);
        
        // Allow tests to modify compilation before running the second generation
        if (secondCompilationModifier != null)
            updatedCompilation = secondCompilationModifier.Invoke((CSharpCompilation)updatedCompilation);
        
        sw = Stopwatch.StartNew();
        // Run a second generation using cache, it should run very fast!
        driver = driver.RunGeneratorsAndUpdateCompilation(updatedCompilation, out var updatedCompilation2, out var diagnostics2);
        sw.Stop();
        outputHelper.WriteLine("Elapsed test time 2: "+sw.Elapsed);

        var runResults = driver.GetRunResult();
        
        // Gather all generated sources
        var outputs = runResults
            .Results
            .SelectMany(r => r.GeneratedSources)
            .OrderBy(s => s.HintName) // sort alphabetically by filename
            .Select(s => "// "+s.HintName+"\n"+s.SourceText)
            .ToList();

        // Combine all results in a single string for single verification
        var combinedOutput = string.Join("\n\n", outputs);
        
        // Use verify to snapshot test the source generator output!
        return Verifier.Verify(combinedOutput).UseExtension("cs");
    }

    static IEnumerable<MetadataReference> GetCompilationReferences()
    {
        // it's suboptimal and slow to add all possible references to the compilation...
        // but I've had headaches trying to find which reference was missing in the generator
        
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .ToList();

        TryAddMissingAssemblyReference(typeof(IComponent), references);
        TryAddMissingAssemblyReference(typeof(UniqueAttribute), references);

        return references;
    }

    static void TryAddMissingAssemblyReference(Type type, List<PortableExecutableReference> references)
    {
        var typeLocation = type.Assembly.Location;
        if (!references.Any(r => string.Equals(r.Display, typeLocation, StringComparison.OrdinalIgnoreCase)))
        {
            references.Add(MetadataReference.CreateFromFile(typeLocation));
        }
    }
}

/// <summary>
/// A test implementation of <see cref="AnalyzerConfigOptionsProvider"/> that returns
/// the provided global options for all files.
/// </summary>
class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
{
    readonly AnalyzerConfigOptions _globalOptions;

    public TestAnalyzerConfigOptionsProvider(Dictionary<string, string> options)
    {
        _globalOptions = new TestAnalyzerConfigOptions(options);
    }

    public override AnalyzerConfigOptions GlobalOptions => _globalOptions;

    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => _globalOptions;

    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => _globalOptions;
}

class TestAnalyzerConfigOptions : AnalyzerConfigOptions
{
    readonly Dictionary<string, string> _options;

    public TestAnalyzerConfigOptions(Dictionary<string, string> options)
    {
        _options = options;
    }

    public override bool TryGetValue(string key, out string value)
    {
        if (_options.TryGetValue(key, out var found))
        {
            value = found;
            return true;
        }
        value = string.Empty;
        return false;
    }
}