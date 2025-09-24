using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

using System.Collections.Immutable;

using Xunit;

namespace UnsafeAccessorGenerator.Tests;

public class UnsafeAccessorGeneratorTests
{
    [Fact]
    public void GeneratePartialClass_ShouldCreateImplementation()
    {
        // Arrange
        var sourceCode = """
            using System.Collections.Generic;
            
            namespace TestNamespace
            {
                public class Book
                {
                    private string _title = string.Empty;
                    private string _publisher = string.Empty;
                    
                    public override string ToString() => $"{_title} {_publisher}";
                }
                
                public partial class JsonContext
                {
                    public partial IEnumerable<Book> GetBooks(string jsonFile);
                }
            }
            """;

        var additionalFiles = new[]
        {
            ("books.json", """[{"title": "Test Book", "publisher": "Test Publisher"}]""")
        };

        // Act
        var result = RunGenerator(sourceCode, additionalFiles);

        // Assert
        Assert.True(result.Diagnostics.IsEmpty, $"Generator produced diagnostics: {string.Join(", ", result.Diagnostics)}");
        Assert.Single(result.GeneratedTrees); // Should have only implementation, no attribute
        
        var generatedSources = result.GeneratedTrees.Select(t => t.ToString()).ToArray();
        
        // Find the JsonContext implementation
        var jsonContextSource = generatedSources.FirstOrDefault(s => s.Contains("public partial class JsonContext"));
        Assert.NotNull(jsonContextSource);
        
        // Verify UnsafeAccessor methods are generated
        Assert.Contains("[UnsafeAccessor(UnsafeAccessorKind.Constructor)]", jsonContextSource);
        Assert.Contains("CreateBook", jsonContextSource);
        Assert.Contains("[UnsafeAccessor(UnsafeAccessorKind.Field, Name = \"_title\")]", jsonContextSource);
        Assert.Contains("GetBookTitleField", jsonContextSource);
        Assert.Contains("[UnsafeAccessor(UnsafeAccessorKind.Field, Name = \"_publisher\")]", jsonContextSource);
        Assert.Contains("GetBookPublisherField", jsonContextSource);
        
        // Verify GetBooks method implementation
        Assert.Contains("public partial IEnumerable<Book> GetBooks(string jsonFile)", jsonContextSource);
        Assert.Contains("JsonSerializer.Deserialize<JsonElement[]>", jsonContextSource);
        
        // Verify field assignment patterns based on actual implementation
        Assert.Contains("GetBookTitleField(instance) = _titleProp.GetString()", jsonContextSource);
        Assert.Contains("GetBookPublisherField(instance) = _publisherProp.GetString()", jsonContextSource);
    }

    [Fact]
    public void GeneratePartialClass_WithPrivateSetters_ShouldCreateImplementation()
    {
        // Arrange
        var sourceCode = """
            using System.Collections.Generic;
            
            namespace TestNamespace
            {
                public class BookWithPrivateSetters
                {
                    public string Title { get; private set; } = string.Empty;
                    public string Publisher { get; private set; } = string.Empty;
                    
                    public override string ToString() => $"{Title} {Publisher}";
                }
                
                public partial class JsonContext
                {
                    public partial IEnumerable<BookWithPrivateSetters> GetBooksWithPrivateSetters(string jsonFile);
                }
            }
            """;

        var additionalFiles = new[]
        {
            ("books.json", """[{"title": "Test Book", "publisher": "Test Publisher"}]""")
        };

        // Act
        var result = RunGenerator(sourceCode, additionalFiles);

        // Assert
        Assert.True(result.Diagnostics.IsEmpty, $"Generator produced diagnostics: {string.Join(", ", result.Diagnostics)}");
        Assert.Single(result.GeneratedTrees);
        
        var generatedSources = result.GeneratedTrees.Select(t => t.ToString()).ToArray();
        var jsonContextSource = generatedSources.FirstOrDefault(s => s.Contains("public partial class JsonContext"));
        Assert.NotNull(jsonContextSource);
        
        // Verify UnsafeAccessor methods for private setters are generated
        Assert.Contains("CreateBookWithPrivateSetters", jsonContextSource);
        Assert.Contains("[UnsafeAccessor(UnsafeAccessorKind.Method, Name = \"set_Title\")]", jsonContextSource);
        Assert.Contains("SetBookWithPrivateSettersTitle", jsonContextSource);
        Assert.Contains("[UnsafeAccessor(UnsafeAccessorKind.Method, Name = \"set_Publisher\")]", jsonContextSource);
        Assert.Contains("SetBookWithPrivateSettersPublisher", jsonContextSource);
        
        // Verify method implementation uses private setters
        Assert.Contains("SetBookWithPrivateSettersTitle(instance,", jsonContextSource);
        Assert.Contains("SetBookWithPrivateSettersPublisher(instance,", jsonContextSource);
    }

