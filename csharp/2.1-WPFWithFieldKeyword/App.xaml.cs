using System.Windows;

namespace CSharp14FieldKeywordWpf;

/// <summary>
/// Interaction logic for App.xaml
/// Entry point for the C# 14 Field Keyword WPF MVVM demonstration
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Set up any global error handling or initialization here
        DispatcherUnhandledException += (sender, args) =>
        {
            MessageBox.Show(
                $"An unexpected error occurred: {args.Exception.Message}", 
                "Error", 
                MessageBoxButton.OK, 
                MessageBoxImage.Error);
            
            args.Handled = true;
        };
    }
}