namespace HelloWorldGenerator.Tests;

public class HelloWorldGeneratorTests
{
    [Fact]
    public Task GeneratesHelloWorldForEmptyClass()
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

        return TestHelper.VerifyGenerator(source);
    }

    [Fact]
    public Task GeneratesHelloWorldForClassWithMethods()
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

        return TestHelper.VerifyGenerator(source);
    }

    [Fact]
    public Task GeneratesHelloWorldForMultipleNamespaces()
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

        return TestHelper.VerifyGenerator(source);
    }

    [Fact]
    public Task GeneratesHelloWorldForMinimalProgram()
    {
        var source = """
            using System;

            Console.WriteLine("Hello");
            """;

        return TestHelper.VerifyGenerator(source);
    }

    [Fact]
    public Task GeneratesHelloWorldForNoUserTypes()
    {
        var source = """
            // Just a comment, no types
            """;

        return TestHelper.VerifyGenerator(source);
    }

    [Fact]
    public Task GeneratesHelloWorldWithDifferentAccessModifiers()
    {
        var source = """
            using System;

            namespace TestNamespace;

            public class PublicClass
            {
                public void PublicMethod() { }
                private void PrivateMethod() { }
                internal void InternalMethod() { }
                protected void ProtectedMethod() { }
            }

            internal class InternalClass
            {
                public void PublicMethodInInternalClass() { }
            }

            public abstract class AbstractClass
            {
                public abstract void AbstractMethod();
                public virtual void VirtualMethod() { }
            }
            """;

        return TestHelper.VerifyGenerator(source);
    }

    [Fact]
    public Task GeneratesHelloWorldForGenericTypes()
    {
        var source = """
            using System;
            using System.Collections.Generic;

            namespace TestNamespace;

            public class GenericClass<T>
            {
                public T ProcessItem(T item) { return item; }
            }

            public class ConstrainedGeneric<T> where T : class, new()
            {
                public T CreateNew() { return new T(); }
            }

            public interface IGenericInterface<TInput, TOutput>
            {
                TOutput Process(TInput input);
            }
            """;

        return TestHelper.VerifyGenerator(source);
    }
}