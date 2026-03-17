using System.ComponentModel;
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
    private readonly IAuthState _authState;
    private readonly IServiceProvider _serviceProvider;

    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _ip = "127.0.0.1";
    private string _device = "android";
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

    public LoginViewModel(ILoginService loginService, IAuthState authState, IServiceProvider serviceProvider)
    {
        _loginService = loginService;
        _authState = authState;
        _serviceProvider = serviceProvider;

        Ip = "127.0.0.1";
        Device = "android";

        LoginCommand = new AsyncRelayCommand(LoginAsync, () => CanLogin);
        TogglePasswordVisibilityCommand = new RelayCommand(TogglePasswordVisibility);
    }

    private void TogglePasswordVisibility()
    {
        IsPasswordHidden = !IsPasswordHidden;
    }

    private async Task LoginAsync()
    {
        if (!CanLogin || IsBusy)
            return;

        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            var token = await _loginService.LoginAsync(new LoginRequest
            {
                Username = Username.Trim(),
                Password = Password,
                Ip = Ip.Trim(),
                Device = Device
            });

            if (string.IsNullOrWhiteSpace(token))
            {
                StatusMessage = "No se pudo iniciar sesión.";
                return;
            }

            await NavigateToSealInspectionAsync();
        }
        catch (Exception ex)
        {
            _authState.SetToken(string.Empty);
            System.Diagnostics.Debug.WriteLine(ex.ToString());
            StatusMessage = $"Error al iniciar sesión: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task NavigateToSealInspectionAsync()
    {
        var next = _serviceProvider.GetRequiredService<MainMenuPage>();

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            Application.Current!.MainPage = next;
        });
    }

        MainThread.BeginInvokeOnMainThread(() =>
        {
            Application.Current!.MainPage = next;
        });

        return Task.CompletedTask;
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
