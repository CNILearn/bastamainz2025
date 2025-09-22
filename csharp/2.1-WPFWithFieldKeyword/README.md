# C# 14 Field Keyword WPF MVVM Sample

A comprehensive demonstration of C# 14's new `field` keyword in a Windows Presentation Foundation (WPF) application using the Model-View-ViewModel (MVVM) pattern. This sample showcases how the `field` keyword improves code readability, debugging experience, and property definition simplicity in real-world MVVM scenarios.

## 🚀 Features

- **C# 14 Field Keyword**: Demonstrates field-backed properties with the new `field` keyword
- **WPF MVVM Pattern**: Complete MVVM implementation with proper separation of concerns
- **Enhanced Debugging**: Shows improved debugging experience compared to traditional backing fields
- **Property Validation**: Demonstrates validation logic using field-backed properties
- **Data Binding**: Full two-way data binding between Views and ViewModels
- **Command Pattern**: ICommand implementation for user interactions
- **Modern UI**: Clean, responsive WPF interface with proper styling

## 📋 Prerequisites

- **.NET 10 RC 1 SDK** or later
- **Visual Studio 2024** or **VS Code** with C# extension
- **Windows** (for WPF support)

## 🏗️ Architecture

```
CSharp14FieldKeywordWpf/
├── Models/                          # Domain models and base classes
│   ├── ObservableObject.cs         # Base classes (traditional vs modern)
│   └── UserProfile.cs              # User model demonstrating field keyword
├── ViewModels/                      # MVVM ViewModels
│   ├── MainViewModel.cs            # Main application ViewModel
│   └── Converters.cs               # Value converters for data binding
├── Views/                           # WPF Views and Windows
│   ├── MainWindow.xaml             # Main application window
│   └── MainWindow.xaml.cs          # Code-behind
├── App.xaml                        # Application resources and startup
├── App.xaml.cs                     # Application entry point
├── CSharp14FieldKeywordWpf.csproj  # Project configuration
└── README.md                       # This file
```

## 🔧 Setup

### 1. Install Dependencies

```bash
# Navigate to project directory
cd src/CSharp14FieldKeywordWpf

# Restore NuGet packages
dotnet restore
```

### 2. Build the Project

```bash
# Build the application
dotnet build
```

### 3. Run the Application

```bash
# Run the WPF application
dotnet run
```

## 💡 Key Concepts

### Field Keyword Benefits

The C# 14 `field` keyword provides several advantages over traditional property implementations:

#### Traditional Approach (Verbose)

```csharp
public class UserProfileTraditional : ObservableObjectTraditional
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private int _age;

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
}
```

#### Modern Approach with Field Keyword (Cleaner)

```csharp
public class UserProfileModern : ObservableObjectModern
{
    public string FirstName
    {
        get => field;
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

    public string LastName
    {
        get => field;
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

    public int Age
    {
        get => field;
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
}
```

### Key Improvements

1. **Reduced Boilerplate**: No need to declare explicit backing fields
2. **Cleaner Syntax**: Direct access to the implicit field via `field` keyword
3. **Better Debugging**: Improved debugging experience with cleaner stack traces
4. **Inline Initialization**: Properties can be initialized inline using `= value` syntax
5. **Consistent Naming**: No more `_fieldName` vs `fieldName` naming considerations

### MVVM Pattern Implementation

The sample demonstrates proper MVVM separation:

- **Models**: Pure data classes with validation logic
- **ViewModels**: Presentation logic, commands, and data binding
- **Views**: XAML-based UI with data binding and minimal code-behind

### Property Change Notification

The sample shows two approaches to `INotifyPropertyChanged` implementation:

1. **Traditional**: Using helper methods and explicit backing fields
2. **Modern**: Using the `field` keyword with simplified property setters

## 🎯 Use Cases Demonstrated

### 1. Property Validation

```csharp
public string Email
{
    get => field;
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
```

### 2. Computed Properties

```csharp
public string FullName => $"{FirstName} {LastName}";
```

### 3. Business Logic Integration

```csharp
public bool IsActive
{
    get => field;
    set
    {
        if (field != value)
        {
            field = value;
            OnPropertyChanged();
            // Trigger business logic when status changes
            if (value) UpdateLastLogin();
        }
    }
} = true;
```

### 4. MVVM Command Binding

```csharp
public UserProfileModern? SelectedUser
{
    get => field;
    set
    {
        if (!ReferenceEquals(field, value))
        {
            field = value;
            OnPropertyChanged();
            
            // Update command states when selection changes
            SaveUserCommand.RaiseCanExecuteChanged();
            DeleteUserCommand.RaiseCanExecuteChanged();
        }
    }
}
```

## 🔍 Sample Operations

### User Management
- **Add User**: Creates new user profiles with default values
- **Edit User**: Modify user properties with real-time validation
- **Delete User**: Remove users with proper selection management
- **Save User**: Persist changes with simulated async operations

### Data Validation
- **Email Validation**: Ensures valid email format using data annotations
- **Age Range**: Validates age between 0 and 150 years
- **Required Fields**: Ensures first name and last name are not empty
- **Positive Values**: Validates salary is not negative

### UI Features
- **Responsive Layout**: Adapts to window resizing
- **Status Messages**: Shows operation feedback with auto-clear
- **Loading Indicators**: Visual feedback during async operations
- **Data Binding**: Two-way binding between UI and ViewModel

## 🧠 Debugging Improvements

The `field` keyword provides several debugging advantages:

1. **Cleaner Stack Traces**: No intermediate backing field references
2. **Direct Property Access**: Debugger shows property values directly
3. **Simplified Breakpoints**: Set breakpoints directly on property accessors
4. **Better IntelliSense**: Improved autocomplete and navigation

## 📊 Sample Output

When you run the application, you'll see:

```
🏠 Main Window
├── User List Panel (Left)
│   ├── John Doe (john.doe@example.com)
│   ├── Jane Smith (jane.smith@example.com)
│   └── Bob Johnson (bob.johnson@example.com)
├── User Details Panel (Right)
│   ├── Form Fields (First Name, Last Name, Email, Age, Salary)
│   ├── Active Status Checkbox
│   └── Last Login Display
├── Action Buttons
│   ├── Add, Save, Delete, Refresh
│   └── Status Messages
└── C# 14 Benefits Information Panel
```

## 🔗 Related Technologies

- **MVVM Pattern**: Modern WPF application architecture
- **Data Binding**: WPF's powerful binding engine
- **INotifyPropertyChanged**: Property change notification interface
- **ICommand Interface**: Command pattern for UI interactions
- **Value Converters**: Data transformation for UI binding

## 📚 Additional Resources

- [C# 14 Field Keyword Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-14.0/field-keyword)
- [WPF MVVM Pattern Guide](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/advanced/mvvm)
- [.NET 10 Release Notes](https://docs.microsoft.com/dotnet/core/whats-new/dotnet-10)
- [C# 14 Language Features](https://docs.microsoft.com/dotnet/csharp/whats-new/csharp-14)

## 🤝 Contributing

This sample is part of the .NET 10 samples collection. Feel free to submit issues and enhancement requests!

## 📄 License

This sample is licensed under the MIT License. See LICENSE file for details.