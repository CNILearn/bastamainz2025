using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace LanguageVersionGenerator.Tests;

public class LanguageVersionGeneratorTests
{
    [Fact]
    public void GeneratesCodeForCSharp8()
    {
        var source = """
            using System;
            
            public class TestClass { }
            """;

        var (generatedSource, diagnostics) = TestHelper.RunGenerator(source, LanguageVersion.CSharp8);
        
        // Verify no diagnostics/errors
        Assert.Empty(diagnostics);
        
        // Verify C# 8 specific features
        Assert.Contains("public class Person", generatedSource); // Should be class, not record
        Assert.Contains("namespace Generated\n{", generatedSource); // Traditional namespace
        Assert.Contains("new List<T>(items)", generatedSource); // Traditional collection init
        Assert.Contains("Global usings not supported", generatedSource); // Global usings comment
        Assert.Contains("Generic attributes not supported", generatedSource); // Generic attributes comment
        
        // Verify language version is detected correctly
        Assert.Contains("8.0", generatedSource);
        
        // Should NOT contain newer features
        Assert.DoesNotContain("record Person", generatedSource);
        Assert.DoesNotContain("namespace Generated;", generatedSource);
        Assert.DoesNotContain("= [];", generatedSource); // Collection expressions
        Assert.DoesNotContain("Person(string Name, int Age)", generatedSource); // Primary constructor
    }

    [Fact]
    public void GeneratesCodeForCSharp9()
    {
        var source = """
            using System;
            
            namespace TestNamespace;
            
            public class TestClass
            {
                public void TestMethod()
                {
                    Console.WriteLine("Test");
                }
            }
            """;

        var (generatedSource, diagnostics) = TestHelper.RunGenerator(source, LanguageVersion.CSharp9);
        
        // Verify no diagnostics/errors
        Assert.Empty(diagnostics);
        
        // Verify C# 9 specific features
        Assert.Contains("record Person", generatedSource); // Should be record
        Assert.Contains("using var", generatedSource); // Using declarations from C# 8
        Assert.Contains("namespace Generated\n{", generatedSource); // Still traditional namespace
        Assert.Contains("??=", generatedSource); // Null-coalescing assignment from C# 8
        
        // Verify language version is detected correctly
        Assert.Contains("9.0", generatedSource);
        
        // Should NOT contain newer features
        Assert.DoesNotContain("namespace Generated;", generatedSource); // File-scoped namespace is C# 10
        Assert.DoesNotContain("Person(string Name, int Age)", generatedSource); // Primary constructor is C# 12
    }

    [Fact]
    public void GeneratesCodeForCSharp10()
    {
        var source = """
            using System;
            
            namespace TestNamespace;
            
            public class TestClass
            {
                public void TestMethod()
                {
                    Console.WriteLine("Test");
                }
            }
            """;

        var (generatedSource, diagnostics) = TestHelper.RunGenerator(source, LanguageVersion.CSharp10);
        
        // Verify no diagnostics/errors
        Assert.Empty(diagnostics);
        
        // Verify C# 10 specific features
        Assert.Contains("namespace Generated;", generatedSource); // File-scoped namespace
        Assert.Contains("Global usings are supported", generatedSource);
        Assert.Contains("record Person", generatedSource); // Should be record
        Assert.Contains("using var", generatedSource); // Using declarations
        
        // Should NOT contain C# 10+ closing brace for namespace
        Assert.DoesNotContain("namespace Generated\n{", generatedSource);
        
        // Verify language version is detected correctly
        Assert.Contains("10.0", generatedSource);
    }

    [Fact]
    public void GeneratesCodeForCSharp12()
    {
        var source = """
            using System;
            
            namespace TestNamespace;
            
            public class TestClass
            {
                public void TestMethod()
                {
                    Console.WriteLine("Test");
                }
            }
            """;

        var (generatedSource, diagnostics) = TestHelper.RunGenerator(source, LanguageVersion.CSharp12);
        
        // Verify no diagnostics/errors
        Assert.Empty(diagnostics);
        
        // Verify C# 12 specific features
        Assert.Contains("namespace Generated;", generatedSource); // File-scoped namespace
        Assert.Contains("record Person(string Name, int Age)", generatedSource); // Primary constructor
        Assert.Contains("= [];", generatedSource); // Collection expressions
        Assert.Contains("[.. items]", generatedSource); // Collection expressions with spread
        Assert.Contains("??=", generatedSource); // Null-coalescing assignment
        Assert.Contains("using var", generatedSource); // Using declarations
        
        // Verify language version is detected correctly
        Assert.Contains("12.0", generatedSource);
    }

    [Fact]
    public void GeneratesCodeForLatestVersion()
    {
        var source = """
            using System;
            
            namespace TestNamespace;
            
            public class TestClass
            {
                public void TestMethod()
                {
                    Console.WriteLine("Test");
                }
            }
            """;

        var (generatedSource, diagnostics) = TestHelper.RunGenerator(source, LanguageVersion.Latest);
        
        // Verify no diagnostics/errors
        Assert.Empty(diagnostics);
        
        // Should contain the most modern features available
        Assert.Contains("namespace Generated;", generatedSource);
        Assert.Contains("record Person(string Name, int Age)", generatedSource);
        Assert.Contains("= [];", generatedSource);
        Assert.Contains("??=", generatedSource);
        Assert.Contains("using var", generatedSource);
    }

    [Fact]
    public void IncludesFeatureSupportInformation()
    {
        var source = """
            using System;
            
            public class TestClass { }
            """;

        var (generatedSource, diagnostics) = TestHelper.RunGenerator(source, LanguageVersion.CSharp12);
        
        // Verify no diagnostics/errors
        Assert.Empty(diagnostics);
        
        // Verify feature support information is included
        Assert.Contains("public static class LanguageVersionInfo", generatedSource);
        Assert.Contains("GetFeatureSupport", generatedSource);
        Assert.Contains("Records:", generatedSource);
        Assert.Contains("Primary constructors:", generatedSource);
        Assert.Contains("Collection expressions:", generatedSource);
        Assert.Contains("✓", generatedSource); // Should have checkmarks for supported features
    }

    [Fact]
    public void HandlesEmptySource()
    {
        var source = "";

        var (generatedSource, diagnostics) = TestHelper.RunGenerator(source, LanguageVersion.Latest);
        
        // Should still generate code even with empty source
        Assert.NotEmpty(generatedSource);
        Assert.Contains("namespace Generated;", generatedSource);
        Assert.Contains("LanguageVersionInfo", generatedSource);
    }

    [Fact]
    public void VerifyDifferentVersionsProduceDifferentOutput()
    {
        var source = """
            using System;
            
            public class TestClass { }
            """;

        var (csharp8Output, _) = TestHelper.RunGenerator(source, LanguageVersion.CSharp8);
        var (csharp12Output, _) = TestHelper.RunGenerator(source, LanguageVersion.CSharp12);
        
        // The outputs should be different
        Assert.NotEqual(csharp8Output, csharp12Output);
        
        // C# 8 should have traditional patterns
        Assert.Contains("public class Person", csharp8Output);
        Assert.Contains("namespace Generated\n{", csharp8Output);
        
        // C# 12 should have modern patterns
        Assert.Contains("record Person(string Name, int Age)", csharp12Output);
        Assert.Contains("namespace Generated;", csharp12Output);
    }
}