namespace OpsT360.Services;

public interface IRfidScannerService
{
    Task<RfidReadResult> StartAntennaAsync(CancellationToken cancellationToken = default);
    Task<RfidReadResult> TryReadSingleEpcAsync(CancellationToken cancellationToken = default);
}

public sealed record RfidReadResult(bool Success, string? Epc, string? Tid, string Message)
{
    public static RfidReadResult Ok(string epc, string? tid = null) =>
        new(true, epc, tid, "Lectura RFID OK");

    public static RfidReadResult Fail(string message) =>
        new(false, null, null, message);
}