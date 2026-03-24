using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpsT360.Models;
using OpsT360.Services;

namespace OpsT360.ViewModels;

public partial class SealInspectionViewModel : ObservableObject
{
    private readonly ITransactionsService _transactionsService;
    private readonly IRfidScannerService _rfidScannerService;
    private readonly IAppLanguageState _languageState;

    private readonly Dictionary<string, ContainerProfile> _profiles = new(StringComparer.OrdinalIgnoreCase)
    {
        ["HSU3523430"] = new() { EntityId = 100004, Size = "40HC", ShippingLine = "HAPAG-LLOYD", OriginPort = "ECGYE", LoadingPort = "ECGYE", DischargePort = "USMIA", DestinationPort = "USMIA", Goods = "Piña fresca", Booking = "BK2025-000101", OriginWeight = 22340, TerminalEntryDate = "2025-09-10T08:15:00", PreNoticeDate = "2025-09-08T13:40:00", Carrier = "TRANSPORTES PACIFICO S.A.", Plate = "ABC101", Driver = "Conductor 1", CustomsSeal = "SAP561", Observations = "Contenedor refrigerado, temperatura 8°C" },
        ["TEMU3523471"] = new() { EntityId = 100003, Size = "40HC", ShippingLine = "ONE", OriginPort = "ECGYE", LoadingPort = "ECGYE", DischargePort = "USMIA", DestinationPort = "USMIA", Goods = "Mango fresco", Booking = "BK2025-000102", OriginWeight = 21980, TerminalEntryDate = "2025-09-10T09:00:00", PreNoticeDate = "2025-09-08T14:00:00", Carrier = "RUTAS DEL ECUADOR S.A.", Plate = "ABC102", Driver = "Conductor 2", CustomsSeal = "SAP562", Observations = "Cadena de frío activa 10°C" },
        ["SUDU3436921"] = new() { EntityId = 100002, Size = "40HC", ShippingLine = "HMM", OriginPort = "ECGYE", LoadingPort = "ECGYE", DischargePort = "USMIA", DestinationPort = "USMIA", Goods = "Papaya fresca", Booking = "BK2025-000103", OriginWeight = 22850, TerminalEntryDate = "2025-09-10T09:35:00", PreNoticeDate = "2025-09-08T15:00:00", Carrier = "LOGISTICA DEL LITORAL C.A.", Plate = "ABC103", Driver = "Conductor 3", CustomsSeal = "SAP563", Observations = "Sin novedades durante inspección" },
        ["MSCU8460995"] = new() { EntityId = 100001, Size = "40HC", ShippingLine = "MSC", OriginPort = "ECGYE", LoadingPort = "ECGYE", DischargePort = "USMIA", DestinationPort = "USMIA", Goods = "Banano fresco", Booking = "BK2025-000004", OriginWeight = 23400, TerminalEntryDate = "2025-09-09T08:00:00", PreNoticeDate = "2025-09-07T15:30:00", Carrier = "TRANSPORTES ANDES S.A.", Plate = "ABC124", Driver = "Conductor 4", CustomsSeal = "SAP564", Observations = "Contenedor refrigerado, temperatura 13°C" },
        ["MAEU5083343"] = new() { EntityId = 90009, Size = "40HC", ShippingLine = "MAERSK", OriginPort = "ECGYE", LoadingPort = "ECGYE", DischargePort = "USMIA", DestinationPort = "USMIA", Goods = "Pitahaya fresca", Booking = "BK2025-000105", OriginWeight = 21650, TerminalEntryDate = "2025-09-10T10:10:00", PreNoticeDate = "2025-09-08T15:20:00", Carrier = "ECUATORIANA DE CARGA S.A.", Plate = "ABC105", Driver = "Conductor 5", CustomsSeal = "SAP565", Observations = "Despacho prioritario exportación" },
        ["MAEU7429918"] = new() { EntityId = 80008, Size = "40HC", ShippingLine = "MAERSK", OriginPort = "ECGYE", LoadingPort = "ECGYE", DischargePort = "USMIA", DestinationPort = "USMIA", Goods = "Plátano orgánico", Booking = "BK2025-000106", OriginWeight = 23020, TerminalEntryDate = "2025-09-10T10:55:00", PreNoticeDate = "2025-09-08T16:00:00", Carrier = "RUTA COSTERA LOGISTICS", Plate = "ABC106", Driver = "Conductor 6", CustomsSeal = "SAP566", Observations = "Contenedor pre-enfriado 11°C" },
        ["MAEU9045524"] = new() { EntityId = 70007, Size = "40HC", ShippingLine = "MAERSK", OriginPort = "ECGYE", LoadingPort = "ECGYE", DischargePort = "USMIA", DestinationPort = "USMIA", Goods = "Cacao en grano", Booking = "BK2025-000107", OriginWeight = 21000, TerminalEntryDate = "2025-09-10T11:25:00", PreNoticeDate = "2025-09-08T16:40:00", Carrier = "TRAMAR CARGA ECUADOR", Plate = "ABC107", Driver = "Conductor 7", CustomsSeal = "SAP567", Observations = "Sellado y pesado en origen" },
        ["MAEU6651037"] = new() { EntityId = 60006, Size = "40HC", ShippingLine = "MAERSK", OriginPort = "ECGYE", LoadingPort = "ECGYE", DischargePort = "USMIA", DestinationPort = "USMIA", Goods = "Yuca procesada", Booking = "BK2025-000108", OriginWeight = 20540, TerminalEntryDate = "2025-09-10T12:05:00", PreNoticeDate = "2025-09-08T17:10:00", Carrier = "ANDINA FREIGHT MOVERS", Plate = "ABC108", Driver = "Conductor 8", CustomsSeal = "SAP568", Observations = "Unidad apta para carga seca" },
        ["CMAU9921401"] = new() { EntityId = 40004, Size = "40HC", ShippingLine = "CMA CGM", OriginPort = "ECGYE", LoadingPort = "ECGYE", DischargePort = "USMIA", DestinationPort = "USMIA", Goods = "Atún en conserva", Booking = "BK2025-000109", OriginWeight = 19870, TerminalEntryDate = "2025-09-10T12:45:00", PreNoticeDate = "2025-09-08T17:35:00", Carrier = "LOGISTICA ECUAMAR", Plate = "ABC109", Driver = "Conductor 9", CustomsSeal = "SAP569", Observations = "Inspección documental completada" },
        ["CMAU7004129"] = new() { EntityId = 30003, Size = "40HC", ShippingLine = "CMA CGM", OriginPort = "ECGYE", LoadingPort = "ECGYE", DischargePort = "USMIA", DestinationPort = "USMIA", Goods = "Camarón congelado", Booking = "BK2025-000110", OriginWeight = 22560, TerminalEntryDate = "2025-09-10T13:20:00", PreNoticeDate = "2025-09-08T18:00:00", Carrier = "TRANSCOLD ECUADOR", Plate = "ABC110", Driver = "Conductor 10", CustomsSeal = "SAP570", Observations = "Reefer configurado a -18°C" },
        ["CMAU5573305"] = new() { EntityId = 20002, Size = "40HC", ShippingLine = "CMA CGM", OriginPort = "ECGYE", LoadingPort = "ECGYE", DischargePort = "USMIA", DestinationPort = "USMIA", Goods = "Maracuyá fresca", Booking = "BK2025-000111", OriginWeight = 21480, TerminalEntryDate = "2025-09-10T14:00:00", PreNoticeDate = "2025-09-08T18:20:00", Carrier = "CARGA EXPRESS COSTA", Plate = "ABC111", Driver = "Conductor 11", CustomsSeal = "SAP571", Observations = "Sello aduana verificado en patio" },
        ["CMAU2849912"] = new() { EntityId = 20001, Size = "40HC", ShippingLine = "CMA CGM", OriginPort = "ECGYE", LoadingPort = "ECGYE", DischargePort = "USMIA", DestinationPort = "USMIA", Goods = "Uva de mesa", Booking = "BK2025-000112", OriginWeight = 22110, TerminalEntryDate = "2025-09-10T14:30:00", PreNoticeDate = "2025-09-08T18:40:00", Carrier = "PACIFIC TRUCKING GROUP", Plate = "ABC112", Driver = "Conductor 12", CustomsSeal = "SAP572", Observations = "Carga sensible, manipulación cuidadosa" },
        ["TEMU0927644"] = new() { EntityId = 12345, Size = "40HC", ShippingLine = "ONE", OriginPort = "ECGYE", LoadingPort = "ECGYE", DischargePort = "USMIA", DestinationPort = "USMIA", Goods = "Arándano fresco", Booking = "BK2025-000113", OriginWeight = 20880, TerminalEntryDate = "2025-09-10T15:00:00", PreNoticeDate = "2025-09-08T19:00:00", Carrier = "NORTE SUR TRANSPORT", Plate = "ABC113", Driver = "Conductor 13", CustomsSeal = "SAP573", Observations = "Control térmico continuo 6°C" }
    };

