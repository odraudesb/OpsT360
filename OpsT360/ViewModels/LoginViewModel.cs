using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpsT360.Models;
using OpsT360.Services;
using OpsT360.Views;

namespace OpsT360.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly ILoginService _loginService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string ip = "127.0.0.1";

    [ObservableProperty]
    private string device = "android";

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public LoginViewModel(ILoginService loginService, IServiceProvider serviceProvider)
    {
        _loginService = loginService;
        _serviceProvider = serviceProvider;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            var token = await _loginService.LoginAsync(new LoginRequest
            {
                Username = Username,
                Password = Password,
                Ip = Ip,
                Device = Device
            });

            if (string.IsNullOrWhiteSpace(token))
            {
                StatusMessage = "Login inválido (sin token).";
                return;
            }

            var next = _serviceProvider.GetRequiredService<SealInspectionPage>();
            await Application.Current!.MainPage!.Navigation.PushAsync(next);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
