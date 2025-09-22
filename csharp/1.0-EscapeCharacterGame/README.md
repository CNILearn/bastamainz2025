# C# 13 \e Escape Character Console Game Sample

This sample demonstrates the concept of C# 13's **`\e` escape character** through an interactive console trivia game. The game showcases rich console formatting using ANSI escape sequences and explains how the `\e` escape character simplifies and improvese console application development.

## 🚀 Features

- **Interactive Trivia Game**: Educational quiz about escape characters and ANSI sequences
- **Rich Console Formatting**: Colorful text, bold/underline formatting, screen clearing
- **C# 13 Concept Demonstration**: Shows current methods vs. future `\e` syntax
- **Educational Content**: Comprehensive explanation of escape character benefits
- **Practical Examples**: Real-world use cases for ANSI escape sequences

## 📋 Prerequisites

- **.NET 9 SDK** or later
- **Visual Studio 2022** or **VS Code** with C# extension  
- **C# 13 Language Support** (LangVersion=13.0)
- **Terminal with ANSI support** (Windows Terminal, macOS Terminal, Linux terminals)

## 🏗️ Architecture

```
EscapeCharacterGame/
├── Program.cs                        # Main game implementation with trivia logic
├── EscapeCharacterGame.csproj       # Project file targeting .NET 10
└── README.md                        # This documentation
```

## 🔧 Setup

### 1. Navigate to Project Directory

```bash
cd Dotnet10Samples/src/EscapeCharacterGame
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Project

```bash
dotnet build
```

### 4. Run the Game

```bash
dotnet run
```

## 💡 Key Concepts

### The \e Escape Character

The `\e` escape character is implemented with C# 13 and represents the ESC character (ASCII 27). It serves as the foundation for ANSI escape sequences used in terminal control and formatting.

#### Current Approach (C# 12 and earlier)
```csharp
// Multiple ways to represent ESC character
string ESC = "\x1b";                    // Hexadecimal representation
string ESC = ((char)27).ToString();     // ASCII code conversion
string ESC = "\u001b";                  // Unicode escape sequence

// Building ANSI escape sequences
string RED = $"{ESC}[31m";              // Requires string concatenation
string RESET = $"{ESC}[0m";             // More verbose syntax
string CLEAR = $"{ESC}[2J{ESC}[H";      // Complex sequences need multiple concatenations
```

#### With \e Escape Character (C# 13+)
```csharp
// Direct, readable syntax
string RED = "\e[31m";                   // Clean and intuitive
string RESET = "\e[0m";                  // Consistent with other escapes
string CLEAR = "\e[2J\e[H";              // Simplified complex sequences

