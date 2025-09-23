using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CSharp14FieldKeywordWpf.Models;

/// <summary>
/// Base class implementing INotifyPropertyChanged using traditional approach.
/// This demonstrates the "before" state without the field keyword.
/// </summary>
public abstract class ObservableObjectTraditional : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Traditional approach to setting properties with backing fields
    /// </summary>
    protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

/// <summary>
/// Modern base class leveraging C# 14's field keyword for cleaner property definitions.
/// This demonstrates the "after" state with improved readability and debugging.
/// </summary>
public abstract class ObservableObjectModern : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}