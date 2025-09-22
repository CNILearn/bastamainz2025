using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharp14FieldKeywordWpf.Models;

namespace CSharp14FieldKeywordWpf.ViewModels;

/// <summary>
/// Main ViewModel demonstrating CommunityToolkit.MVVM source generator usage in MVVM pattern.
/// Shows improved readability and debugging experience with ObservableProperty and RelayCommand attributes.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    /// <summary>
    /// Currently selected user profile using ObservableProperty
    /// Demonstrates cleaner property syntax with validation
    /// </summary>
    [ObservableProperty]
    public partial UserProfile? SelectedUser { get; set; }

    /// <summary>
    /// Collection of user profiles for demonstration
    /// </summary>
    public ObservableCollection<UserProfile> Users { get; } = [];

    /// <summary>
    /// Status message using ObservableProperty with validation
    /// </summary>
    [ObservableProperty]
    public partial string StatusMessage { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the application is performing an operation
    /// Using ObservableProperty for simple boolean property
    /// </summary>
    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    /// <summary>
    /// User count for display - using ObservableProperty with computed updates
    /// </summary>
    [ObservableProperty]
    public partial int UserCount { get; set; }

    public MainViewModel()
    {
        // Initialize with sample data
        InitializeSampleData();
        
        // Set up collection change notification
        Users.CollectionChanged += (_, _) => UserCount = Users.Count;
        UserCount = Users.Count;
    }

    partial void OnSelectedUserChanged(UserProfile? value)
    {
        // Update command states when selection changes
        SaveUserCommand.NotifyCanExecuteChanged();
        DeleteUserCommand.NotifyCanExecuteChanged();
    }

    partial void OnStatusMessageChanged(string value)
    {
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

    partial void OnIsBusyChanged(bool value)
    {
        // Update command states when busy state changes
        AddUserCommand.NotifyCanExecuteChanged();
        SaveUserCommand.NotifyCanExecuteChanged();
        DeleteUserCommand.NotifyCanExecuteChanged();
        RefreshCommand.NotifyCanExecuteChanged();
    }

    private void InitializeSampleData()
    {
        UserProfile[] sampleUsers =
        [
            new UserProfile
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Age = 30,
                Salary = 75000m,
                IsActive = true
            },
            new UserProfile
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Age = 28,
                Salary = 82000m,
                IsActive = true
            },
            new UserProfile
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

    [RelayCommand(CanExecute = nameof(CanAddUser))]
    private async Task AddUserAsync()
    {
        IsBusy = true;
        StatusMessage = "Adding new user...";

        try
        {
            // Simulate async operation
            await Task.Delay(500);

            UserProfile newUser = new()
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

    private bool CanAddUser() => !IsBusy;

    [RelayCommand(CanExecute = nameof(CanSaveUser))]
    private async Task SaveUserAsync()
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

    private bool CanSaveUser() => !IsBusy && SelectedUser != null;

    [RelayCommand(CanExecute = nameof(CanDeleteUser))]
    private async Task DeleteUserAsync()
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

    private bool CanDeleteUser() => !IsBusy && SelectedUser != null;

    [RelayCommand(CanExecute = nameof(CanRefresh))]
    private async Task RefreshAsync()
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

    private bool CanRefresh() => !IsBusy;
}