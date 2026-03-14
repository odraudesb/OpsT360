using Microsoft.Extensions.DependencyInjection;

namespace OpsT360.Views;

public sealed class MainMenuPage : FlyoutPage
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMenuHost _menuHost;

    public MainMenuPage(
        IServiceProvider serviceProvider,
        TransactionsPage transactionsPage,
        SealInspectionPage sealInspectionPage)
    {
        _serviceProvider = serviceProvider;

        FlyoutLayoutBehavior = FlyoutLayoutBehavior.Popover;
        Flyout = new MainMenuFlyout(this);

        _menuHost = new MenuHost(new()
        {
            [MainMenuOption.SealPlacement] = CreateNavigationPage(sealInspectionPage),
            [MainMenuOption.SealInspectionChange] = CreateNavigationPage(transactionsPage)
        });

        NavigateTo(MainMenuOption.SealPlacement);
    }

    public void NavigateTo(MainMenuOption option)
    {
        if (!_menuHost.TryGetPage(option, out var page))
            return;

        Detail = page;
        IsPresented = false;
    }

    public void OpenMenu() => IsPresented = true;

    public async Task ChangeLanguageAsync()
    {
        var currentPage = GetCurrentDetailPage();
        if (currentPage is null)
            return;

        var selected = await currentPage.DisplayActionSheet("Change language", "Cancel", null, "Español", "English");
        if (selected is "Español" or "English")
            await currentPage.DisplayAlert("Language", $"Selected language: {selected}", "OK");
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

    private interface IMenuHost
    {
        bool TryGetPage(MainMenuOption option, out NavigationPage page);
    }

    private sealed class MenuHost : IMenuHost
    {
        private readonly IReadOnlyDictionary<MainMenuOption, NavigationPage> _pages;

        public MenuHost(IReadOnlyDictionary<MainMenuOption, NavigationPage> pages)
        {
            _pages = pages;
        }

        public bool TryGetPage(MainMenuOption option, out NavigationPage page)
            => _pages.TryGetValue(option, out page!);
    }

    private sealed class MainMenuFlyout : ContentPage
    {
        public MainMenuFlyout(MainMenuPage owner)
        {
            Title = "Menu";
            BackgroundColor = Colors.White;

            var menuLayout = new VerticalStackLayout
            {
                Spacing = 0,
                Padding = new Thickness(0, 8)
            };

            menuLayout.Children.Add(CreateMenuItem("Colocación de Sellos [Etiquetas]", true, () => owner.NavigateTo(MainMenuOption.SealPlacement)));
            menuLayout.Children.Add(CreateMenuItem("Cambio de Sellos [Etiquetas] por Inspección", false, () => owner.NavigateTo(MainMenuOption.SealInspectionChange)));
            menuLayout.Children.Add(CreateMenuItem("Change Language", false, async () => await owner.ChangeLanguageAsync()));
            menuLayout.Children.Add(CreateMenuItem("Cerrar Sesión", false, owner.Logout));

            Content = new ScrollView
            {
                Content = menuLayout
            };
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

            var row = new Grid
            {
                HeightRequest = 46,
                BackgroundColor = Colors.White
            };

            row.Children.Add(label);
            row.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(action)
            });

            var border = new Border
            {
                Stroke = Color.FromArgb("#E2E2E2"),
                StrokeThickness = 0.5,
                Content = row
            };

            return border;
        }
    }
}

public enum MainMenuOption
{
    SealPlacement,
    SealInspectionChange
}
