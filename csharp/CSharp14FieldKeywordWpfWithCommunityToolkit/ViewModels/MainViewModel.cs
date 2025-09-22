using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using CSharp14FieldKeywordWpf.Models;

namespace CSharp14FieldKeywordWpf.ViewModels;

/// <summary>
/// Main ViewModel demonstrating C# 14 field keyword usage in MVVM pattern.
/// Shows improved readability and debugging experience.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    /// <summary>
    /// Currently selected user profile using field keyword
    /// Demonstrates cleaner property syntax with validation
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveUserCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteUserCommand))]
    public partial UserProfileModern? SelectedUser {  get; set; }
    {
        get;
        set
        {
            if (!ReferenceEquals(field, value))
            {
                field = value;
                OnPropertyChanged();
                
                // Update command states when selection changes
                SaveUserCommand.NotifyCanExecuteChanged();
                DeleteUserCommand.NotifyCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Collection of user profiles for demonstration
    /// Uses field keyword for cleaner initialization
    /// </summary>
    public ObservableCollection<UserProfileModern> Users { get; } = [];

    /// <summary>
    /// Status message using field keyword with validation
    /// </summary>
    public string StatusMessage
    {
        get;
        set
        {
            if (!string.Equals(field, value, StringComparison.Ordinal))
            {
                field = value;
                OnPropertyChanged();
                
                // Auto-clear status after 3 seconds
                if (!string.IsNullOrEmpty(value))
                {
                    Task.Delay(3000).ContinueWith(_ =>
                    {
                        if (string.Equals(StatusMessage, value, StringComparison.Ordinal))
                        {
                            StatusMessage = string.Empty;
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }
    } = string.Empty;

    /// <summary>
    /// Indicates if the application is performing an operation
    /// Using field keyword for simple boolean property
    /// </summary>
    public bool IsBusy
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
                
                // Update command states when busy state changes
                AddUserCommand.NotifyCanExecuteChanged();
                SaveUserCommand.NotifyCanExecuteChanged();
                DeleteUserCommand.NotifyCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// User count for display - using field keyword with computed updates
    /// </summary>
    public int UserCount
    {
        get;
        private set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    // Commands for MVVM pattern
    public RelayCommand AddUserCommand { get; }
    public RelayCommand SaveUserCommand { get; }
    public RelayCommand DeleteUserCommand { get; }
    public RelayCommand RefreshCommand { get; }

    public MainViewModel()
    {
        // Initialize commands
        AddUserCommand = new RelayCommand(AddUser, () => !IsBusy);
        SaveUserCommand = new RelayCommand(SaveUser, () => !IsBusy && SelectedUser != null);
        DeleteUserCommand = new RelayCommand(DeleteUser, () => !IsBusy && SelectedUser != null);
        RefreshCommand = new RelayCommand(RefreshUsers, () => !IsBusy);

        // Initialize with sample data
        InitializeSampleData();
        
        // Set up collection change notification
        Users.CollectionChanged += (_, _) => UserCount = Users.Count;
        UserCount = Users.Count;
    }

    private void InitializeSampleData()
    {
        UserProfileModern[] sampleUsers =
        [
            new UserProfileModern
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Age = 30,
                Salary = 75000m,
                IsActive = true
            },
            new UserProfileModern
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Age = 28,
                Salary = 82000m,
                IsActive = true
            },
            new UserProfileModern
            {
                FirstName = "Bob",
                LastName = "Johnson",
                Email = "bob.johnson@example.com",
                Age = 35,
                Salary = 68000m,
                IsActive = false
            }
        ];

        foreach (var user in sampleUsers)
        {
            Users.Add(user);
        }

        SelectedUser = Users.FirstOrDefault();
    }

[RelayCommand()]
private async Task AddUser()
    {
        IsBusy = true;
        StatusMessage = "Adding new user...";

        try
        {
            // Simulate async operation
            await Task.Delay(500);

            UserProfileModern newUser = new()
            {
                FirstName = $"User",
                LastName = $"{Users.Count + 1}",
                Email = $"user{Users.Count + 1}@example.com",
                Age = 25,
                Salary = 50000m,
                IsActive = true
            };

            Users.Add(newUser);
            SelectedUser = newUser;
            
            StatusMessage = "User added successfully!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error adding user: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

[RelayCommand(CanExecute = nameof(CanSaveUser))]
private async void SaveUser()
    {
        if (SelectedUser == null) return;

        IsBusy = true;
        StatusMessage = "Saving user...";

        try
        {
            // Simulate async save operation
            await Task.Delay(300);
            
            SelectedUser.UpdateLastLogin();
            StatusMessage = "User saved successfully!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving user: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

[RelayCommand(CanExecute = nameof(CanDeleteUser))]
private async void DeleteUser()
    {
        if (SelectedUser == null) return;

        IsBusy = true;
        StatusMessage = "Deleting user...";

        try
        {
            // Simulate async delete operation
            await Task.Delay(300);

            var userToDelete = SelectedUser;
            var index = Users.IndexOf(userToDelete);
            
            Users.Remove(userToDelete);
            
            // Select next available user
            if (Users.Count > 0)
            {
                SelectedUser = Users[Math.Min(index, Users.Count - 1)];
            }
            else
            {
                SelectedUser = null;
            }
            
            StatusMessage = "User deleted successfully!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error deleting user: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

[RelayCommand]
    private async void RefreshUsers()
    {
        IsBusy = true;
        StatusMessage = "Refreshing users...";

        try
        {
            // Simulate async refresh operation
            await Task.Delay(800);
            
            // Update last login for all active users
            foreach (var user in Users.Where(u => u.IsActive))
            {
                user.UpdateLastLogin();
            }
            
            StatusMessage = "Users refreshed successfully!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error refreshing users: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}