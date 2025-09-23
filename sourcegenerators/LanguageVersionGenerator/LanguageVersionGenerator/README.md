# Language Version Generator

A source generator that demonstrates how to use `ParseOptionsProvider` to detect the C# language version and generate different code based on the language features available.

## Overview

This source generator showcases how to create language version-aware source generators that emit different code depending on the detected C# language version. It demonstrates the use of `ParseOptionsProvider` to access parse options and determine which language features are available.

## Key Features

- **ParseOptionsProvider Usage**: Demonstrates how to access and use parse options in source generators
- **Language Version Detection**: Detects the C# language version at compile time
- **Feature-Aware Code Generation**: Generates different code based on available language features
- **Comprehensive Examples**: Shows how different language versions affect generated code

## How It Works

### ParseOptionsProvider

The generator uses `ParseOptionsProvider` to access the compilation's parse options:

```csharp
public void Initialize(IncrementalGeneratorInitializationContext context)
{
    // Use ParseOptionsProvider to get language version information
    var parseOptionsProvider = context.ParseOptionsProvider;
    
    // Transform parse options to extract language version
    var languageVersionProvider = parseOptionsProvider
        .Select(static (parseOptions, ct) => GetLanguageVersionInfo(parseOptions));

    // Register source generation based on language version
    context.RegisterSourceOutput(languageVersionProvider, static (spc, languageInfo) => 
        Execute(spc, languageInfo));
}
```

### Language Version Detection

The generator extracts language version information and determines feature support:

```csharp
private static LanguageVersionInfo GetLanguageVersionInfo(ParseOptions parseOptions)
{
    if (parseOptions is CSharpParseOptions csharpOptions)
    {
        var langVersion = csharpOptions.LanguageVersion;
        return new LanguageVersionInfo(
            LanguageVersion: langVersion,
            SupportsRecords: langVersion >= LanguageVersion.CSharp9,
            SupportsFileScopedNamespaces: langVersion >= LanguageVersion.CSharp10,
            SupportsPrimaryConstructors: langVersion >= LanguageVersion.CSharp12,
            // ... other feature checks
        );
    }
    // ... fallback logic
}
```

## Generated Code Examples

The generator produces different code based on the detected language version:

### C# 8.0 Features
- Traditional using statements
- Null-coalescing assignment (`??=`)
- Using declarations

### C# 9.0 Features
- Records instead of classes
- Top-level programs support detection

### C# 10.0 Features
- File-scoped namespaces (`namespace MyNamespace;`)
- Global using directives

### C# 11.0 Features
- Required members
- Generic attributes

### C# 12.0 Features
- Primary constructors for records
- Collection expressions (`[]`)

## Example Output

### C# 8.0 Output
```csharp
namespace Generated
{
    public class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
    }
    
    // Traditional using statement
    using (var reader = new StringReader("data"))
    {
        // ...
    }
}
```

### C# 12.0 Output
```csharp
namespace Generated;

public record Person(string Name, int Age)
{
    public string Email { get; init; } = string.Empty;
}

public class DataProcessor
{
    public List<string> Items { get; } = []; // Collection expression
    
    // Using declaration
    using var reader = new StringReader("data");
}
```

## Usage

1. Add the generator as an analyzer reference:
```xml
<ProjectReference Include="LanguageVersionGenerator.csproj" 
                  OutputItemType="Analyzer" 
                  ReferenceOutputAssembly="false" />
```

2. Set your desired C# language version:
```xml
<LangVersion>12.0</LangVersion>
```

3. The generator will automatically create language version-appropriate code.

## Testing Different Language Versions

To see how different language versions affect code generation:

1. Change the `<LangVersion>` property in your project file
2. Rebuild the project
3. Examine the generated code in the `Generated` namespace

Supported language versions for testing:
- `8.0` - Using declarations, null-coalescing assignment
- `9.0` - Records, top-level programs
- `10.0` - File-scoped namespaces, global usings
- `11.0` - Required members, generic attributes
- `12.0` - Primary constructors, collection expressions
- `latest` - Latest available features

## Use Cases

This pattern is valuable for:

- **Library Authors**: Creating source generators that work across different C# versions
- **Migration Tools**: Generating code that gradually adopts new language features
- **Educational Tools**: Demonstrating language evolution and feature adoption
- **Compatibility**: Ensuring generated code is compatible with the target language version

## Technical Details

### ParseOptionsProvider Benefits

Using `ParseOptionsProvider` provides several advantages:

1. **Incremental Generation**: Only regenerates when parse options change
2. **Language Awareness**: Access to language version and feature flags
3. **Compiler Integration**: Direct access to the same options the compiler uses
4. **Preprocessing Symbols**: Access to conditional compilation symbols

### Feature Detection Pattern

The generator uses a systematic approach to feature detection:

```csharp
SupportsFeature: langVersion >= LanguageVersion.CSharpX
```

This ensures that features are only used when the language version supports them.

### Code Generation Strategy

The generator uses conditional code generation:

```csharp
var namespaceDeclaration = info.SupportsFileScopedNamespaces 
    ? "namespace Generated;"
    : "namespace Generated\n{";
```

This allows generating appropriate syntax for each language version.

## Build Requirements

- .NET 9.0 SDK
- C# language version support in your project

## Contributing

This generator demonstrates advanced source generator patterns including:
- ParseOptionsProvider usage
- Language version detection
- Conditional code generation
- Feature-aware source generation

Perfect for learning about language version-aware source generators and ParseOptionsProvider!