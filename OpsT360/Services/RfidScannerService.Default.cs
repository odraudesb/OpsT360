#if !ANDROID
namespace OpsT360.Services;

public partial class RfidScannerService
{
    private partial Task<RfidReadResult> StartAntennaPlatformAsync(CancellationToken cancellationToken)
        => Task.FromResult(RfidReadResult.Fail("Activación de antena disponible solo en Android."));

    private partial Task<RfidReadResult> TryReadSingleEpcPlatformAsync(CancellationToken cancellationToken)
        => Task.FromResult(RfidReadResult.Fail("SDK RFID ST-E7100 disponible solo en Android."));

    private partial Task<RfidBatchReadResult> TryReadDistinctEpcsPlatformAsync(int maxCount, CancellationToken cancellationToken)
        => Task.FromResult(RfidBatchReadResult.Fail("SDK RFID ST-E7100 disponible solo en Android."));
}
#endif
