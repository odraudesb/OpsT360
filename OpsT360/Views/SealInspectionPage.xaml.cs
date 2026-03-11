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

        var sdkCaptured = await _vm.TryCaptureSealFromSdkAsync(sealNumber);
        if (sdkCaptured)
        {
            _vm.ReadSealCommand.Execute(sealNumber.ToString());
            return;
        }

        _vm.StatusText = "No se obtuvo EPC por SDK RFID. Verifica que el handheld esté en modo UHF y no en escáner de código de barras.";
    }

    private void OnSealEntryFocused(object? sender, FocusEventArgs e)
    {
        if (sender is not Entry entry || !int.TryParse(entry.ClassId, out var sealNumber))
            return;

        _vm.StatusText = $"Sello #{sealNumber} listo. Pulsa Read seal para activar antena y capturar EPC.";
    }
}
