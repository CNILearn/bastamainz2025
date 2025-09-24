using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LanguageVersionGenerator.Tests;

public static class TestHelper
{
    public static (string GeneratedSource, string[] Diagnostics) RunGenerator(string source, LanguageVersion languageVersion = LanguageVersion.Latest)
    {
        // Create the generator
        var generator = new LanguageVersionSourceGenerator();

        // Create the compilation with specified language version
        var parseOptions = new CSharpParseOptions(languageVersion);
        var syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions);
        PortableExecutableReference[] references =
        [
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(List<>).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
        ];

        var compilation = CSharpCompilation.Create(
            assemblyName: "TestAssembly",
            syntaxTrees: [syntaxTree],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.ConsoleApplication));

        // Run the generator with parse options
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [generator.AsSourceGenerator()], 
            parseOptions: parseOptions);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        // Get the generated source
        var runResult = driver.GetRunResult();
        var generatedSource = "";
        if (runResult.Results.Length > 0 && runResult.Results[0].GeneratedSources.Length > 0)
        {
            generatedSource = runResult.Results[0].GeneratedSources[0].SourceText.ToString();
        }

        var diagnosticMessages = diagnostics.Select(d => d.ToString()).ToArray();
        
        return (generatedSource, diagnosticMessages);
    }
}