    private static readonly Regex EpcHexRegex = new("^[0-9A-F]{8,64}$", RegexOptions.Compiled);
    private static readonly Regex GenericHexRegex = new("^[0-9A-F]{4,128}$", RegexOptions.Compiled);
    private static readonly TimeSpan RfidReadTimeout = TimeSpan.FromSeconds(8);
    private static readonly TimeSpan RfidBatchReadTimeout = TimeSpan.FromSeconds(14);
    private const int RfidSealPlacementEventId = 8;
    private const string RfidSealPlacementEventName = "Colocación de Sello RFID previo Ingreso";
    private const int RfidSealPlacementFailureEventId = 32;
    private const string RfidSealPlacementFailureEventName = "Fallo en colocación de Etiqueta RFID";
    private static readonly bool EnableFailureAlertEmail = false; // Temporal para demo APK.

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
    [ObservableProperty] private string statusText = "Tap Read Seals to activate the antenna and capture EPC values (ST-E100).";
    [ObservableProperty] private bool isInspectionChangeMode;
    [ObservableProperty] private string operationTitle = "RFID Seal [Label] Placement";
    [ObservableProperty] private string containerPlaceholder = "Container Number";
    [ObservableProperty] private string panel1ButtonText = "Photo Panel # 1";
    [ObservableProperty] private string panel2ButtonText = "Photo Panel # 2";
    [ObservableProperty] private string readSealsButtonText = "Read Seals";
    [ObservableProperty] private string seal1Placeholder = "Seal #1";
    [ObservableProperty] private string seal2Placeholder = "Seal #2";
    [ObservableProperty] private string seal3Placeholder = "Seal #3";
    [ObservableProperty] private string seal4Placeholder = "Seal #4";
    [ObservableProperty] private string containerPhotoButtonText = "Container Photo";

