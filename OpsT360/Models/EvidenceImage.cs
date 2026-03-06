namespace OpsT360.Models;

public sealed class EvidenceImage
{
    public string Label { get; init; } = string.Empty;
    public string? FileName { get; set; }
    public byte[]? Bytes { get; set; }
    public string? Base64 { get; set; }
    public string ValidationStatus { get; set; } = "idle";
}