// Inline usage becomes much cleaner
Console.WriteLine("\e[32mSuccess!\e[0m");                    // vs
Console.WriteLine($"{ESC}[32mSuccess!{ESC}[0m");             // before
```

### ANSI Escape Sequences Demonstrated

The game showcases various ANSI escape sequences:

| Sequence | Purpose | Example |
|----------|---------|---------|
| `\e[31m` | Red text | Error messages |
| `\e[32m` | Green text | Success indicators |
| `\e[33m` | Yellow text | Warnings |
| `\e[34m` | Blue text | Information |
| `\e[1m` | Bold text | Headers and emphasis |
| `\e[4m` | Underlined text | Important content |
| `\e[2J\e[H` | Clear screen | Game state transitions |
| `\e[0m` | Reset formatting | Return to default |

## 🎮 Game Features

### 1. Interactive Trivia Questions
- 5 educational questions about escape characters and ANSI sequences
- Multiple choice format with color-coded options
- Immediate feedback with explanations

### 2. Rich Visual Feedback
- **Color-coded questions**: Different colors for each answer option
- **Success animations**: Green celebration emojis for correct answers
- **Failure indicators**: Red visual feedback for incorrect answers
- **Score tracking**: Real-time score display with conditional formatting

### 3. Educational Content
- **Code comparisons**: Side-by-side current vs. future syntax
- **Benefit explanations**: Why `\e` improves developer experience
- **Real-world applications**: Practical use cases for console formatting

## 🔍 Sample Output

```
🎮 ESCAPE CHARACTER TRIVIA GAME 🎮
============================================

Welcome to the Escape Character Trivia Game!
This game demonstrates rich console formatting using ANSI escape sequences.

🎯 Features demonstrated:
  • Colorful text output
  • Text formatting (bold, underline)
  • Screen clearing and cursor positioning
  • Interactive console application

Question 1:
==================================================

What ASCII code does the ESC character represent?

A. 26
B. 27
C. 28
D. 29

Your answer (A-D): B

✓ Correct! The ESC character has ASCII code 27, which is used as the foundation for ANSI escape sequences.
🎉 🎉 🎉
```

## 🌟 Benefits of \e Escape Character

### 1. **Improved Readability**
- **Before**: `string RED = $"{ESC}[31m";`
- **After**: `string RED = "\e[31m";`

### 2. **Reduced Complexity**
- **Before**: `Console.WriteLine($"{ESC}[32m{message}{ESC}[0m");`
- **After**: `Console.WriteLine($"\e[32m{message}\e[0m");`

### 3. **Better Performance**
- No string concatenation overhead
- Compile-time string literal optimization
- Reduced memory allocations

### 4. **Enhanced Developer Experience**
- IntelliSense support for escape sequences
- Syntax highlighting improvements
- Easier debugging and maintenance

### 5. **Consistency**
- Aligns with existing escape characters (`\n`, `\t`, `\r`, `\"`)
- Follows established C# string literal conventions
- Maintains backwards compatibility

## 🎯 Real-World Applications

### 1. Command-Line Tools
```csharp
// Progress indicators
Console.Write($"\e[32m{percentage:F1}%\e[0m");

// Status messages
Console.WriteLine($"\e[33mWarning:\e[0m Configuration file not found");
```

### 2. Development Debugging
```csharp
// Highlighted debug output
Debug.WriteLine($"\e[35m[DEBUG]\e[0m Variable x = {x}");

// Error highlighting in logs
Logger.Error($"\e[31mFatal error:\e[0m {exception.Message}");
```

### 3. Interactive Applications
```csharp
// Menu highlighting
Console.WriteLine($"\e[1m\e[36mSelect an option:\e[0m");

// Input validation feedback
Console.WriteLine($"\e[31mInvalid input. Please try again.\e[0m");
```

### 4. Terminal Games
```csharp
// Game state display
Console.WriteLine($"\e[2J\e[H\e[1mScore: \e[32m{score}\e[0m");

// Player feedback
Console.WriteLine($"\e[92m🎉 Level Complete!\e[0m");
```

## 🔬 Technical Details

### Project Configuration
- **Target Framework**: `net10.0`
- **Language Version**: `preview` (enables C# 14 features)
- **Output Type**: Console application
- **Nullable**: Enabled for better code safety

### Compatibility Notes
- The `\e` escape character is **not yet available** in .NET 10 RC 1
- This sample demonstrates the concept using current methods (`\x1b`, `(char)27`)
- Code includes comments showing how `\e` would improve the implementation
- Sample will work with current .NET versions and be ready for C# 13

### Cross-Platform Support
- **Windows**: Requires Windows Terminal or compatible terminal
- **macOS**: Works with Terminal.app and iTerm2
- **Linux**: Compatible with most modern terminal emulators
- **VS Code**: Integrated terminal supports ANSI sequences

## 🧪 Testing

### Manual Testing
1. Run the application: `dotnet run`
2. Play through all trivia questions
3. Verify color output in terminal
4. Check educational content display

### Automated Testing
```bash
# Test with automated input
echo -e "\nB\n\nC\n\nA\n\nC\n\nB\n" | dotnet run
```

## 🚀 Future Enhancements

When C# 13 and `\e` escape character become available:

1. **Simplified Constants**:
   ```csharp
   // Replace all instances of "\x1b" with "\e"
   private static readonly string RED = "\e[31m";
   private static readonly string GREEN = "\e[32m";
   ```

2. **Inline Usage**:
   ```csharp
   // Direct inline usage becomes cleaner
   Console.WriteLine("\e[32mSuccess!\e[0m");
   ```

3. **Enhanced Readability**:
   ```csharp
   // Complex sequences become more readable
   Console.Write("\e[2J\e[H\e[1m\e[36mWelcome!\e[0m");
   ```

## 📖 Educational Value

This sample serves as both a practical demonstration and educational tool:

- **Concept Introduction**: Introduces the upcoming `\e` escape character
- **Current Alternatives**: Shows existing methods for ANSI sequences
- **Migration Path**: Demonstrates how code will improve with C# 13
- **Best Practices**: Exhibits proper console application patterns
- **Real-World Context**: Provides practical use cases and benefits

## 🔗 Related Resources

- [ANSI Escape Codes Documentation](https://en.wikipedia.org/wiki/ANSI_escape_code)
- [C# String Escape Sequences](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/strings/#string-escape-sequences)
- [Console Class Documentation](https://docs.microsoft.com/en-us/dotnet/api/system.console)
- [Terminal Color and Formatting Guide](https://misc.flogisoft.com/bash/tip_colors_and_formatting)

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](../../LICENSE) file for details.

---

**Note**: This sample demonstrates the C# 13 `\e` escape character concept using current C# syntax. The actual `\e` escape character will be available when C# 13 is released. The game provides both educational content and practical examples of ANSI escape sequence usage in console applications.
