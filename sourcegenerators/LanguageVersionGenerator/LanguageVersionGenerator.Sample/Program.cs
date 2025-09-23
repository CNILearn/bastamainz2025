using Generated;

// don't remove these usings to allow C# < 10.0
using System;
using System.Linq;

// don't change to file-scoped namespace to allow using C# < 10.0
namespace LanguageVersionGenerator.Sample
{

    /// <summary>
    /// Sample program demonstrating language version-aware source generation
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Language Version Generator Demo ===");
            Console.WriteLine();

            // Display detected language version and feature support
            Console.WriteLine("Language Version Information:");
            Console.WriteLine($"Detected Version: {LanguageVersionInfo.LanguageVersion}");
            Console.WriteLine();

            Console.WriteLine("Feature Support Details:");
            Console.WriteLine(LanguageVersionInfo.GetFeatureSupport());
            Console.WriteLine();

            // Demonstrate Person class usage (varies by language version)
            Console.WriteLine("=== Person Class Demo ===");

            // Create person - the syntax will depend on the generated code structure
            // For C# 12+ with primary constructors: new Person("John Doe", 30) { Email = "john@example.com" }
            // For C# 9+ records: new Person { Name = "John Doe", Age = 30, Email = "john@example.com" }  
            // For traditional classes: new Person { Name = "John Doe", Age = 30, Email = "john@example.com" }
            var person = CreatePersonForDemo();

            Console.WriteLine($"Person: {person.Name}, Age: {person.Age}, Email: {person.Email}");
            Console.WriteLine();

            // Demonstrate DataProcessor with language version-specific features
            Console.WriteLine("=== Data Processing Demo ===");
            var processor = new DataProcessor();

            Console.WriteLine("Processing null input:");
            var result1 = processor.ProcessData(null);
            Console.WriteLine($"Result: {result1}");

            Console.WriteLine("Processing normal input:");
            var result2 = processor.ProcessData("Hello World");
            Console.WriteLine($"Result: {result2}");

            // Demonstrate file processing (shows using declaration vs traditional using)
            Console.WriteLine("Processing file (demonstrating resource management):");
            processor.ProcessFile("sample.txt");
           
            Console.WriteLine($"Items in processor: [{string.Join(", ", processor.Items.Select(i => $"\"{i}\""))}]");
            Console.WriteLine();

            // Demonstrate utility methods
            Console.WriteLine("=== Utility Methods Demo ===");
            var numbers = LanguageUtilities.CreateList(1, 2, 3, 4, 5);
            Console.WriteLine($"Created list: [{string.Join(", ", numbers)}]");

            var message = LanguageUtilities.FormatMessage("Alice", numbers.Count);
            Console.WriteLine($"Formatted message: {message}");
            Console.WriteLine();

            Console.WriteLine("=== Generation Summary ===");
            Console.WriteLine("This sample demonstrates how ParseOptionsProvider enables source generators");
            Console.WriteLine("to detect the C# language version and generate appropriate code:");
            Console.WriteLine("- Different class structures based on records support");
            Console.WriteLine("- File-scoped vs traditional namespaces");
            Console.WriteLine("- Collection expressions vs traditional initialization");
            Console.WriteLine("- Using declarations vs traditional using statements");
            Console.WriteLine("- Null-coalescing assignment vs traditional null checks");
            Console.WriteLine();
            Console.WriteLine("Try changing the <LangVersion> property in the .csproj file");
            Console.WriteLine("and rebuilding to see different generated code!");
        }

        /// <summary>
        /// Creates a Person instance using reflection to handle different generated code structures
        /// </summary>
        private static Person CreatePersonForDemo()
        {
            var personType = typeof(Person);
            var constructors = personType.GetConstructors();

            // Check if we have a primary constructor (C# 12+ with parameters)
            var primaryConstructor = constructors.FirstOrDefault(c => c.GetParameters().Length > 0);
            if (primaryConstructor != null)
            {
                // Use primary constructor
                var instance = (Person)primaryConstructor.Invoke(new object[] { "John Doe", 30 });

                // Set Email property if it exists and is settable
                var emailProperty = personType.GetProperty("Email");
                if (emailProperty != null && emailProperty.CanWrite)
                {
                    emailProperty.SetValue(instance, "john@example.com");
                }

                return instance;
            }
            else
            {
                // Use parameterless constructor and set properties
                var instance = (Person)Activator.CreateInstance(personType)!;

                var nameProperty = personType.GetProperty("Name");
                var ageProperty = personType.GetProperty("Age");
                var emailProperty = personType.GetProperty("Email");

                if (nameProperty != null && nameProperty.CanWrite)
                    nameProperty.SetValue(instance, "John Doe");
                if (ageProperty != null && ageProperty.CanWrite)
                    ageProperty.SetValue(instance, 30);
                if (emailProperty != null && emailProperty.CanWrite)
                    emailProperty.SetValue(instance, "john@example.com");

                return instance;
            }
        }
    }
}