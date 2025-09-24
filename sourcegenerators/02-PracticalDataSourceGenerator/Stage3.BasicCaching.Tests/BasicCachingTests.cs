using Xunit;

namespace Stage3.BasicCaching.Tests;

public class BasicCachingDataSourceGeneratorTests
{
    [Fact]
    public void GeneratesDataFactoryForSimpleClassWithoutExternalFiles()
    {
        var source = """
            using Stage3.BasicCaching.Attributes;
            using System;

            namespace TestNamespace;

            [DataSource(EntityName = "User", Count = 5)]
            public class User
            {
                public string Name { get; set; }
                public int Age { get; set; }
                public bool IsActive { get; set; }
            }
            """;

        var (generatedSource, diagnostics) = TestHelper.RunGenerator(source);
        
        // Verify no diagnostics/errors
        Assert.Empty(diagnostics);
        
        // Verify the generated source contains expected elements
        Assert.Contains("public static class UserDataFactory", generatedSource);
        Assert.Contains("CreateSample()", generatedSource);
        Assert.Contains("CreateSamples(int count = 5)", generatedSource);
        Assert.Contains("GetGeneratorInfo()", generatedSource);
        Assert.Contains("GetConfigurationInfo()", generatedSource);
        Assert.Contains("GetCachingStats()", generatedSource);
        
        // Verify Stage 3 characteristics
        Assert.Contains("Stage 3: Basic Caching Data Source Generator", generatedSource);
        Assert.Contains("Cached configurations", generatedSource);
        Assert.Contains("No cached configuration found", generatedSource);
    }

    [Fact]
    public void GeneratesDataFactoryWithCachedExternalConfiguration()
    {
        var source = """
            using Stage3.BasicCaching.Attributes;
            using System;

            namespace TestNamespace;

            [DataSource(EntityName = "User", Count = 5)]
            public class User
            {
                public string Name { get; set; }
                public string Email { get; set; }
                public int Age { get; set; }
            }
            """;

        var configFile = """
            {
              "entityType": "User",
              "defaultCount": 7,
              "templates": {
                "Name": ["Alice", "Bob", "Carol"],
                "Email": ["alice@test.com", "bob@test.com", "carol@test.com"]
              }
            }
            """;

        var (generatedSource, diagnostics) = TestHelper.RunGenerator(source, 
            ("User.datasource.json", configFile));
        
        // Verify no diagnostics/errors
        Assert.Empty(diagnostics);
        
        // Verify cached configuration is used
        Assert.Contains("Cached configuration: User.datasource.json", generatedSource);
        Assert.Contains("Templates: 2", generatedSource);
        Assert.Contains("Hash:", generatedSource);
        
        // Verify template-based generation
        Assert.Contains("new[] { \"Alice\", \"Bob\", \"Carol\" }", generatedSource);
        Assert.Contains("new[] { \"alice@test.com\", \"bob@test.com\", \"carol@test.com\" }", generatedSource);
        
        // Verify caching functionality
        Assert.Contains("GetCachingStats()", generatedSource);
        Assert.Contains("Caching: Configuration parsed once and reused until file changes detected", generatedSource);
    }

    [Fact]
    public void HandlesCachingWithMultipleClassesAndConfigurations()
    {
        var source = """
            using Stage3.BasicCaching.Attributes;

            namespace TestNamespace;

            [DataSource(EntityName = "User")]
            public class User
            {
                public string Name { get; set; }
            }

            [DataSource(EntityName = "Product")]
            public class Product
            {
                public string Name { get; set; }
                public decimal Price { get; set; }
            }
            """;

        var userConfig = """
            {
              "entityType": "User",
              "templates": {
                "Name": ["Alice", "Bob"]
              }
            }
            """;

        var productConfig = """
            {
              "entityType": "Product",
              "templates": {
                "Name": ["Laptop", "Phone"]
              }
            }
            """;

        var (generatedSource, diagnostics) = TestHelper.RunGenerator(source, 
            ("User.datasource.json", userConfig),
            ("Product.datasource.json", productConfig));
        
        // Verify no diagnostics/errors
        Assert.Empty(diagnostics);
        
        // Verify both factories are generated
        Assert.Contains("public static class UserDataFactory", generatedSource);
        Assert.Contains("public static class ProductDataFactory", generatedSource);
        
        // Verify different cached configurations are applied
        Assert.Contains("new[] { \"Alice\", \"Bob\" }", generatedSource);
        Assert.Contains("new[] { \"Laptop\", \"Phone\" }", generatedSource);
        
        // Verify multiple cached configurations
        Assert.Contains("Cached configuration: User.datasource.json", generatedSource);
        Assert.Contains("Cached configuration: Product.datasource.json", generatedSource);
    }

