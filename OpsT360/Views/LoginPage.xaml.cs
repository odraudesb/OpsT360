using OpsT360.ViewModels;

namespace OpsT360.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private async void OnSignInClicked(object? sender, EventArgs e)
    {
        if (BindingContext is LoginViewModel vm && vm.LoginCommand.CanExecute(null))
        {
            await vm.LoginCommand.ExecuteAsync(null);
        }
    }
}
