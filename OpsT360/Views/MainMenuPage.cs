using Microsoft.Extensions.DependencyInjection;
using OpsT360.ViewModels;

namespace OpsT360.Views;

public sealed class MainMenuPage : FlyoutPage
{
    private readonly IServiceProvider _serviceProvider;
    private readonly SealInspectionPage _sealInspectionPage;
    private readonly SealInspectionViewModel _sealInspectionViewModel;
    private readonly NavigationPage _homeNavigation;
    private readonly NavigationPage _sealNavigation;

    public MainMenuPage(
        IServiceProvider serviceProvider,
        SealInspectionPage sealInspectionPage,
        SealInspectionViewModel sealInspectionViewModel)
    {
        _serviceProvider = serviceProvider;
        _sealInspectionPage = sealInspectionPage;
        _sealInspectionViewModel = sealInspectionViewModel;

        FlyoutLayoutBehavior = FlyoutLayoutBehavior.Popover;
        Flyout = new MainMenuFlyout(this);

        _homeNavigation = CreateNavigationPage(CreateHomePage());
        _sealNavigation = CreateNavigationPage(_sealInspectionPage);

        Detail = _homeNavigation;
    }

    public void OpenMenu() => IsPresented = true;

    public void NavigateToSealPlacement()
    {
        _sealInspectionViewModel.ConfigureOperationMode(isInspectionChange: false);
        Detail = _sealNavigation;
        IsPresented = false;
    }

    public void NavigateToSealInspectionChange()
    {
        _sealInspectionViewModel.ConfigureOperationMode(isInspectionChange: true);
        Detail = _sealNavigation;
        IsPresented = false;
    }

    public async Task ChangeLanguageAsync()
    {
        var page = GetCurrentDetailPage();
        if (page is null)
            return;

        var selected = await page.DisplayActionSheet("Change language", "Cancel", null, "Español", "English");
        if (selected is "Español" or "English")
            await page.DisplayAlert("Language", $"Selected language: {selected}", "OK");

        IsPresented = false;
    }

    public void Logout()
    {
        var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
        Application.Current!.MainPage = new NavigationPage(loginPage);
    }

    private ContentPage? GetCurrentDetailPage()
    {
        if (Detail is NavigationPage nav)
            return nav.CurrentPage;

        return Detail as ContentPage;
    }

    private ContentPage CreateHomePage()
    {
        return new ContentPage
        {
            Title = "iT360° Application",
            BackgroundColor = Color.FromArgb("#4357E8"),
            Content = new Grid
            {
                Children =
                {
                    new Image
                    {
                        Source = "logo-360.svg",
                        HeightRequest = 200,
                        WidthRequest = 200,
                        Aspect = Aspect.AspectFit,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                }
            }
        };
    }

    private static NavigationPage CreateNavigationPage(Page root)
    {
        var navPage = new NavigationPage(root)
        {
            BarBackgroundColor = Color.FromArgb("#4357E8"),
            BarTextColor = Colors.White
        };

        navPage.ToolbarItems.Add(new ToolbarItem
        {
            IconImageSource = "logo-360.svg",
            Priority = 0,
            Order = ToolbarItemOrder.Primary,
            Command = new Command(() => { })
        });

        return navPage;
    }

    private sealed class MainMenuFlyout : ContentPage
    {
        public MainMenuFlyout(MainMenuPage owner)
        {
            BackgroundColor = Colors.White;

            var menuLayout = new VerticalStackLayout
            {
                Spacing = 0,
                Padding = new Thickness(0, 8)
            };

            menuLayout.Children.Add(CreateMenuItem("Colocación de Sellos [Etiquetas]", true, owner.NavigateToSealPlacement));
            menuLayout.Children.Add(CreateMenuItem("Cambio de Sellos [Etiquetas] por Inspección", false, owner.NavigateToSealInspectionChange));
            menuLayout.Children.Add(CreateMenuItem("Change Language", false, async () => await owner.ChangeLanguageAsync()));
            menuLayout.Children.Add(CreateMenuItem("Cerrar Sesión", false, owner.Logout));

            Content = new ScrollView { Content = menuLayout };
        }

        private static View CreateMenuItem(string text, bool bold, Action action)
        {
            var label = new Label
            {
                Text = text,
                FontAttributes = bold ? FontAttributes.Bold : FontAttributes.None,
                TextColor = Color.FromArgb("#222B3A"),
                VerticalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(16, 0)
            };

            var row = new Grid { HeightRequest = 46, BackgroundColor = Colors.White };
            row.Children.Add(label);
            row.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(action) });

            return new Border
            {
                Stroke = Color.FromArgb("#E2E2E2"),
                StrokeThickness = 0.5,
                Content = row
            };
        }
    }
}
