using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using OpsT360.Services;
using OpsT360.ViewModels;

namespace OpsT360.Views;

public sealed class MainMenuPage : FlyoutPage
{
    private readonly IServiceProvider _serviceProvider;
    private readonly NavigationPage _homeNavigation;

    private NavigationPage? _sealNavigation;
    private SealInspectionViewModel? _sealInspectionViewModel;

    public MainMenuPage(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        FlyoutLayoutBehavior = FlyoutLayoutBehavior.Popover;
        Flyout = CreateFlyoutPage();

        _homeNavigation = CreateNavigationPage(CreateHomePage());
        Detail = _homeNavigation;
    }

    public void OpenMenu() => IsPresented = true;

    public void NavigateToSealPlacement()
    {
        EnsureSealPage();
        _sealInspectionViewModel!.ConfigureOperationMode(isInspectionChange: false);
        Detail = _sealNavigation!;
        IsPresented = false;
    }

    public void NavigateToSealInspectionChange()
    {
        EnsureSealPage();
        _sealInspectionViewModel!.ConfigureOperationMode(isInspectionChange: true);
        Detail = _sealNavigation!;
        IsPresented = false;
    }

    public async Task ChangeLanguageAsync()
    {
        var page = GetCurrentDetailPage();
        if (page is null)
            return;

        var selected = await page.DisplayActionSheet("Change language", "Cancel", null, "English");

        if (selected == "English")
            await page.DisplayAlert("Language", "Language switched to English.", "OK");

        IsPresented = false;
    }

    public void Logout()
    {
        var authState = _serviceProvider.GetRequiredService<IAuthState>();
        authState.SetToken(string.Empty);

        var loginPage = _serviceProvider.GetRequiredService<LoginPage>();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            Application.Current!.MainPage = new NavigationPage(loginPage);
        });
    }

    private void EnsureSealPage()
    {
        if (_sealNavigation is not null && _sealInspectionViewModel is not null)
            return;

        var sealPage = _serviceProvider.GetRequiredService<SealInspectionPage>();

        var vm = sealPage.BindingContext as SealInspectionViewModel
                 ?? _serviceProvider.GetRequiredService<SealInspectionViewModel>();

        sealPage.BindingContext = vm;

        _sealInspectionViewModel = vm;
        _sealNavigation = CreateNavigationPage(sealPage);
    }

    private ContentPage? GetCurrentDetailPage()
    {
        if (Detail is NavigationPage nav)
            return nav.CurrentPage as ContentPage;

        return Detail as ContentPage;
    }

    private ContentPage CreateHomePage()
    {
        var layout = new VerticalStackLayout
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Spacing = 24
        };

        layout.Children.Add(new Image
        {
            Source = "logo.png",
            WidthRequest = 220,
            HeightRequest = 220,
            Aspect = Aspect.AspectFit,
            HorizontalOptions = LayoutOptions.Center
        });

        layout.Children.Add(new Label
        {
            Text = "iT360°",
            TextColor = Colors.White,
            FontSize = 34,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center,
            HorizontalOptions = LayoutOptions.Center
        });

        return new ContentPage
        {
            Title = "iT360° Application",
            BackgroundColor = Color.FromArgb("#4357E8"),
            Content = new Grid
            {
                Children =
                {
                    layout
                }
            }
        };
    }

    private ContentPage CreateFlyoutPage()
    {
        var header = new Grid
        {
            BackgroundColor = Color.FromArgb("#4357E8"),
            Padding = new Thickness(20, 50, 20, 24)
        };

        header.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        header.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var logo = new Image
        {
            Source = "logo.png",
            WidthRequest = 90,
            HeightRequest = 90,
            Aspect = Aspect.AspectFit,
            HorizontalOptions = LayoutOptions.Start
        };

        var title = new Label
        {
            Text = "iT360° Application",
            TextColor = Colors.White,
            FontSize = 22,
            FontAttributes = FontAttributes.Bold,
            Margin = new Thickness(0, 12, 0, 0)
        };

        header.Children.Add(logo);
        Grid.SetRow(logo, 0);

        header.Children.Add(title);
        Grid.SetRow(title, 1);

        var menuLayout = new VerticalStackLayout
        {
            Spacing = 8,
            Padding = new Thickness(16, 18)
        };

        menuLayout.Children.Add(CreateMenuButton("RFID Seal Placement", NavigateToSealPlacement));
        menuLayout.Children.Add(CreateMenuButton("RFID Seal Inspection Change", NavigateToSealInspectionChange));
        menuLayout.Children.Add(CreateMenuButton("Change Language", async () => await ChangeLanguageAsync()));
        menuLayout.Children.Add(CreateMenuButton("Sign Out", Logout));

        var scroll = new ScrollView
        {
            Content = menuLayout
        };

        var root = new Grid
        {
            BackgroundColor = Colors.White
        };

        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

        root.Children.Add(header);
        Grid.SetRow(header, 0);

        root.Children.Add(scroll);
        Grid.SetRow(scroll, 1);

        return new ContentPage
        {
            Title = "Menu",
            BackgroundColor = Colors.White,
            Content = root
        };
    }

    private View CreateMenuButton(string text, Action action)
    {
        return new Button
        {
            Text = text,
            BackgroundColor = Colors.Transparent,
            TextColor = Color.FromArgb("#1F2937"),
            HorizontalOptions = LayoutOptions.Fill,
            HorizontalTextAlignment = TextAlignment.Start,
            LineBreakMode = LineBreakMode.WordWrap,
            FontSize = 16,
            Padding = new Thickness(14, 14),
            CornerRadius = 10,
            BorderColor = Color.FromArgb("#E5E7EB"),
            BorderWidth = 1,
            Command = new Command(action)
        };
    }

    private static NavigationPage CreateNavigationPage(Page root)
    {
        return new NavigationPage(root)
        {
            BarBackgroundColor = Color.FromArgb("#4357E8"),
            BarTextColor = Colors.White
        };
    }
}
