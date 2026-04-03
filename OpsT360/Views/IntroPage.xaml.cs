using Microsoft.Extensions.DependencyInjection;

namespace OpsT360.Views;

public partial class IntroPage : ContentPage
{
    private readonly IServiceProvider _serviceProvider;
    private bool _navigated;

    public IntroPage(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(1800);
        await NavigateToLoginAsync();
    }

    private async Task NavigateToLoginAsync()
    {
        if (_navigated)
            return;

        _navigated = true;
        var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
        await Navigation.PushAsync(loginPage);
        Navigation.RemovePage(this);
    }
}
