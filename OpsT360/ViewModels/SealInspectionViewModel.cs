using System.Collections.ObjectModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpsT360.Models;
using OpsT360.Services;

namespace OpsT360.ViewModels;

public partial class SealInspectionViewModel : ObservableObject
{
    private readonly ITransactionsService _transactionsService;
    private readonly IRfidScannerService _rfidScannerService;

    private readonly Dictionary<string, ContainerProfile> _profiles = new(StringComparer.OrdinalIgnoreCase)
    {
        ["HSU3523430"] = new() { EntityId = 100004, ShippingLine = "HAPAG-LLOYD", Goods = "Piña fresca", Booking = "BK2025-000101", OriginWeight = 22340 },
        ["TEMU3523471"] = new() { EntityId = 100003, ShippingLine = "ONE", Goods = "Mango fresco", Booking = "BK2025-000102", OriginWeight = 21980 },
        ["MSCU8460995"] = new() { EntityId = 100001, ShippingLine = "MSC", Goods = "Banano fresco", Booking = "BK2025-000004", OriginWeight = 23400 },
        ["TEMU0927644"] = new() { EntityId = 12345, ShippingLine = "ONE", Goods = "Arándano fresco", Booking = "BK2025-000113", OriginWeight = 20880 }
    };

    public ObservableCollection<string> ContainerSuggestions { get; } = new();
    public ObservableCollection<SealItem> Seals { get; } = new(Enumerable.Range(1, 4).Select(i => new SealItem(i)));
    public ObservableCollection<EvidenceImage> SealImages { get; } = new(Enumerable.Range(1, 4).Select(i => new EvidenceImage { Label = $"Seal Image #{i}" }));
    public ObservableCollection<string> ActivationPoints { get; } = new() { "Pre-Gate / Insp", "Gate Out", "Yard" };
    public EvidenceImage ContainerImage { get; } = new() { Label = "Container Image" };

    [ObservableProperty] private string containerId = string.Empty;
    [ObservableProperty] private string selectedActivationPoint = "Pre-Gate / Insp";
    [ObservableProperty] private int currentSealIndex;
    [ObservableProperty] private bool sealEntryLocked;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string statusText = "Pulsa Read Seal #1 y dispara el hand-held para capturar EPC.";

    public bool CanUploadImages => Seals.All(s => !string.IsNullOrWhiteSpace(s.Code));
    public bool CanSend => CanUploadImages && SealImages.All(i => i.Bytes is { Length: > 0 }) && ContainerImage.Bytes is { Length: > 0 };

    public SealInspectionViewModel(ITransactionsService transactionsService, IRfidScannerService rfidScannerService)
    {
        _transactionsService = transactionsService;
        _rfidScannerService = rfidScannerService;
    }


    [RelayCommand]
    private async Task StartAntennaAsync()
    {
        var result = await _rfidScannerService.StartAntennaAsync();
        StatusText = result.Message;
    }

    public async Task<bool> TryCaptureSealFromSdkAsync(int sealNumber)
    {
        var index = sealNumber - 1;
        if (index < 0 || index >= Seals.Count)
            return false;

        var read = await _rfidScannerService.TryReadSingleEpcAsync();
        if (!read.Success || string.IsNullOrWhiteSpace(read.Epc))
        {
            StatusText = read.Message;
            return false;
        }

        Seals[index].Code = read.Epc.Trim().ToUpperInvariant();
        StatusText = $"EPC capturado desde SDK: {Seals[index].Code}";
        OnPropertyChanged(nameof(CanUploadImages));
        OnPropertyChanged(nameof(CanSend));
        return true;
    }

    partial void OnContainerIdChanged(string value)
    {
        ContainerSuggestions.Clear();
        if (string.IsNullOrWhiteSpace(value))
            return;

        foreach (var item in _profiles.Keys.Where(k => k.Contains(value, StringComparison.OrdinalIgnoreCase)).Take(6))
        {
            ContainerSuggestions.Add(item);
        }
    }

    [RelayCommand]
    private void SelectContainer(string id)
    {
        ContainerId = id;
        ContainerSuggestions.Clear();
    }

    [RelayCommand]
    private void ReadSeal(object? sealParameter)
    {
        var index = ResolveSealIndex(sealParameter);
        if (index < 0 || index >= Seals.Count)
            return;

        var sealNumber = index + 1;

        if (SealEntryLocked && Seals[index].IsLocked)
            return;

        var code = Seals[index].Code?.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(code))
        {
            StatusText = $"Captura o pistolea primero el código del sello #{sealNumber}.";
            return;
        }

        Seals[index].Code = code;
        Seals[index].IsLocked = true;
        CurrentSealIndex = Math.Max(CurrentSealIndex, index + 1);

        if (CanUploadImages)
        {
            SealEntryLocked = true;
            StatusText = "4 sellos cargados. Ya puedes subir fotos.";
        }
        else
        {
            var next = Seals.FirstOrDefault(s => !s.IsLocked)?.Number ?? 4;
            StatusText = $"Sello #{sealNumber} leído. Continúa con #{next}.";
        }

