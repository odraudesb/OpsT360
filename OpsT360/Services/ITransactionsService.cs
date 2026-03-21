namespace OpsT360.Services;

public interface ITransactionsService
{
    Task<RoboflowValidationResult> ValidatePhotoAsync(string imageBase64, string fileName, CancellationToken cancellationToken = default);
    Task<bool> RegisterWithFilesAsync(Dictionary<string, string> fields, IEnumerable<(string FileName, byte[] Content)> files, CancellationToken cancellationToken = default);
    Task<bool> RegisterTransactionAsync(Dictionary<string, object> payload, CancellationToken cancellationToken = default);
    Task<bool> SendAlertMailAsync(string xmlDetails, string containerId, string eventName, string stepLabel, CancellationToken cancellationToken = default);
}
