namespace OpsT360.Services;

public interface ITransactionsService
{
    Task<bool> ValidatePhotoAsync(string imageBase64, string fileName, CancellationToken cancellationToken = default);
    Task<bool> RegisterWithFilesAsync(Dictionary<string, string> fields, IEnumerable<(string FileName, byte[] Content)> files, CancellationToken cancellationToken = default);
}
