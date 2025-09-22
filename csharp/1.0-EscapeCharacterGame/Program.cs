using EscapeCharacterGame;

// Create and run the game
Runner runner = new();
await runner.RunAsync();

/// <summary>
/// Represents a trivia question with multiple choice answers.
/// </summary>
public record TriviaQuestion
{
    public required string Question { get; init; }
    public required string[] Options { get; init; }
    public required char CorrectAnswer { get; init; }
    public required string Explanation { get; init; }
}