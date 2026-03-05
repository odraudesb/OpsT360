using System.Net;
using System.Net.Sockets;
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
    private string device = "web";

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isPasswordHidden = true;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public bool CanLogin => !IsBusy
        && !string.IsNullOrWhiteSpace(Username)
        && !string.IsNullOrWhiteSpace(Password);

    public Color SignInButtonColor => CanLogin ? Color.FromArgb("#4357E8") : Color.FromArgb("#D8DDE5");

    public string PasswordToggleGlyph => IsPasswordHidden ? "👁" : "🙈";

    public LoginViewModel(ILoginService loginService, IServiceProvider serviceProvider)
    {
        _loginService = loginService;
        _serviceProvider = serviceProvider;

        Ip = ResolveLocalIpAddress();
        Device = "web";
    }

    partial void OnUsernameChanged(string value)
    {
        OnPropertyChanged(nameof(CanLogin));
        OnPropertyChanged(nameof(SignInButtonColor));
    }

    partial void OnPasswordChanged(string value)
    {
        OnPropertyChanged(nameof(CanLogin));
        OnPropertyChanged(nameof(SignInButtonColor));
    }

    partial void OnIsBusyChanged(bool value)
    {
        OnPropertyChanged(nameof(CanLogin));
        OnPropertyChanged(nameof(SignInButtonColor));
    }

    partial void OnIsPasswordHiddenChanged(bool value)
    {
        OnPropertyChanged(nameof(PasswordToggleGlyph));
    }

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordHidden = !IsPasswordHidden;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (!CanLogin)
            return;

        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            var token = await _loginService.LoginAsync(new LoginRequest
            {
                Username = Username.Trim(),
                Password = Password,
                Ip = Ip,
                Device = "web"
            });

            if (string.IsNullOrWhiteSpace(token))
            {
                StatusMessage = "No se pudo iniciar sesión.";
                return;
            }

            var next = _serviceProvider.GetRequiredService<SealInspectionPage>();
            await Application.Current!.MainPage!.Navigation.PushAsync(next);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error de login: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static string ResolveLocalIpAddress()
    {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ip = host.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(a));
            return ip?.ToString() ?? "127.0.0.1";
        }
        catch
        {
            return "127.0.0.1";
        }
    }
}
