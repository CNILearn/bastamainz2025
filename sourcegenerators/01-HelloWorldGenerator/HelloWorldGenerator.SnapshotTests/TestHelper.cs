using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace HelloWorldGenerator.Tests;

public static class TestHelper
{
    public static Task VerifyGenerator(string source, [System.Runtime.CompilerServices.CallerMemberName] string testName = "")
    {
        // Create the generator
        HelloWorldSourceGenerator generator = new();

        // Create the compilation
        CSharpParseOptions parseOptions = new(LanguageVersion.Latest);
        var syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions);

        // Add all the assembly references that are used in the source-generator generated code
        PortableExecutableReference[] references =
        [
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
        ];

        var compilation = CSharpCompilation.Create(
            assemblyName: "TestAssembly",
            syntaxTrees: [syntaxTree],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.ConsoleApplication));

        // Use Verify.SourceGenerators to verify the output
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);
        
        return Verifier.Verify(driver.GetRunResult())
            .UseDirectory("Snapshots")
            .UseFileName(testName);
    }
}