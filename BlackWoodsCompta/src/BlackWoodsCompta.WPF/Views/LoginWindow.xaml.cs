using System.Windows;
using System.Windows.Controls;
using BlackWoodsCompta.WPF.ViewModels;

namespace BlackWoodsCompta.WPF.Views;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        
        _viewModel.LoginCompleted += OnLoginCompleted;
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            _viewModel.Password = passwordBox.Password;
        }
    }

    private void OnLoginCompleted(object? sender, bool success)
    {
        if (success)
        {
            var mainWindow = App.ServiceProvider?.GetService(typeof(MainWindow)) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.Show();
                this.Close();
            }
        }
    }
}
