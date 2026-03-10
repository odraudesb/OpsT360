using OpsT360.ViewModels;

namespace OpsT360.Views;

public partial class SealInspectionPage : ContentPage
{
    private readonly SealInspectionViewModel _vm;

    public SealInspectionPage(SealInspectionViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    private void OnSealCompleted(object? sender, EventArgs e)
    {
        if (sender is not Entry entry || !int.TryParse(entry.ClassId, out var sealNumber))
            return;

        _vm.ReadSealCommand.Execute(sealNumber.ToString());
    }

    private async void OnReadSealClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button || !int.TryParse(button.ClassId, out var sealNumber))
            return;

        var entry = sealNumber switch
        {
            1 => SealEntry1,
            2 => SealEntry2,
            3 => SealEntry3,
            4 => SealEntry4,
            _ => null
        };

        if (entry is null)
            return;

        entry.Focus();

        var sdkCaptured = await _vm.TryCaptureSealFromSdkAsync(sealNumber);
        if (sdkCaptured)
        {
            _vm.ReadSealCommand.Execute(sealNumber.ToString());
            return;
        }

        // Fallback: si ya llegó EPC por el hand-held como teclado, confirmar lectura inmediatamente
        if (!string.IsNullOrWhiteSpace(entry.Text))
            _vm.ReadSealCommand.Execute(sealNumber.ToString());
    }

    private void OnSealEntryFocused(object? sender, FocusEventArgs e)
    {
        if (sender is not Entry entry || !int.TryParse(entry.ClassId, out var sealNumber))
            return;

        _vm.StatusText = $"Sello #{sealNumber} listo. Pulsa Read seal para activar antena y capturar EPC.";
    }
}
