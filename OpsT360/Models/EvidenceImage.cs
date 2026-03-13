using CommunityToolkit.Mvvm.ComponentModel;

namespace OpsT360.Models;

public partial class EvidenceImage : ObservableObject
{
    public string Label { get; init; } = string.Empty;

    [ObservableProperty] private string? fileName;
    [ObservableProperty] private byte[]? bytes;
    [ObservableProperty] private string? base64;
    [ObservableProperty] private string validationStatus = "idle";

    public bool HasImage => Bytes is { Length: > 0 };

    public ImageSource? PreviewImage => Bytes is null
        ? null
        : ImageSource.FromStream(() => new MemoryStream(Bytes));

    partial void OnBytesChanged(byte[]? value)
    {
        OnPropertyChanged(nameof(PreviewImage));
        OnPropertyChanged(nameof(HasImage));
    }
}
