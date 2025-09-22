using CommunityToolkit.Mvvm.ComponentModel;

using System.ComponentModel.DataAnnotations;

namespace CSharp14FieldKeywordWpf.Models;


/// <summary>
/// Represents a user profile using C# 14's field keyword.
/// Demonstrates the modern, cleaner approach with field-backed properties.
/// </summary>
public partial class UserProfile : ObservableObject
{
    /// <summary>
    /// First name with validation using field keyword - cleaner syntax
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor("FullName")]
    public partial string FirstName { get; set; }


    /// <summary>
    /// Last name with validation using field keyword - cleaner syntax
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor("FullName")]
    public partial string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Email with validation using field keyword - cleaner syntax
    /// </summary>
    public string Email
    {
        get;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || !new EmailAddressAttribute().IsValid(value))
                throw new ArgumentException("Invalid email address");
            
            if (!EqualityComparer<string>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged();
            }
        }
    } = string.Empty;

    /// <summary>
    /// Age with range validation using field keyword - cleaner syntax
    /// </summary>
    public int Age
    {
        get;
        set
        {
            if (value < 0 || value > 150)
                throw new ArgumentOutOfRangeException(nameof(value), "Age must be between 0 and 150");
            
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Salary with validation using field keyword - cleaner syntax
    /// </summary>
    public decimal Salary
    {
        get;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Salary cannot be negative");
            
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Active status using field keyword - simplified
    /// </summary>
    public bool IsActive
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    } = true;

    /// <summary>
    /// Last login date with auto-update using field keyword
    /// </summary>
    public DateTime LastLoginDate
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    } = DateTime.Now;

    /// <summary>
    /// Computed full name property - no field needed
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Method to update last login to current time
    /// </summary>
    public void UpdateLastLogin()
    {
        LastLoginDate = DateTime.Now;
    }
}