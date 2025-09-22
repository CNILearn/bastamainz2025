namespace EscapeCharacterGame;

/// <summary>
/// Main game runner class that handles the escape character trivia game logic.
/// Demonstrates the concept of C# 13's \e escape character through a console trivia game.
/// This sample showcases ANSI escape sequences for rich console formatting and explains 
/// how the \e escape character would simplify and improve console application development.
/// 
/// Note: The \e escape character is implemented with C# 13.
/// This sample demonstrates the concept using existing methods (\x1b, (char)27) and shows
/// how \e would improve readability and maintainability.
/// </summary>
public class Runner
{
    // Color constants using the new \e escape character (C# 13)
    private static readonly string RESET = "\e[0m";
    private static readonly string RED = "\e[31m";
    private static readonly string GREEN = "\e[32m";
    private static readonly string YELLOW = "\e[33m";
    private static readonly string BLUE = "\e[34m";
    private static readonly string MAGENTA = "\e[35m";
    private static readonly string CYAN = "\e[36m";
    private static readonly string WHITE = "\e[37m";
    private static readonly string BRIGHT_RED = "\e[91m";
    private static readonly string BRIGHT_GREEN = "\e[92m";
    private static readonly string BRIGHT_YELLOW = "\e[93m";
    private static readonly string BRIGHT_BLUE = "\e[94m";
    private static readonly string BRIGHT_MAGENTA = "\e[95m";
    private static readonly string BRIGHT_CYAN = "\e[96m";
    
    // Formatting constants using \e
    private static readonly string BOLD = "\e[1m";
    private static readonly string UNDERLINE = "\e[4m";
    private static readonly string CLEAR_SCREEN = "\e[2J\e[H";
    private static readonly string DHTOP = "\e#3";
    private static readonly string DHBOT = "\e#4";
    private static readonly string SWSH = "\e#5";
    private static readonly string DWSH = "\e#6";
    
    // Game state
    private int score = 0;
    private int questionCount = 0;

    /// <summary>
    /// Runs the main game loop.
    /// </summary>
    public async Task RunAsync()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        ShowIntroduction();
        await Task.Delay(2000); // Give time to read
        
        await PlayTriviaGame();
        
