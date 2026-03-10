namespace OpsT360.Services;

public interface IRfidScannerService
{
    Task<RfidReadResult> TryReadSingleEpcAsync(CancellationToken cancellationToken = default);
}

public sealed record RfidReadResult(bool Success, string? Epc, string Message)
{
    public static RfidReadResult Ok(string epc) => new(true, epc, "Lectura RFID OK");
    public static RfidReadResult Fail(string message) => new(false, null, message);
}