    [Fact]
    public void GeneratePartialClass_WithRecordClass_ShouldCreateImplementation()
    {
        // Arrange
        var sourceCode = """
            using System.Collections.Generic;
            
            namespace TestNamespace
            {
                public record class BookRecord(string Title, string Publisher);
                
                public partial class JsonContext
                {
                    public partial IEnumerable<BookRecord> GetBookRecords(string jsonFile);
                }
            }
            """;

        var additionalFiles = new[]
        {
            ("books.json", """[{"title": "Test Book", "publisher": "Test Publisher"}]""")
        };

        // Act
        var result = RunGenerator(sourceCode, additionalFiles);

        // Assert
        Assert.True(result.Diagnostics.IsEmpty, $"Generator produced diagnostics: {string.Join(", ", result.Diagnostics)}");
        Assert.Single(result.GeneratedTrees);
        
        var generatedSources = result.GeneratedTrees.Select(t => t.ToString()).ToArray();
        var jsonContextSource = generatedSources.FirstOrDefault(s => s.Contains("public partial class JsonContext"));
        Assert.NotNull(jsonContextSource);
        
        // Verify record creation and backing field access
        Assert.Contains("new BookRecord(string.Empty, string.Empty)", jsonContextSource);
        Assert.Contains("GetBookRecordTitleBackingField", jsonContextSource);
        Assert.Contains("GetBookRecordPublisherBackingField", jsonContextSource);
        
        // Verify backing field population
        Assert.Contains("GetBookRecordTitleBackingField(instance)", jsonContextSource);
        Assert.Contains("GetBookRecordPublisherBackingField(instance)", jsonContextSource);
    }

    [Fact]
    public void NoPartialMethods_ShouldNotGenerateImplementation()
    {
        // Arrange
        var sourceCode = """
            namespace TestNamespace
            {
                public class RegularClass
                {
                    public void RegularMethod() { }
                }
            }
            """;

        // Act
        var result = RunGenerator(sourceCode, Array.Empty<(string, string)>());

        // Assert
        // Should generate no files since there are no partial classes with partial methods
        Assert.Empty(result.GeneratedTrees);
    }

    [Fact]
    public void PartialClassWithoutPartialMethods_ShouldNotGenerateImplementation()
    {
        // Arrange
        var sourceCode = """
            namespace TestNamespace
            {
                public partial class PartialClassWithoutPartialMethods
                {
                    public void RegularMethod() { }
                }
            }
            """;

        // Act
        var result = RunGenerator(sourceCode, Array.Empty<(string, string)>());

        // Assert
        // Should generate no files since there are no partial methods returning IEnumerable<T>
        Assert.Empty(result.GeneratedTrees);
    }

    [Fact]
    public void PartialClassWithNonIEnumerablePartialMethod_ShouldNotGenerateImplementation()
    {
        // Arrange
        var sourceCode = """
            namespace TestNamespace
            {
                public partial class JsonContext
                {
                    public partial string GetSingleItem(string jsonFile);
                }
            }
            """;

        // Act
        var result = RunGenerator(sourceCode, Array.Empty<(string, string)>());

        // Assert
        // Should generate no files since the partial method doesn't return IEnumerable<T>
        Assert.Empty(result.GeneratedTrees);
    }

    private GeneratorDriverRunResult RunGenerator(string sourceCode, (string fileName, string content)[] additionalFiles)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Text.Json.JsonSerializer).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Runtime.CompilerServices.UnsafeAccessorAttribute).Assembly.Location)
        };

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new UnsafeAccessorGeneratorImpl();
        var driver = CSharpGeneratorDriver.Create(generator);

        // Add additional files
        var additionalTexts = additionalFiles
            .Select(f => (AdditionalText)new TestAdditionalText(f.fileName, f.content))
            .ToArray();

        if (additionalTexts.Length > 0)
        {
            driver = (CSharpGeneratorDriver)driver.AddAdditionalTexts(ImmutableArray.CreateRange(additionalTexts));
        }

        return driver.RunGenerators(compilation).GetRunResult();
    }

    private class TestAdditionalText : AdditionalText
    {
        private readonly string _text;

        public TestAdditionalText(string path, string text)
        {
            Path = path;
            _text = text;
        }

        public override string Path { get; }

        public override SourceText? GetText(CancellationToken cancellationToken = default)
        {
            return SourceText.From(_text);
        }
    }
}