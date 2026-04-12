using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
        CompilationModifier? secondCompilationModifier = null)
        => Verify(source, "Entitas.CodeGeneration-Tests", outputHelper,
            secondCompilationModifier: secondCompilationModifier);

    public static Task Verify(string source, string assemblyName, ITestOutputHelper outputHelper,
        Dictionary<string, string>? globalOptions = null,
        CompilationModifier? secondCompilationModifier = null)
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

        // The GeneratorDriver is used to run our generator against a compilation
        AnalyzerConfigOptionsProvider? optionsProvider = globalOptions != null
            ? new TestAnalyzerConfigOptionsProvider(globalOptions)
            : null;

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: ImmutableArray.Create(generator.AsSourceGenerator()),
            optionsProvider: optionsProvider);

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

    /// <summary>
    /// Minimal <see cref="AnalyzerConfigOptionsProvider"/> for use in unit tests.
    /// </summary>
    sealed class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        readonly TestAnalyzerConfigOptions _globalOptions;

        public TestAnalyzerConfigOptionsProvider(Dictionary<string, string> globalOptions)
            => _globalOptions = new TestAnalyzerConfigOptions(globalOptions);

        public override AnalyzerConfigOptions GlobalOptions => _globalOptions;
        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => TestAnalyzerConfigOptions.Empty;
        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => TestAnalyzerConfigOptions.Empty;
    }

    sealed class TestAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        public static readonly TestAnalyzerConfigOptions Empty = new(new Dictionary<string, string>());

        readonly Dictionary<string, string> _options;

        public TestAnalyzerConfigOptions(Dictionary<string, string> options)
            => _options = options;

        public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
            => _options.TryGetValue(key, out value);
    }
}