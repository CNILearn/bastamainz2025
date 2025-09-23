using System.ComponentModel.DataAnnotations;

namespace CSharp14FieldKeywordWpf.Models;

/// <summary>
/// Represents a user profile with traditional property implementation.
/// Demonstrates the verbose approach with explicit backing fields.
/// </summary>
public class UserProfileTraditional : ObservableObjectTraditional
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _email = string.Empty;
    private int _age;
    private decimal _salary;
    private bool _isActive = true;
    private DateTime _lastLoginDate = DateTime.Now;

    /// <summary>
    /// First name with validation - traditional approach
    /// </summary>
    public string FirstName
    {
        get => _firstName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("First name cannot be empty");
            
            SetProperty(ref _firstName, value);
        }
    }

    /// <summary>
    /// Last name with validation - traditional approach
    /// </summary>
    public string LastName
    {
        get => _lastName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Last name cannot be empty");
            
            SetProperty(ref _lastName, value);
        }
    }

    /// <summary>
    /// Email with validation - traditional approach
    /// </summary>
    public string Email
    {
        get => _email;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || !new EmailAddressAttribute().IsValid(value))
                throw new ArgumentException("Invalid email address");
            
            SetProperty(ref _email, value);
        }
    }

    /// <summary>
    /// Age with range validation - traditional approach
    /// </summary>
    public int Age
    {
        get => _age;
        set
        {
            if (value < 0 || value > 150)
                throw new ArgumentOutOfRangeException(nameof(value), "Age must be between 0 and 150");
            
            SetProperty(ref _age, value);
        }
    }

    /// <summary>
    /// Salary with validation - traditional approach
    /// </summary>
    public decimal Salary
    {
        get => _salary;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Salary cannot be negative");
            
            SetProperty(ref _salary, value);
        }
    }

    /// <summary>
    /// Active status - traditional approach
    /// </summary>
    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    /// <summary>
    /// Last login date with auto-update on access - traditional approach
    /// </summary>
    public DateTime LastLoginDate
    {
        get => _lastLoginDate;
        set => SetProperty(ref _lastLoginDate, value);
    }

    /// <summary>
    /// Computed full name property
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";
}

/// <summary>
/// Represents a user profile using C# 14's field keyword.
/// Demonstrates the modern, cleaner approach with field-backed properties.
/// </summary>
public class UserProfileModern : ObservableObjectModern
{
    /// <summary>
    /// First name with validation using field keyword - cleaner syntax
    /// </summary>
    public string FirstName
    {
        get;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("First name cannot be empty");
            
            if (!EqualityComparer<string>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FullName)); // Notify dependent property
            }
        }
    } = string.Empty;

    /// <summary>
    /// Last name with validation using field keyword - cleaner syntax
    /// </summary>
    public string LastName
    {
        get;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Last name cannot be empty");
            
            if (!EqualityComparer<string>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FullName)); // Notify dependent property
            }
        }
    } = string.Empty;

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