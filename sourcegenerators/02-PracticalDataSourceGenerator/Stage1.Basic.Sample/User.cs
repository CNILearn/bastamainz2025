using Stage1.Basic.Attributes;

namespace Stage1.Basic.Sample;

/// <summary>
/// Sample entity for demonstrating Stage 1: Basic Data Source Generator
/// </summary>
[DataSource(EntityName = "User", Count = 5)]
public class User
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserRole Role { get; set; }
}
