using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using OpsT360.Models;
using OpsT360.Services;
using OpsT360.Views;

namespace OpsT360.ViewModels;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly ILoginService _loginService;
    private readonly IServiceProvider _serviceProvider;

    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _ip = "127.0.0.1";
    private string _device = "web";
    private bool _isBusy;
    private bool _isPasswordHidden = true;
    private string _statusMessage = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public IAsyncRelayCommand LoginCommand { get; }
    public IRelayCommand TogglePasswordVisibilityCommand { get; }

    public string Username
    {
        get => _username;
        set
        {
            if (SetProperty(ref _username, value))
                RaiseLoginStateChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if (SetProperty(ref _password, value))
                RaiseLoginStateChanged();
        }
    }

    public string Ip
    {
        get => _ip;
        set => SetProperty(ref _ip, value);
    }

    public string Device
    {
        get => _device;
        set => SetProperty(ref _device, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
                RaiseLoginStateChanged();
        }
    }

    public bool IsPasswordHidden
    {
        get => _isPasswordHidden;
        set
        {
            if (SetProperty(ref _isPasswordHidden, value))
                OnPropertyChanged(nameof(PasswordToggleGlyph));
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

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

        LoginCommand = new AsyncRelayCommand(LoginAsync, () => CanLogin);
        TogglePasswordVisibilityCommand = new RelayCommand(TogglePasswordVisibility);
    }

    private void TogglePasswordVisibility()
    {
        IsPasswordHidden = !IsPasswordHidden;
    }

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
            StatusMessage = $"Error de red al conectar a {LoginService.PrimaryLoginUrl}. No es header JSON; es conectividad de red al puerto/API. Detalle: {ex.Message} {(ex.InnerException is null ? string.Empty : "| " + ex.InnerException.Message)}";
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

    private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
            return false;

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void RaiseLoginStateChanged()
    {
        OnPropertyChanged(nameof(CanLogin));
        OnPropertyChanged(nameof(SignInButtonColor));
        LoginCommand.NotifyCanExecuteChanged();
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
