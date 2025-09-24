using Stage6.ForAttributeWithMetadataName.Attributes;

namespace Stage6.ForAttributeWithMetadataName.Sample;

/// <summary>
/// Sample entity for demonstrating Stage 6: ForAttributeWithMetadataName Data Source Generator
/// This class uses enhanced attribute detection for improved performance
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
