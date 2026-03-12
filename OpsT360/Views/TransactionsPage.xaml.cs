using OpsT360.ViewModels;

namespace OpsT360.Views;

public partial class TransactionsPage : ContentPage
{
    private readonly SealInspectionViewModel _sealInspectionViewModel;

    public TransactionsPage(SealInspectionViewModel sealInspectionViewModel)
    {
        InitializeComponent();
        _sealInspectionViewModel = sealInspectionViewModel;

        BindingContext = new
        {
            Transactions = new[]
            {
                new TransactionCard("SUDU343692", "Colocación Sello RFID posterior Inspección", "Dry", "Ship", "Export", "At Port", "01/12/2025", true),
                new TransactionCard("CMAU5573305", "Preaviso Exportación", "Reefer", "Ship", "Export", "At Port", "13/09/2025", false),
                new TransactionCard("MSCU640771", "Preaviso Exportación", "Dry", "Ship", "Export", "At Port", "22/08/2025", false)
            }
        };
    }

    private async void OnPageMenuClicked(object? sender, EventArgs e)
    {
        var option = await DisplayActionSheet("Ir a", "Cancelar", null, "Transactions", "Seals");
        if (option is "Transactions" or "Seals")
            SwitchToTab(option);
    }

    private void OnTransactionSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not TransactionCard selected)
            return;

        _sealInspectionViewModel.ContainerId = selected.Code;
        _sealInspectionViewModel.StatusText = $"Contenedor {selected.Code} cargado desde Transactions.";

        SwitchToTab("Seals");

        if (sender is CollectionView cv)
            cv.SelectedItem = null;
    }

    private void SwitchToTab(string tabTitle)
    {
        var navigationPage = Parent as NavigationPage;
        var tabs = navigationPage?.Parent as TabbedPage;
        if (tabs is null)
            return;

        var targetTab = tabs.Children.FirstOrDefault(child =>
            string.Equals(child.Title, tabTitle, StringComparison.OrdinalIgnoreCase));

        if (targetTab is null)
            return;

        tabs.CurrentPage = targetTab;

        if (targetTab is NavigationPage nav)
            nav.PopToRootAsync(false);
    }

    private sealed record TransactionCard(
        string Code,
        string LastEvent,
        string ContainerType,
        string Ship,
        string Category,
        string Status,
        string EventDate,
        bool HasAlert);
}
