using System.Net.Http.Json;
using OpsT360.Models;

namespace OpsT360.Services;

public class LoginService : ILoginService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthState _authState;
    private const string LoginUrl = "http://38.242.225.119:3000/api/auth/login";

    public LoginService(HttpClient httpClient, IAuthState authState)
    {
        _httpClient = httpClient;
        _authState = authState;
    }

    public async Task<string?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(LoginUrl, new
        {
            username = request.Username,
            password = request.Password,
            ip = request.Ip,
            device = request.Device
        }, cancellationToken);

        if (!response.IsSuccessStatusCode)
            return null;

        var payload = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken);
        var token = payload?.Data?.Token;
        if (!string.IsNullOrWhiteSpace(token))
        {
            _authState.SetToken(token);
        }

        return token;
    }
}