    public bool AreAllSealsCaptured => Seals.All(s => !string.IsNullOrWhiteSpace(s.Code));
    public bool CanUploadImages => true;
    public bool CanSend => AreAllSealsCaptured
                           && SealImages.Take(2).All(i => i.Bytes is { Length: > 0 })
                           && ContainerImage.Bytes is { Length: > 0 };

    public SealInspectionViewModel(ITransactionsService transactionsService, IRfidScannerService rfidScannerService, IAppLanguageState languageState)
    {
        _transactionsService = transactionsService;
        _rfidScannerService = rfidScannerService;
        _languageState = languageState;
        _languageState.LanguageChanged += OnLanguageChanged;
        ApplyLanguageResources(_languageState.IsEnglish);
    }

    public void ConfigureOperationMode(bool isInspectionChange)
    {
        IsInspectionChangeMode = isInspectionChange;
        ApplyLanguageResources(_languageState.IsEnglish);
    }

    private void OnLanguageChanged(bool isEnglish)
    {
        ApplyLanguageResources(isEnglish);
    }

    private void ApplyLanguageResources(bool isEnglish)
    {
        if (isEnglish)
        {
            ContainerPlaceholder = "Container Number";
            Panel1ButtonText = "Photo Panel # 1";
            Panel2ButtonText = "Photo Panel # 2";
            ReadSealsButtonText = "Read Seals";
            Seal1Placeholder = "Seal #1";
            Seal2Placeholder = "Seal #2";
            Seal3Placeholder = "Seal #3";
            Seal4Placeholder = "Seal #4";
            ContainerPhotoButtonText = "Container Photo";
            OperationTitle = IsInspectionChangeMode ? "RFID Seal Inspection Change" : "RFID Seal [Label] Placement";
            StatusText = IsInspectionChangeMode
                ? "Inspection mode active: previous seals must be Deactivated (Reason: Inspection) before registering new ones."
                : "Tap Read Seals to activate the antenna and capture EPC values (ST-E100).";
            return;
        }

        ContainerPlaceholder = "Número de Contenedor";
        Panel1ButtonText = "Foto panel # 1";
        Panel2ButtonText = "Foto Panel # 2";
        ReadSealsButtonText = "Leer Sellos";
        Seal1Placeholder = "Sello #1";
        Seal2Placeholder = "Sello #2";
        Seal3Placeholder = "Sello #3";
        Seal4Placeholder = "Sello #4";
        ContainerPhotoButtonText = "Foto Contenedor";
        OperationTitle = IsInspectionChangeMode ? "Cambio de Sellos por Inspección" : "RFID Seal [Label] Placement";
        StatusText = IsInspectionChangeMode
            ? "Modo inspección activo: desactiva los sellos previos (Reason: Inspection) antes de registrar los nuevos."
            : "Pulsa Leer Sellos para activar la antena y capturar EPC (ST-E100).";
    }

