using OpsT360.Models;

namespace OpsT360.Services;

public interface ILoginService
{
    Task<string?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
