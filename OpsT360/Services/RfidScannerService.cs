namespace OpsT360.Services;

public partial class RfidScannerService : IRfidScannerService
{
    public Task<RfidReadResult> StartAntennaAsync(CancellationToken cancellationToken = default)
        => StartAntennaPlatformAsync(cancellationToken);

    public Task<RfidReadResult> TryReadSingleEpcAsync(CancellationToken cancellationToken = default)
        => TryReadSingleEpcPlatformAsync(cancellationToken);

    private partial Task<RfidReadResult> StartAntennaPlatformAsync(CancellationToken cancellationToken);
    private partial Task<RfidReadResult> TryReadSingleEpcPlatformAsync(CancellationToken cancellationToken);
}
