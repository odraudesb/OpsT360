using OpsT360.Services;
using OpsT360.ViewModels;

namespace OpsT360.Views;

public partial class MainMenuPage : ContentPage
{
    private readonly SealInspectionPage _sealInspectionPage;
    private readonly SealInspectionViewModel _sealInspectionViewModel;
    private readonly LoginPage _loginPage;
    private readonly IAuthState _authState;

    public MainMenuPage(
        SealInspectionPage sealInspectionPage,
        SealInspectionViewModel sealInspectionViewModel,
        LoginPage loginPage,
        IAuthState authState)
    {
        InitializeComponent();
        _sealInspectionPage = sealInspectionPage;
        _sealInspectionViewModel = sealInspectionViewModel;
        _loginPage = loginPage;
        _authState = authState;
    }

    private async void OnHamburgerClicked(object? sender, EventArgs e)
    {
        var selected = await DisplayActionSheet(
            "Menú",
            "Cancelar",
            null,
            "Colocación de Sellos [Etiquetas]",
            "Cambio de Sellos [Etiquetas] por Inspección",
            "Change Languaje",
            "Cerrar Sesión");

        await ExecuteMenuActionAsync(selected);
    }

    private async void OnOpenSealPlacementClicked(object? sender, EventArgs e) =>
        await ExecuteMenuActionAsync("Colocación de Sellos [Etiquetas]");

    private async void OnOpenSealInspectionChangeClicked(object? sender, EventArgs e) =>
        await ExecuteMenuActionAsync("Cambio de Sellos [Etiquetas] por Inspección");

    private async void OnChangeLanguageClicked(object? sender, EventArgs e) =>
        await ExecuteMenuActionAsync("Change Languaje");

    private async void OnLogoutClicked(object? sender, EventArgs e) =>
        await ExecuteMenuActionAsync("Cerrar Sesión");

    private async Task ExecuteMenuActionAsync(string? selected)
    {
        switch (selected)
        {
            case "Colocación de Sellos [Etiquetas]":
                await Navigation.PushAsync(_sealInspectionPage);
                break;

            case "Cambio de Sellos [Etiquetas] por Inspección":
                _sealInspectionViewModel.StatusText =
                    "Modo inspección activo: para los sellos anteriores usar estado Deactivated y Reason: Inspection.";
                await Navigation.PushAsync(_sealInspectionPage);
                break;

            case "Change Languaje":
                await DisplayAlert("Idioma", "Próximamente: selector de idioma.", "OK");
                break;

            case "Cerrar Sesión":
                _authState.SetToken(string.Empty);
                Application.Current!.MainPage = new NavigationPage(_loginPage);
                break;
        }
    }
}
