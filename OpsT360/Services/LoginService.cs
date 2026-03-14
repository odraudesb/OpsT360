using System.Net;
using System.Text;
using System.Text.Json;
using OpsT360.Models;

namespace OpsT360.Services;

public class LoginService : ILoginService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthState _authState;

    public const string PrimaryLoginUrl = "http://38.242.225.119:3000/api/auth/login";
    public static readonly string[] LoginFallbackUrls =
    {
        PrimaryLoginUrl,
        "http://38.242.225.119/api/auth/login",
        "https://38.242.225.119:3000/api/auth/login",
        "https://38.242.225.119/api/auth/login"
    };

    public LoginService(HttpClient httpClient, IAuthState authState)
    {
        _httpClient = httpClient;
        _authState = authState;
    }

    public async Task<string?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        Exception? lastError = null;

        foreach (var loginUrl in LoginFallbackUrls)
        {
            try
            {
                var token = await TryLoginEndpointAsync(loginUrl, request, cancellationToken);
                _authState.SetToken(token);
                return token;
            }
            catch (Exception ex)
            {
                lastError = ex;
            }
        }

        throw new HttpRequestException(
            $"No se pudo completar login. Endpoints probados: {string.Join(" | ", LoginFallbackUrls)}. Último error: {lastError?.Message}",
            lastError);
    }

    private async Task<string> TryLoginEndpointAsync(string loginUrl, LoginRequest request, CancellationToken cancellationToken)
    {
        var endpoint = new Uri(loginUrl, UriKind.Absolute);

        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        System.Diagnostics.Debug.WriteLine($"LOGIN URL: {endpoint}");
        System.Diagnostics.Debug.WriteLine($"LOGIN REQUEST: {json}");

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        httpRequest.Headers.Accept.ParseAdd("application/json");

        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        System.Diagnostics.Debug.WriteLine($"LOGIN STATUS: {(int)response.StatusCode}");
        System.Diagnostics.Debug.WriteLine($"LOGIN RESPONSE: {body}");

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new HttpRequestException(
                    $"Login API respondió 403 Forbidden en {endpoint}. Body: {body}");
            }

            throw new HttpRequestException(
                $"Login API respondió {(int)response.StatusCode} en {endpoint}. Body: {body}");
        }

        var payload = JsonSerializer.Deserialize<LoginResponse>(body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var token = payload?.Data?.Token;
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new JsonException($"Login sin token en respuesta de {endpoint}. Body: {body}");
        }

        return token;
    }
}
