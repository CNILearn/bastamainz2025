using Xunit;

namespace Stage5.Optimized.Tests;

public class OptimizedDataSourceGeneratorTests
{
    [Fact]
    public void Generator_WithBasicClass_GeneratesCode()
    {
        var source = """
            using Stage5.Optimized.Attributes;

            namespace TestNamespace;

            [DataSource(EntityName = "TestEntity", Count = 5)]
            public class TestEntity
            {
                public string Name { get; set; }
                public string Value { get; set; }
                public int Number { get; set; }
            }
            """;

        var configJson = """
            {
              "entityType": "TestEntity",
              "defaultCount": 5,
              "templates": {
                "Name": ["Test1", "Test2", "Test3"],
                "Value": ["Value1", "Value2", "Value3"]
              }
            }
            """;

        var (generatedSource, diagnostics) = TestHelper.RunGenerator(source, ("TestEntity.datasource.json", configJson));

        Assert.Empty(diagnostics);
        Assert.Contains("TestEntityDataGenerator", generatedSource);
        Assert.Contains("Stage 5: Optimized", generatedSource);
        Assert.Contains("GenerateData", generatedSource);
        Assert.Contains("CreateSample", generatedSource);
    }

    [Fact]
    public void Generator_WithMultipleClasses_GeneratesMultipleFiles()
    {
        var source = """
            using Stage5.Optimized.Attributes;

            namespace TestNamespace;

            [DataSource(EntityName = "User", Count = 10)]
            public class User
            {
                public string Name { get; set; }
                public string Email { get; set; }
            }

            [DataSource(EntityName = "Product", Count = 15)]
            public class Product
            {
                public string Name { get; set; }
                public decimal Price { get; set; }
            }
            """;

        var (generatedSource, diagnostics) = TestHelper.RunGenerator(source);

        Assert.Empty(diagnostics);
        Assert.Contains("UserDataGenerator", generatedSource);
        Assert.Contains("ProductDataGenerator", generatedSource);
    }

    [Fact]
    public void Generator_WithAdvancedConfiguration_HandlesExternalDependencies()
    {
        var source = """
            using Stage5.Optimized.Attributes;

            namespace TestNamespace;

            [DataSource(EntityName = "AdvancedEntity")]
            public class AdvancedEntity
            {
                public string Name { get; set; }
                public bool IsActive { get; set; }
            }
            """;

        var configJson = """
            {
              "entityType": "AdvancedEntity",
              "defaultCount": 20,
              "templates": {
                "Name": ["Advanced1", "Advanced2"]
              },
              "dependencies": ["ExternalService"],
              "externalDependencies": {
                "apiEndpoint": "https://api.example.com",
                "service": "ExternalService"
              }
            }
            """;

        var (generatedSource, diagnostics) = TestHelper.RunGenerator(source, ("AdvancedEntity.datasource.json", configJson));

        Assert.Empty(diagnostics);
        Assert.Contains("AdvancedEntityDataGenerator", generatedSource);
        Assert.Contains("GetAdvancedCachingStats", generatedSource);
        Assert.Contains("GetCachePerformanceMetrics", generatedSource);
    }

    [Fact]
    public void CacheKey_WithSameValues_AreEqual()
    {
        var key1 = new CacheKey("TestEntity", "test.json", 123, 456789, "1.0");
        var key2 = new CacheKey("TestEntity", "test.json", 123, 456789, "1.0");

        Assert.Equal(key1, key2);
        Assert.Equal(key1.GetHashCode(), key2.GetHashCode());
    }

    [Fact]
    public void CacheKey_WithDifferentValues_AreNotEqual()
    {
        var key1 = new CacheKey("TestEntity", "test.json", 123, 456789, "1.0");
        var key2 = new CacheKey("TestEntity", "test.json", 124, 456789, "1.0");

        Assert.NotEqual(key1, key2);
    }

    [Fact]
    public async Task OptimizedCacheManager_StoresAndRetrievesValues()
    {
        var cacheManager = new OptimizedCacheManager();
        var key = new CacheKey("TestEntity", "test.json", 123, 456789, "1.0");
        var value = "Test cached value";

        await cacheManager.SetAsync(key, value);
        var retrieved = await cacheManager.GetAsync<string>(key);

        Assert.Equal(value, retrieved);
    }

    [Fact]
    public void ChangeDetectionEngine_DetectsContentChanges()
    {
        var engine = new OptimizedChangeDetectionEngine();
        var identifier = "test.json";
        var originalContent = "original content";
        var changedContent = "changed content";

        engine.TrackContent(identifier, originalContent);
        
        var hasChanged = engine.HasContentChanged(identifier, changedContent);
        
        Assert.True(hasChanged);
    }

    [Fact]
    public void ChangeDetectionEngine_DoesNotDetectSameContent()
    {
        var engine = new OptimizedChangeDetectionEngine();
        var identifier = "test.json";
        var content = "same content";

        engine.TrackContent(identifier, content);
        
        var hasChanged = engine.HasContentChanged(identifier, content);
        
        Assert.False(hasChanged);
    }
}