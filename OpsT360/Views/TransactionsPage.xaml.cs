using System.Collections.ObjectModel;
using OpsT360.ViewModels;

namespace OpsT360.Views;

public partial class TransactionsPage : ContentPage
{
    private readonly SealInspectionViewModel _sealInspectionViewModel;
    private readonly IReadOnlyList<TransactionCard> _allTransactions;

    public ObservableCollection<TransactionCard> Transactions { get; }

    public TransactionsPage(SealInspectionViewModel sealInspectionViewModel)
    {
        InitializeComponent();
        _sealInspectionViewModel = sealInspectionViewModel;

        _allTransactions = new[]
        {
            new TransactionCard("SUDU343692", "Colocación Sello RFID posterior Inspección", "Dry", "Ship", "Export", "At Port", "01/12/2025", true),
            new TransactionCard("CMAU5573305", "Preaviso Exportación", "Reefer", "Ship", "Export", "At Port", "13/09/2025", false),
            new TransactionCard("MSCU640771", "Preaviso Exportación", "Dry", "Ship", "Export", "At Port", "22/08/2025", false)
        };

        Transactions = new ObservableCollection<TransactionCard>(_allTransactions);
        BindingContext = this;
    }

    private void OnPageMenuClicked(object? sender, EventArgs e)
    {
        var flyout = (Parent as NavigationPage)?.Parent as MainMenuPage;
        flyout?.OpenMenu();
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        var query = e.NewTextValue?.Trim() ?? string.Empty;
        var filtered = string.IsNullOrWhiteSpace(query)
            ? _allTransactions
            : _allTransactions.Where(t => t.Code.Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();

        Transactions.Clear();
        foreach (var transaction in filtered)
            Transactions.Add(transaction);
    }

    private void OnTransactionSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not TransactionCard selected)
            return;

        _sealInspectionViewModel.ContainerId = selected.Code;
        _sealInspectionViewModel.StatusText = $"Contenedor {selected.Code} cargado desde Transactions.";

        var flyout = (Parent as NavigationPage)?.Parent as MainMenuPage;
        flyout?.NavigateTo(MainMenuOption.SealPlacement);

        if (sender is CollectionView cv)
            cv.SelectedItem = null;
    }

    public sealed record TransactionCard(
        string Code,
        string LastEvent,
        string ContainerType,
        string Ship,
        string Category,
        string Status,
        string EventDate,
        bool HasAlert);
}