    public async Task<bool> TryCaptureSealFromSdkAsync(int sealNumber)
    {
        var index = sealNumber - 1;
        if (index < 0 || index >= Seals.Count)
            return false;

        if (IsBusy)
        {
            StatusText = "RFID read in progress. Please wait...";
            return false;
        }

        IsBusy = true;
        try
        {
            using var cts = new CancellationTokenSource(RfidReadTimeout);
            var read = await _rfidScannerService.TryReadSingleEpcAsync(cts.Token);

            if (!read.Success || string.IsNullOrWhiteSpace(read.Epc))
            {
                StatusText = read.Message;
                return false;
            }

            var normalizedEpc = NormalizeEpc(read.Epc);
            if (string.IsNullOrWhiteSpace(normalizedEpc))
            {
                StatusText = $"Reader returned a non-EPC value ({read.Epc}). Check RFID/UHF configuration on the handheld.";
                return false;
            }

            Seals[index].Code = normalizedEpc;
          //  Seals[index].Tid = NormalizeHex(read.Tid) ?? string.Empty;

            StatusText = $"EPC captured from SDK: {Seals[index].Code}";
            OnPropertyChanged(nameof(CanUploadImages));
            OnPropertyChanged(nameof(CanSend));
            return true;
        }
        catch (OperationCanceledException)
        {
            StatusText = "RFID timeout: no tag detected within 8s. Move the seal closer and retry.";
            return false;
        }
        catch (Exception ex)
        {
            StatusText = $"Unexpected RFID read error: {ex.Message}";
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task<int> TryCaptureRemainingSealsFromSdkAsync()
    {
        if (IsBusy)
        {
            StatusText = "RFID read in progress. Please wait...";
            return 0;
        }

        var pendingIndexes = Seals
            .Select((seal, index) => new { seal, index })
            .Where(x => !x.seal.IsLocked)
            .Select(x => x.index)
            .ToList();

        if (pendingIndexes.Count == 0)
        {
            StatusText = "All seals were already read.";
            return 0;
        }

        var existing = new HashSet<string>(
            Seals.Select(s => NormalizeEpc(s.Code)).Where(code => !string.IsNullOrWhiteSpace(code))!,
            StringComparer.OrdinalIgnoreCase);

        IsBusy = true;
        try
        {
            using var cts = new CancellationTokenSource(RfidBatchReadTimeout);
            var batch = await _rfidScannerService.TryReadDistinctEpcsAsync(4, cts.Token);
            if (!batch.Success || batch.Epcs.Count == 0)
            {
                StatusText = batch.Message;
                return 0;
            }

            var loaded = 0;
            foreach (var rawEpc in batch.Epcs)
            {
                var epc = NormalizeEpc(rawEpc);
                if (string.IsNullOrWhiteSpace(epc) || existing.Contains(epc))
                    continue;

                if (loaded >= pendingIndexes.Count)
                    break;

                var targetIndex = pendingIndexes[loaded];
                Seals[targetIndex].Code = epc;
                ReadSeal((targetIndex + 1).ToString());
                existing.Add(epc);
                loaded++;
            }

            if (loaded == 0)
            {
                StatusText = "RFID tags were detected, but none were new EPC values for pending seals.";
                return 0;
            }

            var remaining = Seals.Count(s => !s.IsLocked);
            StatusText = remaining == 0
                ? "4 seals captured automatically with distinct EPC values."
                : $"{loaded} new EPC(s) captured. {remaining} seal(s) remaining.";

            OnPropertyChanged(nameof(CanUploadImages));
            OnPropertyChanged(nameof(CanSend));
            return loaded;
        }
        catch (OperationCanceledException)
        {
            StatusText = "RFID timeout: multi-seal read did not complete.";
            return 0;
        }
        catch (Exception ex)
        {
            StatusText = $"Unexpected RFID multi-read error: {ex.Message}";
            return 0;
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnContainerIdChanged(string value)
    {
        ContainerSuggestions.Clear();
        if (string.IsNullOrWhiteSpace(value))
            return;

        foreach (var item in _profiles.Keys.Where(k => k.Contains(value, StringComparison.OrdinalIgnoreCase)).Take(6))
            ContainerSuggestions.Add(item);
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

        var code = NormalizeEpc(Seals[index].Code);
        if (string.IsNullOrWhiteSpace(code))
        {
            StatusText = $"Seal #{sealNumber} does not contain a valid RFID EPC (hex). Use Read seal (RFID), not barcode mode.";
            return;
        }

        Seals[index].Code = code;
        Seals[index].Tid = NormalizeHex(Seals[index].Tid) ?? string.Empty;
        Seals[index].IsLocked = true;
        CurrentSealIndex = Math.Max(CurrentSealIndex, index + 1);

        if (AreAllSealsCaptured)
        {
            SealEntryLocked = true;
            StatusText = "4 seals loaded. You can upload photos now.";
        }
        else
        {
            var next = Seals.FirstOrDefault(s => !s.IsLocked)?.Number ?? 4;
            StatusText = $"Seal #{sealNumber} captured. Continue with #{next}.";
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

        StatusText = SealEntryLocked ? "Seals locked" : "Replacement mode active";
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
            seal.Tid = string.Empty;
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
        StatusText = "Waiting for seal #1 read.";

        OnPropertyChanged(nameof(CanUploadImages));
        OnPropertyChanged(nameof(CanSend));
    }

    private async Task<FileResult?> CapturePhotoAsync(string title)
    {
        var mainPage = Application.Current?.MainPage;
        if (mainPage is null)
            return null;
        
        if (!MediaPicker.Default.IsCaptureSupported)
        {
            await mainPage.DisplayAlert("Camera unavailable", "This device cannot capture from camera.", "OK");
            return null;
        }

        return await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
        {
            Title = title
        });
    }

    [RelayCommand]
    private async Task LoadSealImageAsync(object? sealParameter)
    {
        var index = ResolveSealIndex(sealParameter);
        if (index < 0 || index >= SealImages.Count)
            return;

        FileResult? result;
        try
        {
            result = await CapturePhotoAsync("Take seal photo");
        }
        catch (Exception ex)
        {
            StatusText = $"Could not open camera/files: {ex.Message}";
            await ShowErrorAlertAsync("Photo load error", StatusText);
            return;
        }

        if (result is null)
        {
            await ShowErrorAlertAsync("Upload canceled", "No photo selected for this panel.");
            return;
        }

        using var stream = await result.OpenReadAsync();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);

        var bytes = ms.ToArray();
        var image = SealImages[index];
        image.FileName = result.FileName;
        image.Bytes = bytes;
        image.Base64 = Convert.ToBase64String(bytes);
        image.ValidationStatus = "idle";

        OnPropertyChanged(nameof(CanSend));
    }

    [RelayCommand]
    private async Task LoadContainerImageAsync()
    {
        FileResult? result;
        try
        {
            result = await CapturePhotoAsync("Take container photo");
        }
        catch (Exception ex)
        {
            StatusText = $"Could not open camera/files: {ex.Message}";
            await ShowErrorAlertAsync("Photo load error", StatusText);
            return;
        }

        if (result is null)
        {
            await ShowErrorAlertAsync("Upload canceled", "No container photo selected.");
            return;
        }

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
        var missing = new List<string>();
        if (string.IsNullOrWhiteSpace(ContainerId))
            missing.Add(_languageState.IsEnglish ? "container number" : "número de contenedor");
        if (!AreAllSealsCaptured)
            missing.Add(_languageState.IsEnglish ? "4 seal EPCs" : "4 EPC de sellos");
        if (SealImages.Take(2).Any(i => i.Bytes is not { Length: > 0 }))
            missing.Add(_languageState.IsEnglish ? "2 panel photos" : "2 fotos de panel");
        if (ContainerImage.Bytes is not { Length: > 0 })
            missing.Add(_languageState.IsEnglish ? "container photo" : "foto del contenedor");

        if (missing.Count > 0)
        {
            var title = _languageState.IsEnglish ? "Missing data" : "Datos incompletos";
            var msg = _languageState.IsEnglish
                ? $"Complete: {string.Join(", ", missing)}."
                : $"Complete: {string.Join(", ", missing)}.";
            await Application.Current!.MainPage!.DisplayAlert(title, msg, "OK");
            return;
        }

        IsBusy = true;
        try
        {
            var failedPanels = await ValidateAccessPanelsAsync();

            var profile = ResolveProfile();
            var xml = BuildXml(profile, RfidSealPlacementEventId, RfidSealPlacementEventName);
            var now = DateTime.UtcNow.ToString("O");
            var containerId = ContainerId.Trim().ToUpperInvariant();

            var fields = new Dictionary<string, string>
            {
                ["eventId"] = RfidSealPlacementEventId.ToString(),
                ["entityType"] = containerId,
                ["entityId"] = profile.EntityId.ToString(),
                ["status"] = "1",
                ["isReefer"] = "false",
                ["details"] = RfidSealPlacementEventName,
                ["document"] = "PRE-GATE",
                ["xmlDetails"] = xml,
                ["recordDate"] = now,
                ["eventDate"] = now
            };

            var files = SealImages
                .Take(2)
                .Where(i => i.Bytes != null)
                .Select(i => (i.FileName ?? $"seal-{i.Label}.jpg", i.Bytes!))
                .ToList();

            files.Add((ContainerImage.FileName ?? "container.jpg", ContainerImage.Bytes!));

            var sent = false;
            Exception? sendException = null;
            try
            {
                sent = await _transactionsService.RegisterWithFilesAsync(fields, files);
            }
            catch (Exception ex)
            {
                sendException = ex;
            }
            var hasFailures = failedPanels.Count > 0;
            var sentFailureEvent = false;
            var sentFailureMail = false;

            if (hasFailures && sent)
            {
                try
                {
                    var failureXml = BuildValidationFailureXml(profile, failedPanels, now);
                    var failedPhotoFiles = SealImages
                        .Take(2)
                        .Where(panel => failedPanels.Contains(panel.Label, StringComparer.OrdinalIgnoreCase))
                        .Select(panel => (panel.FileName ?? $"seal-{panel.Label}.jpg", panel.Bytes))
                        .Where(x => x.Bytes is { Length: > 0 })
                        .Select(x => (x.Item1, x.Bytes!))
                        .ToList();

                    var failureFields = BuildValidationFailureFields(profile, containerId, failureXml, now);
                    sentFailureEvent = failedPhotoFiles.Count > 0
                        ? await _transactionsService.RegisterWithFilesAsync(failureFields, failedPhotoFiles)
                        : await _transactionsService.RegisterTransactionAsync(BuildValidationFailurePayload(profile, containerId, failureXml, now));

                    if (EnableFailureAlertEmail)
                    {
                        sentFailureMail = await _transactionsService.SendAlertMailAsync(
                            failureXml,
                            containerId,
                            RfidSealPlacementFailureEventName,
                            "PRE-GATE");
                    }
                }
                catch (Exception alertEx)
                {
                    sentFailureEvent = false;
                    sentFailureMail = false;
                    System.Diagnostics.Debug.WriteLine($"ALERT FLOW ERROR: {alertEx}");
                }
            }

            var message = sent
                ? hasFailures
                    ? $"Transaction sent. Validation alerts: {string.Join(", ", failedPanels)}. " +
                      $"History alert: {(sentFailureEvent ? "OK" : "ERROR")} | Email alert: {(EnableFailureAlertEmail ? (sentFailureMail ? "OK" : "ERROR") : "SKIPPED")}."
                    : "Transaction sent successfully."
                : $"Could not send transaction.{(sendException is null ? string.Empty : $" {sendException.Message}")}";

            await Application.Current!.MainPage!.DisplayAlert(sent ? "Sent" : "Error", message, "OK");
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("Error", $"Error sending transaction: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task<List<string>> ValidateAccessPanelsAsync()
    {
        var failedPanels = new List<string>();

        foreach (var panel in SealImages.Take(2))
        {
            if (string.IsNullOrWhiteSpace(panel.Base64) || string.IsNullOrWhiteSpace(panel.FileName))
            {
                panel.ValidationStatus = "failed";
                failedPanels.Add(panel.Label);
                continue;
            }

            panel.ValidationStatus = "pending";
            try
            {
                var result = await _transactionsService.ValidatePhotoAsync(panel.Base64, panel.FileName);

                var validatedSource = result.ValidatedImageBase64 ?? result.OutputImageBase64;
                if (!string.IsNullOrWhiteSpace(validatedSource))
                {
                    var validatedBytes = await ResolveValidatedImageBytesAsync(validatedSource);
                    if (validatedBytes is { Length: > 0 })
                    {
                        panel.Bytes = validatedBytes;
                        panel.Base64 = Convert.ToBase64String(validatedBytes);
                        panel.FileName = AppendValidatedSuffix(panel.FileName);
                    }
                }

                panel.ValidationStatus = result.IsSuccessful ? "success" : "failed";
                if (!result.IsSuccessful)
                {
                    failedPanels.Add(panel.Label);
                }
            }
            catch (Exception ex)
            {
                panel.ValidationStatus = "failed";
                failedPanels.Add(panel.Label);
                StatusText = $"Validation error on {panel.Label}: {ex.Message}";
            }
        }

        return failedPanels;
    }

    private static async Task<byte[]?> ResolveValidatedImageBytesAsync(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (value.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            return DecodeDataUrlToBytes(value);

        if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            if (uri.Scheme is "http" or "https")
            {
                try
                {
                    using var client = new HttpClient();
                    return await client.GetByteArrayAsync(uri);
                }
                catch
                {
                    return null;
                }
            }

            if (uri.Scheme == "file")
            {
                var localPath = uri.LocalPath;
                if (File.Exists(localPath))
                    return await File.ReadAllBytesAsync(localPath);
            }
        }

        if (File.Exists(value))
            return await File.ReadAllBytesAsync(value);

        return DecodeDataUrlToBytes($"data:image/jpeg;base64,{value}");
    }

    private static byte[]? DecodeDataUrlToBytes(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var payload = value;
        var commaIndex = value.IndexOf(',');
        if (value.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && commaIndex >= 0)
            payload = value[(commaIndex + 1)..];

        try
        {
            return Convert.FromBase64String(payload);
        }
        catch
        {
            return null;
        }
    }

    private static string AppendValidatedSuffix(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        var name = Path.GetFileNameWithoutExtension(fileName);
        extension = string.IsNullOrWhiteSpace(extension) ? ".jpg" : extension;
        return $"{name}-validated{extension}";
    }

    private async Task ShowErrorAlertAsync(string title, string message)
    {
        var mainPage = Application.Current?.MainPage;
        if (mainPage is null)
            return;

        await MainThread.InvokeOnMainThreadAsync(() => mainPage.DisplayAlert(title, message, "OK"));
    }

    private ContainerProfile ResolveProfile() =>
        _profiles.TryGetValue(ContainerId.Trim(), out var profile)
            ? profile
            : new ContainerProfile { EntityId = 100004 };

    private static string? NormalizeEpc(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized = value.Trim().ToUpperInvariant();
        return EpcHexRegex.IsMatch(normalized) ? normalized : null;
    }

    private static string? NormalizeHex(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized = value.Trim().ToUpperInvariant();
        return GenericHexRegex.IsMatch(normalized) ? normalized : null;
    }

    private static int ResolveSealIndex(object? parameter)
    {
        if (parameter is int i)
            return i - 1;

        if (parameter is string s && int.TryParse(s, out var parsed))
            return parsed - 1;

        return -1;
    }

    private string BuildXml(ContainerProfile profile, int eventId, string eventName)
    {
        var seals = Seals
            .Select((s, i) => string.IsNullOrWhiteSpace(s.Code) ? $"SLL{i + 1}X" : s.Code.Trim().ToUpperInvariant())
            .ToArray();

        var now = DateTime.UtcNow.ToString("O");
        var container = ContainerId.Trim().ToUpperInvariant();

        var sb = new StringBuilder();
        sb.Append("<Contenedor>");
        sb.Append($"<event_id>{eventId}</event_id><event_name>{eventName}</event_name>");
        sb.Append("<document>PRE-GATE</document>");
        sb.Append($"<entity_id>{profile.EntityId}</entity_id><contenedor>{container}</contenedor>");
        sb.Append($"<activation_point>{SelectedActivationPoint}</activation_point>");
        sb.Append($"<tamaño>{profile.Size}</tamaño><naviera>{profile.ShippingLine}</naviera>");
        sb.Append($"<puerto_origen>{profile.OriginPort}</puerto_origen><puerto_carga>{profile.LoadingPort}</puerto_carga>");
        sb.Append($"<puerto_descarga>{profile.DischargePort}</puerto_descarga><puerto_destino>{profile.DestinationPort}</puerto_destino>");
        sb.Append($"<mercancía>{profile.Goods}</mercancía><booking>{profile.Booking}</booking><peso_origen>{profile.OriginWeight}</peso_origen>");
        sb.Append($"<fecha_ingreso_terminal>{profile.TerminalEntryDate}</fecha_ingreso_terminal><fecha_preaviso>{profile.PreNoticeDate}</fecha_preaviso>");
        sb.Append($"<transportista>{profile.Carrier}</transportista><placa>{profile.Plate}</placa><conductor>{profile.Driver}</conductor>");
        sb.Append($"<sello-1>{seals[0]}</sello-1><sello-2>{seals[1]}</sello-2><sello-3>{seals[2]}</sello-3><sello-4>{seals[3]}</sello-4>");
        sb.Append($"<sello_aduana>{profile.CustomsSeal}</sello_aduana><observaciones>{profile.Observations}</observaciones>");
        sb.Append($"<fecha_registro>{now}</fecha_registro>");
        sb.Append("</Contenedor>");
        return sb.ToString();
    }

    private string BuildValidationFailureXml(ContainerProfile profile, IReadOnlyCollection<string> failedPanels, string now)
    {
        var seals = Seals
            .Select((s, i) => string.IsNullOrWhiteSpace(s.Code) ? $"SLL{i + 1}X" : s.Code.Trim().ToUpperInvariant())
            .ToArray();
        var container = ContainerId.Trim().ToUpperInvariant();
        var failedPhotoNames = string.Join("; ", failedPanels.Where(x => !string.IsNullOrWhiteSpace(x)));

        var sb = new StringBuilder();
        sb.Append("<Contenedor>");
        sb.Append($"<event_id>{RfidSealPlacementFailureEventId}</event_id>");
        sb.Append($"<event_name>{RfidSealPlacementFailureEventName}</event_name>");
        sb.Append("<document>PRE-GATE</document>");
        sb.Append($"<entity_id>{profile.EntityId}</entity_id><contenedor>{container}</contenedor>");
        sb.Append($"<activation_point>{SelectedActivationPoint}</activation_point>");
        sb.Append($"<tamaño>{profile.Size}</tamaño><naviera>{profile.ShippingLine}</naviera>");
        sb.Append($"<puerto_origen>{profile.OriginPort}</puerto_origen><puerto_carga>{profile.LoadingPort}</puerto_carga>");
        sb.Append($"<puerto_descarga>{profile.DischargePort}</puerto_descarga><puerto_destino>{profile.DestinationPort}</puerto_destino>");
        sb.Append($"<mercancía>{profile.Goods}</mercancía><booking>{profile.Booking}</booking><peso_origen>{profile.OriginWeight}</peso_origen>");
        sb.Append($"<fecha_ingreso_terminal>{profile.TerminalEntryDate}</fecha_ingreso_terminal><fecha_preaviso>{profile.PreNoticeDate}</fecha_preaviso>");
        sb.Append($"<transportista>{profile.Carrier}</transportista><placa>{profile.Plate}</placa><conductor>{profile.Driver}</conductor>");
        sb.Append($"<sello-1>{seals[0]}</sello-1><sello-2>{seals[1]}</sello-2><sello-3>{seals[2]}</sello-3><sello-4>{seals[3]}</sello-4>");
        sb.Append($"<sello_aduana>{profile.CustomsSeal}</sello_aduana>");
        sb.Append($"<failed_photos>{failedPanels.Count}</failed_photos>");
        sb.Append($"<failed_photo_names>{(string.IsNullOrWhiteSpace(failedPhotoNames) ? "Sin detalle" : failedPhotoNames)}</failed_photo_names>");
        sb.Append("<failure_type>NO LEYERON LAS FOTOS DE VALIDACION</failure_type>");
        sb.Append($"<fecha_registro>{now}</fecha_registro>");
        sb.Append("</Contenedor>");
        return sb.ToString();
    }

    private static Dictionary<string, object> BuildValidationFailurePayload(ContainerProfile profile, string containerId, string failureXml, string now)
    {
        return new Dictionary<string, object>
        {
            ["eventId"] = RfidSealPlacementFailureEventId,
            ["entityType"] = containerId,
            ["entityId"] = profile.EntityId,
            ["recordDate"] = now,
            ["eventDate"] = now,
            ["xmlDetails"] = failureXml,
            ["details"] = $"Fallo de validación de fotos para {containerId}",
            ["document"] = "PRE-GATE",
            ["status"] = 1,
            ["eta"] = now,
            ["ship"] = "RFID Simulator",
            ["categoryId"] = 1,
            ["locationStatus"] = "Validated",
            ["isReefer"] = false
        };
    }

    private static Dictionary<string, string> BuildValidationFailureFields(ContainerProfile profile, string containerId, string failureXml, string now)
    {
        return new Dictionary<string, string>
        {
            ["eventId"] = RfidSealPlacementFailureEventId.ToString(),
            ["entityType"] = containerId,
            ["entityId"] = profile.EntityId.ToString(),
            ["status"] = "1",
            ["isReefer"] = "false",
            ["details"] = $"Fallo de validación de fotos para {containerId}",
            ["document"] = "PRE-GATE",
            ["xmlDetails"] = failureXml,
            ["recordDate"] = now,
            ["eventDate"] = now
        };
    }
}
