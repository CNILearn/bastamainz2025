using Xunit;

namespace HelloWorldGenerator.Tests;

public class HelloWorldGeneratorTests
{
    [Fact]
    public void GeneratesHelloWorldForEmptyClass()
    {
        var source = """
            namespace TestNamespace;

            public class TestClass
            {
                public void SomeMethod()
                {
                    // Empty method
                }
            }
            """;

        var (generatedSource, diagnostics) = TestHelper.RunGenerator(source);
        
        // Verify no diagnostics/errors
        Assert.Empty(diagnostics);
        
        // Verify the generated source contains expected elements
        Assert.Contains("public static class GeneratedHelloWorld", generatedSource);
        Assert.Contains("public static string SayHello()", generatedSource);
        Assert.Contains("public static string GetAvailableTypesAndMethods()", generatedSource);
        Assert.Contains("public static string[] GetTypeNames()", generatedSource);
        Assert.Contains("return \"Hello, World!\";", generatedSource);
        Assert.Contains("TestNamespace.TestClass", generatedSource);
    }

    [Fact]
    public void GeneratesHelloWorldForClassWithMethods()
    {
        var source = """
            using System;
            using System.Collections.Generic;

            namespace TestNamespace;

            public class BusinessService
            {
                public string ProcessData(string input)
                {
                    return $"Processed: {input}";
                }

                public int Calculate(int value)
                {
                    return value * 2;
                }

                public void LogMessage(string message)
                {
                    Console.WriteLine(message);
                }
            }

            public class DataProcessor  
            {
                public void ProcessItems(List<string> items)
                {
                    foreach (var item in items)
                    {
                        Console.WriteLine(item);
                    }
                }
            }
            """;

        var (generatedSource, diagnostics) = TestHelper.RunGenerator(source);
        
        // Verify no diagnostics/errors
        Assert.Empty(diagnostics);
        
        // Verify the generated source contains expected elements
        Assert.Contains("public static class GeneratedHelloWorld", generatedSource);
        Assert.Contains("return \"Hello, World!\";", generatedSource);
        Assert.Contains("TestNamespace.BusinessService", generatedSource);
        Assert.Contains("TestNamespace.DataProcessor", generatedSource);
        Assert.Contains("Method: ProcessData", generatedSource);
        Assert.Contains("Method: Calculate", generatedSource);
    }

    [Fact]
    public void GeneratesHelloWorldForMultipleNamespaces()
    {
        var source = """
            using System;

            namespace FirstNamespace
            {
                public class FirstClass
                {
                    public void FirstMethod() { }
                }
            }

            namespace SecondNamespace
            {
                public class SecondClass
                {
                    public string SecondMethod(int param) 
                    { 
                        return param.ToString(); 
                    }
                }
            }
            """;

        var (generatedSource, diagnostics) = TestHelper.RunGenerator(source);
        
        // Verify no diagnostics/errors
        Assert.Empty(diagnostics);
        
        // Verify the generated source contains expected elements
        Assert.Contains("FirstNamespace.FirstClass", generatedSource);
        Assert.Contains("SecondNamespace.SecondClass", generatedSource);
        Assert.Contains("Method: FirstMethod", generatedSource);
        Assert.Contains("Method: SecondMethod", generatedSource);
    }

    [Fact]
    public void GeneratesHelloWorldForMinimalProgram()
    {
        var source = """
            using System;

            Console.WriteLine("Hello");
            """;

        var (generatedSource, diagnostics) = TestHelper.RunGenerator(source);
        
        // Verify no diagnostics/errors
        Assert.Empty(diagnostics);
        
        // Verify the basic structure is still generated even for minimal programs
        Assert.Contains("public static class GeneratedHelloWorld", generatedSource);
        Assert.Contains("return \"Hello, World!\";", generatedSource);
        Assert.Contains("GetAvailableTypesAndMethods", generatedSource);
        Assert.Contains("GetTypeNames", generatedSource);
    }
}