        OnPropertyChanged(nameof(CanUploadImages));
        OnPropertyChanged(nameof(CanSend));
    }

    [RelayCommand]
    private void ToggleEditMode()
    {
        SealEntryLocked = !SealEntryLocked;
        foreach (var seal in Seals)
            seal.IsLocked = SealEntryLocked;

        StatusText = SealEntryLocked ? "Sellos bloqueados" : "Modo reemplazo activo";
    }

    [RelayCommand]
    private void Cancel()
    {
        ContainerId = string.Empty;
        SelectedActivationPoint = ActivationPoints[0];
        CurrentSealIndex = 0;
        SealEntryLocked = false;

        foreach (var seal in Seals)
        {
            seal.Code = string.Empty;
            seal.IsLocked = false;
        }

        foreach (var image in SealImages)
        {
            image.FileName = null;
            image.Bytes = null;
            image.Base64 = null;
            image.ValidationStatus = "idle";
        }

        ContainerImage.FileName = null;
        ContainerImage.Bytes = null;
        ContainerImage.Base64 = null;
        ContainerImage.ValidationStatus = "idle";
        StatusText = "Esperando lectura de sello #1";

        OnPropertyChanged(nameof(CanUploadImages));
        OnPropertyChanged(nameof(CanSend));
    }

    [RelayCommand]
    private async Task LoadSealImageAsync(object? sealParameter)
    {
        if (!CanUploadImages)
            return;

        var index = ResolveSealIndex(sealParameter);
        if (index < 0 || index >= SealImages.Count)
            return;

        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Selecciona imagen del sello",
            FileTypes = FilePickerFileType.Images
        });

        if (result is null)
            return;

        using var stream = await result.OpenReadAsync();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);

        var bytes = ms.ToArray();
        var image = SealImages[index];
        image.FileName = result.FileName;
        image.Bytes = bytes;
        image.Base64 = Convert.ToBase64String(bytes);
        image.ValidationStatus = "pending";

        var ok = await _transactionsService.ValidatePhotoAsync(image.Base64, image.FileName);
        image.ValidationStatus = ok ? "success" : "failed";
        StatusText = ok ? $"{image.Label} validada." : $"{image.Label} falló validación.";

        OnPropertyChanged(nameof(CanSend));
    }

    [RelayCommand]
    private async Task LoadContainerImageAsync()
    {
        if (!CanUploadImages)
            return;

        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Selecciona imagen del contenedor",
            FileTypes = FilePickerFileType.Images
        });

        if (result is null)
            return;

        using var stream = await result.OpenReadAsync();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);

        ContainerImage.FileName = result.FileName;
        ContainerImage.Bytes = ms.ToArray();
        ContainerImage.Base64 = Convert.ToBase64String(ContainerImage.Bytes);
        ContainerImage.ValidationStatus = "success";

        OnPropertyChanged(nameof(CanSend));
    }

    [RelayCommand]
    private async Task SendAsync()
    {
        if (!CanSend || string.IsNullOrWhiteSpace(ContainerId))
        {
            await Application.Current!.MainPage!.DisplayAlert("Faltan datos", "Debes cargar sellos, fotos y container ID.", "OK");
            return;
        }

        IsBusy = true;
        try
        {
            var profile = ResolveProfile();
            var xml = BuildXml(profile);
            var fields = new Dictionary<string, string>
            {
                ["eventId"] = "301",
                ["entityType"] = ContainerId.Trim().ToUpperInvariant(),
                ["entityId"] = profile.EntityId.ToString(),
                ["status"] = "1",
                ["isReefer"] = "false",
                ["details"] = $"RFID validated evidence - {SelectedActivationPoint}",
                ["document"] = "APP-MOBILE",
                ["xmlDetails"] = xml,
                ["recordDate"] = DateTime.UtcNow.ToString("O"),
                ["eventDate"] = DateTime.UtcNow.ToString("O")
            };

            var files = SealImages.Where(i => i.Bytes != null).Select(i => (i.FileName ?? $"seal-{i.Label}.jpg", i.Bytes!)).ToList();
            files.Add((ContainerImage.FileName ?? "container.jpg", ContainerImage.Bytes!));

            var sent = await _transactionsService.RegisterWithFilesAsync(fields, files);
            await Application.Current!.MainPage!.DisplayAlert(sent ? "Enviado" : "Error", sent ? "Transacción enviada." : "No se pudo enviar.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private ContainerProfile ResolveProfile() => _profiles.TryGetValue(ContainerId.Trim(), out var profile) ? profile : new ContainerProfile { EntityId = 100004 };

    private static int ResolveSealIndex(object? parameter)
    {
        if (parameter is int i)
            return i - 1;

        if (parameter is string s && int.TryParse(s, out var parsed))
            return parsed - 1;

        return -1;
    }

    private string BuildXml(ContainerProfile profile)
    {
        var seals = Seals.Select((s, i) => string.IsNullOrWhiteSpace(s.Code) ? $"SLL{i + 1}X" : s.Code).ToArray();
        var now = DateTime.UtcNow.ToString("O");
        var container = ContainerId.Trim().ToUpperInvariant();

        var sb = new StringBuilder();
        sb.Append("<Contenedor>");
        sb.Append($"<entity_id>{profile.EntityId}</entity_id><contenedor>{container}</contenedor>");
        sb.Append($"<activation_point>{SelectedActivationPoint}</activation_point>");
        sb.Append($"<tamaño>{profile.Size}</tamaño><naviera>{profile.ShippingLine}</naviera><mercancía>{profile.Goods}</mercancía>");
        sb.Append($"<booking>{profile.Booking}</booking><peso_origen>{profile.OriginWeight}</peso_origen>");
        sb.Append($"<sello-1>{seals[0]}</sello-1><sello-2>{seals[1]}</sello-2><sello-3>{seals[2]}</sello-3><sello-4>{seals[3]}</sello-4>");
        sb.Append($"<sello_aduana>{profile.CustomsSeal}</sello_aduana><observaciones>{profile.Observations}</observaciones>");
        sb.Append($"<fecha_registro>{now}</fecha_registro></Contenedor>");
        return sb.ToString();
    }
}