        ShowResults();
        ShowEscapeCharacterExplanation();
    }
    
    private void ShowIntroduction()
    {
        Console.Write(CLEAR_SCREEN);
        
        // Title with colorful formatting
        Console.WriteLine($"{BRIGHT_BLUE}{BOLD}🎮 ESCAPE CHARACTER TRIVIA GAME 🎮{RESET}");
        Console.WriteLine($"{CYAN}============================================{RESET}");
        Console.WriteLine();
        
        Console.WriteLine($"{YELLOW}Welcome to the {BOLD}Escape Character Trivia Game{RESET}{YELLOW}!{RESET}");
        Console.WriteLine($"{WHITE}This game demonstrates rich console formatting using ANSI escape sequences.{RESET}");
        Console.WriteLine();
        
        Console.WriteLine($"{BRIGHT_GREEN}🎯 Features demonstrated:{RESET}");
        Console.WriteLine($"  {GREEN}•{RESET} Colorful text output");
        Console.WriteLine($"  {GREEN}•{RESET} Text formatting (bold, underline)");
        Console.WriteLine($"  {GREEN}•{RESET} Screen clearing and cursor positioning");
        Console.WriteLine($"  {GREEN}•{RESET} Interactive console application");
        Console.WriteLine();
        
        Console.WriteLine($"{MAGENTA}💡 About the {BOLD}\\e{RESET}{MAGENTA} escape character:{RESET}");
        Console.WriteLine($"{WHITE}   The {YELLOW}\\e{RESET}{WHITE} escape character (C# 13) represents ESC (ASCII 27){RESET}");
        Console.WriteLine($"{WHITE}   and makes ANSI escape sequences more readable and maintainable.{RESET}");
        Console.WriteLine();
        
        Console.WriteLine($"{BRIGHT_YELLOW}Press any key to start the game...{RESET}");
        
        // Handle both interactive and non-interactive modes
        try
        {
            Console.ReadKey(true);
        }
        catch (InvalidOperationException)
        {
            // Running in non-interactive mode (e.g., piped input)
            Console.ReadLine();
        }
    }
    
    private async Task PlayTriviaGame()
    {
        var questions = GetTriviaQuestions();
        
        foreach (var question in questions)
        {
            Console.Write(CLEAR_SCREEN);
            await AskQuestion(question);
            questionCount++;
            
            Console.WriteLine();
            Console.WriteLine($"{CYAN}Press any key to continue...{RESET}");
            try
            {
                Console.ReadKey(true);
            }
            catch (InvalidOperationException)
            {
                // Running in non-interactive mode
                Console.ReadLine();
            }
        }
    }
    
    private async Task AskQuestion(TriviaQuestion question)
    {
        // Question header with formatting
        Console.WriteLine($"{DHTOP}{BRIGHT_BLUE}{BOLD}Question {questionCount + 1}:{RESET}");
        Console.WriteLine($"{DHBOT}{BRIGHT_BLUE}{BOLD}Question {questionCount + 1}:{RESET}");
        Console.WriteLine($"{SWSH}");
        Console.WriteLine($"{CYAN}{new string('=', 50)}{RESET}");
        Console.WriteLine();
        
        // Display question with highlighting
        Console.WriteLine($"{WHITE}{question.Question}{RESET}");
        Console.WriteLine();
        
        // Display options with colors
        for (int i = 0; i < question.Options.Length; i++)
        {
            var optionColor = i switch
            {
                0 => BRIGHT_RED,
                1 => BRIGHT_GREEN,
                2 => BRIGHT_YELLOW,
                3 => BRIGHT_BLUE,
                _ => WHITE
            };
            
            Console.WriteLine($"{optionColor}{(char)('A' + i)}. {question.Options[i]}{RESET}");
        }

        Console.WriteLine();
        Console.Write($"{MAGENTA}Your answer (A-{(char)('A' + question.Options.Length - 1)}): {RESET}");
        
        // Get user input with validation
        char userAnswer;
        do
        {
            try
            {
                var key = Console.ReadKey(true);
                userAnswer = char.ToUpper(key.KeyChar);
            }
            catch (InvalidOperationException)
            {
                // Running in non-interactive mode
                var input = Console.ReadLine();
                userAnswer = !string.IsNullOrEmpty(input) ? char.ToUpper(input[0]) : 'A';
                break;
            }
        } while (userAnswer < 'A' || userAnswer > (char)('A' + question.Options.Length - 1));
        
        Console.WriteLine(userAnswer);
        Console.WriteLine();
        
        // Check answer and provide feedback
        bool isCorrect = userAnswer == question.CorrectAnswer;
        
        if (isCorrect)
        {
            score++;
            Console.WriteLine($"{BRIGHT_GREEN}{BOLD}✓ Correct!{RESET} {GREEN}{question.Explanation}{RESET}");
            await AnimateSuccess();
        }
        else
        {
            Console.WriteLine($"{BRIGHT_RED}{BOLD}✗ Incorrect.{RESET} {RED}The correct answer was {question.CorrectAnswer}.{RESET}");
            Console.WriteLine($"{YELLOW}{question.Explanation}{RESET}");
            await AnimateFailure();
        }
    }
    
    private static async Task AnimateSuccess()
    {
        Console.Beep();
        // Simple success animation using colors
        for (int i = 0; i < 3; i++)
        {
            Console.Write($"{BRIGHT_GREEN}🎉 ");
            await Task.Delay(200);
        }
        Console.WriteLine(RESET);
    }
    
    private static async Task AnimateFailure()
    {
        // Cross-platform failure sound - use simple beep fallback for non-Windows
        if (OperatingSystem.IsWindows())
        {
            // Play a sad/failure sound pattern on Windows - two lower pitched beeps
            Console.Beep(200, 300); // Lower frequency, longer duration
            await Task.Delay(100);
            Console.Beep(150, 400); // Even lower frequency, longer duration
        }
        else
        {
            // Fallback for non-Windows: multiple short beeps to indicate failure
            Console.Beep();
            await Task.Delay(100);
            Console.Beep();
        }
        
        // Simple failure animation
        for (int i = 0; i < 2; i++)
        {
            Console.Write($"{BRIGHT_RED}💔 ");
            await Task.Delay(300);
        }
        Console.WriteLine(RESET);
    }
    
    private void ShowResults()
    {
        Console.Write(CLEAR_SCREEN);
        
        // Results header
        Console.WriteLine($"{BRIGHT_BLUE}{BOLD}{UNDERLINE}🏆 GAME RESULTS 🏆{RESET}");
        Console.WriteLine();
        
        // Score display with conditional formatting
        var scoreColor = score >= questionCount * 0.8 ? BRIGHT_GREEN : 
                        score >= questionCount * 0.6 ? BRIGHT_YELLOW : BRIGHT_RED;
        
        Console.WriteLine($"{WHITE}Final Score: {scoreColor}{BOLD}{score}/{questionCount}{RESET}");
        
        // Performance message
        var percentage = (double)score / questionCount * 100;
        var message = percentage switch
        {
            >= 90 => $"{BRIGHT_GREEN}🌟 Outstanding! You're a trivia master!{RESET}",
            >= 80 => $"{GREEN}🎯 Excellent work! Great knowledge!{RESET}",
            >= 70 => $"{YELLOW}👍 Good job! You know your stuff!{RESET}",
            >= 60 => $"{YELLOW}📚 Not bad! Keep learning!{RESET}",
            _ => $"{RED}📖 Keep studying and try again!{RESET}"
        };
        
        Console.WriteLine($"Accuracy: {scoreColor}{percentage:F1}%{RESET}");
        Console.WriteLine();
        Console.WriteLine(message);
        Console.WriteLine();
    }
    
    private void ShowEscapeCharacterExplanation()
    {
        Console.WriteLine($"{CYAN}{new string('=', 60)}{RESET}");
        Console.WriteLine($"{BRIGHT_BLUE}{BOLD}💡 About the \\e Escape Character (C# 13 Feature){RESET}");
        Console.WriteLine($"{CYAN}{new string('=', 60)}{RESET}");
        Console.WriteLine();
        
        Console.WriteLine($"{WHITE}This game demonstrates the practical use of ANSI escape sequences{RESET}");
        Console.WriteLine($"{WHITE}using the new {YELLOW}\\e{RESET}{WHITE} escape character from C# 13!{RESET}");
        Console.WriteLine($"{WHITE}Notice how much cleaner and more readable the code becomes:{RESET}");
        Console.WriteLine();
        
        // Code comparison
        Console.WriteLine($"{BRIGHT_YELLOW}Previous approach (C# 12 and earlier):{RESET}");
        Console.WriteLine($"{CYAN}  string ESC = \"\\x1b\";              // Hexadecimal representation{RESET}");
        Console.WriteLine($"{CYAN}  string ESC = ((char)27).ToString(); // ASCII code conversion{RESET}");
        Console.WriteLine($"{CYAN}  string RED = $\"{{ESC}}[31m\";         // Concatenation required{RESET}");
        Console.WriteLine();
        
        Console.WriteLine($"{BRIGHT_GREEN}Current approach (C# 13 with \\e):{RESET}");
        Console.WriteLine($"{GREEN}  string RED = \"\\e[31m\";              // Direct, readable syntax{RESET}");
        Console.WriteLine($"{GREEN}  string RESET = \"\\e[0m\";             // Much cleaner!{RESET}");
        Console.WriteLine($"{GREEN}  Console.WriteLine(\"\\e[32mGreen!\\e[0m\"); // Inline usage{RESET}");
        Console.WriteLine();
        
        Console.WriteLine($"{BRIGHT_MAGENTA}Benefits of \\e demonstrated in this code:{RESET}");
        Console.WriteLine($"{MAGENTA}  ✓ More readable and intuitive syntax{RESET}");
        Console.WriteLine($"{MAGENTA}  ✓ Reduced string concatenation overhead{RESET}");
        Console.WriteLine($"{MAGENTA}  ✓ Better IDE support and IntelliSense{RESET}");
        Console.WriteLine($"{MAGENTA}  ✓ Consistent with other escape characters (\\n, \\t, \\r){RESET}");
        Console.WriteLine($"{MAGENTA}  ✓ Easier maintenance of console applications{RESET}");
        Console.WriteLine();
        
        Console.WriteLine($"{BRIGHT_CYAN}Real-world applications:{RESET}");
        Console.WriteLine($"{CYAN}  • Interactive command-line tools{RESET}");
        Console.WriteLine($"{CYAN}  • Development debugging output{RESET}");
        Console.WriteLine($"{CYAN}  • Terminal-based games and applications{RESET}");
        Console.WriteLine($"{CYAN}  • Log formatting and visualization{RESET}");
        Console.WriteLine($"{CYAN}  • Progress indicators and status displays{RESET}");
        Console.WriteLine();
        
        Console.WriteLine($"{WHITE}The {YELLOW}\\e{RESET}{WHITE} escape character represents the ESC character (ASCII 27){RESET}");
        Console.WriteLine($"{WHITE}and is the foundation of ANSI escape sequences for terminal control.{RESET}");
        Console.WriteLine();
        
        Console.WriteLine($"{BRIGHT_YELLOW}Thank you for playing! 🎮{RESET}");
    }
    
    private static TriviaQuestion[] GetTriviaQuestions()
    {
        return
        [
            new TriviaQuestion
            {
                Question = "What ASCII code does the ESC character represent?",
                Options = ["26", "27", "28", "29"],
                CorrectAnswer = 'B',
                Explanation = "The ESC character has ASCII code 27, which is used as the foundation for ANSI escape sequences."
            },
            
            new TriviaQuestion
            {
                Question = "Which C# version introduces the \\e escape character?",
                Options = ["C# 11", "C# 12", "C# 13", "C# 14"],
                CorrectAnswer = 'C',
                Explanation = "The \\e escape character is introduced in C# 13."
            },
            
            new TriviaQuestion
            {
                Question = "What does ANSI stand for in the context of escape sequences?",
                Options = ["American National Standards Institute", "Advanced Network System Interface", "Automatic Number System Integration", "Applied Numeric Sequence Identifier"],
                CorrectAnswer = 'A',
                Explanation = "ANSI stands for American National Standards Institute, which standardized these terminal control sequences."
            },
            
            new TriviaQuestion
            {
                Question = "Which current C# escape sequence is equivalent to the new \\e?",
                Options = ["\\n", "\\t", "\\x1b", "\\r"],
                CorrectAnswer = 'C',
                Explanation = "\\x1b is the hexadecimal representation of ASCII 27 (ESC character), now simplified to \\e in C# 13."
            },
            
            new TriviaQuestion
            {
                Question = "What is the primary benefit of the \\e escape character?",
                Options = ["Faster execution", "Better readability", "Memory efficiency", "Cross-platform compatibility"],
                CorrectAnswer = 'B',
                Explanation = "The main benefit is improved code readability and maintainability when working with ANSI escape sequences."
            }
        ];
    }
}