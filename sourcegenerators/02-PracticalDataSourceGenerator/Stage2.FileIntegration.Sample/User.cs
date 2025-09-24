using Stage2.FileIntegration.Attributes;

namespace Stage2.FileIntegration.Sample;

/// <summary>
/// Sample entity for demonstrating Stage 2: File Integration Data Source Generator
/// This class uses external JSON configuration for enhanced data generation
/// </summary>
[DataSource(EntityName = "User", Count = 5, ConfigurationFile = "User.datasource.json")]
public class User
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserRole Role { get; set; }
}
