using System.Net.Http.Json;
using System.Text.Json;
using OpsT360.Models;

namespace OpsT360.Services;

public class LoginService : ILoginService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthState _authState;
    public const string LoginUrl = "http://38.242.225.119:3000/api/auth/login";

    public LoginService(HttpClient httpClient, IAuthState authState)
    {
        _httpClient = httpClient;
        _authState = authState;
    }

    public async Task<string?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var endpoint = new Uri(LoginUrl, UriKind.Absolute);
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = JsonContent.Create(new
            {
                username = request.Username,
                password = request.Password,
                ip = request.Ip,
                device = request.Device
            })
        };

        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Login API respondió {(int)response.StatusCode} en {endpoint}. Body: {body}");
        }

        var payload = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken);
        var token = payload?.Data?.Token;
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new JsonException($"Login sin token en respuesta de {endpoint}.");
        }

        _authState.SetToken(token);
        return token;
    }
}