    [Fact]
    public void GeneratesAttributeSourceWithCachingMetadata()
    {
        var source = """
            using System;

            namespace TestNamespace;

            public class EmptyClass
            {
            }
            """;

        var (generatedSource, diagnostics) = TestHelper.RunGenerator(source);
        
        // Verify no diagnostics/errors
        Assert.Empty(diagnostics);
        
        // Verify the DataSource attribute is generated with proper metadata
        Assert.Contains("DataSourceAttribute", generatedSource);
        Assert.Contains("EntityName { get; set; }", generatedSource);
        Assert.Contains("Count { get; set; }", generatedSource);
        Assert.Contains("ConfigurationFile { get; set; }", generatedSource);
        Assert.Contains("Stage3.BasicCaching.Attributes", generatedSource);
        Assert.Contains("Stage 3: Basic caching with improved performance", generatedSource);
    }

    [Fact]
    public void CachingOptimizesFileProcessing()
    {
        var source = """
            using Stage3.BasicCaching.Attributes;

            namespace TestNamespace;

            [DataSource]
            public class TestClass
            {
                public string Name { get; set; }
            }
            """;

        var configFile = """
            {
              "entityType": "TestClass",
              "defaultCount": 15,
              "templates": {
                "Name": ["Test1", "Test2", "Test3"]
              }
            }
            """;

        var (generatedSource, diagnostics) = TestHelper.RunGenerator(source, 
            ("TestClass.datasource.json", configFile));
        
        // Verify no diagnostics/errors
        Assert.Empty(diagnostics);
        
        // Verify cached configuration information is included
        Assert.Contains("Cached configuration: TestClass.datasource.json", generatedSource);
        Assert.Contains("Templates: 1", generatedSource);
        
        // Verify caching-specific methods are generated
        Assert.Contains("GetCachingStats()", generatedSource);
        Assert.Contains("Caching: Configuration parsed once and reused", generatedSource);
        
        // Verify Stage 3 branding
        Assert.Contains("Stage 3: Basic caching", generatedSource);
        Assert.Contains("optimized file operations", generatedSource);
    }

    [Fact]
    public void IgnoresNonDataSourceJsonFilesInCaching()
    {
        var source = """
            using Stage3.BasicCaching.Attributes;

            namespace TestNamespace;

            [DataSource]
            public class TestClass
            {
                public string Name { get; set; }
            }
            """;

        var regularJsonFile = """
            {
              "someOtherConfig": "value"
            }
            """;

        var (generatedSource, diagnostics) = TestHelper.RunGenerator(source, 
            ("config.json", regularJsonFile), // Not a .datasource.json file
            ("other.config.json", regularJsonFile)); // Not a .datasource.json file
        
        // Verify no diagnostics/errors
        Assert.Empty(diagnostics);
        
        // Should generate code but without cached configuration
        Assert.Contains("public static class TestClassDataFactory", generatedSource);
        Assert.Contains("No cached configuration found", generatedSource);
        
        // Should still include caching infrastructure
        Assert.Contains("GetCachingStats()", generatedSource);
    }

    [Fact]
    public void CachingHandlesContentHashForChangeDetection()
    {
        var source = """
            using Stage3.BasicCaching.Attributes;

            namespace TestNamespace;

            [DataSource(EntityName = "User")]
            public class User
            {
                public string Name { get; set; }
                public int Id { get; set; }
            }
            """;

        var configFile = """
            {
              "entityType": "User",
              "defaultCount": 10,
              "templates": {
                "Name": ["John", "Jane", "Jack"]
              }
            }
            """;

        var (generatedSource, diagnostics) = TestHelper.RunGenerator(source, 
            ("User.datasource.json", configFile));
        
        // Verify no diagnostics/errors
        Assert.Empty(diagnostics);
        
        // Verify hash-based caching information is included in the output
        Assert.Contains("Hash:", generatedSource);
        Assert.Contains("Cached configuration: User.datasource.json", generatedSource);
        Assert.Contains("Templates: 1", generatedSource);
        
        // Verify the generated code indicates caching is active
        Assert.Contains("Caching enabled", generatedSource);
        Assert.Contains("content changes", generatedSource);
    }
}