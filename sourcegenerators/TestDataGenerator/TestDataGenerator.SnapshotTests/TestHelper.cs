using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace TestDataGenerator.SnapshotTests;

public static class TestHelper
{
    public static Task Verify(string source)
    {
        // Parse the input source
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        // Create references (similar to a real compilation)
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Console).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location),
        };

        // Create the compilation
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // Create the source generator
        var generator = new TestDataGeneratorImpl();

        // Create the generator driver
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the generator
        var runResult = driver.RunGenerators(compilation);

        // Get the result
        var result = runResult.GetRunResult();

        // Verify that the generator ran without errors
        Assert.Empty(result.Diagnostics);

        // Get generated sources
        var generatedSources = result.Results[0].GeneratedSources;

        return Verifier.Verify(generatedSources)
            .UseDirectory("Snapshots");
    }
}