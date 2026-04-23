using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace OpsT360.Services;

public sealed class TransactionsService : ITransactionsService
{
    private readonly HttpClient _httpClient;
    private readonly RoboflowValidationService _roboflowValidationService;
    private readonly IAuthState _authState;

    private static readonly string RegisterWithFilesUrl = Environment.GetEnvironmentVariable("TRANSACTIONS_REGISTER_WITH_FILES_URL")
        ?? "http://38.242.225.119:3000/api/transactions/register-with-files";
    private static readonly string RegisterUrl = Environment.GetEnvironmentVariable("TRANSACTIONS_REGISTER_URL")
        ?? "http://38.242.225.119:3000/api/transactions/register";
    private static readonly string CompanySettingsUrl = Environment.GetEnvironmentVariable("COMPANY_SETTINGS_URL")
        ?? "http://38.242.225.119:3000/api/company-setting";
    private static readonly string[] AlertMailUrls =
    {
        "http://38.242.225.119:3000/api/emails",
        "http://38.242.225.119:3000/api/emails/send",
        "http://38.242.225.119:3000/api/emails/send-email"
    };
    private const string AlertMailTo = "rmurillo@infraportus.com";
    private const string AlertMailCc = "edu1991e@gmail.com";

    private readonly SemaphoreSlim _settingsLock = new(1, 1);
    private CompanySettingsSnapshot? _settingsSnapshot;

    private static readonly CompanySettingsSnapshot FallbackSettings = new(
        "https://serverless.roboflow.com",
        "mi-workspace-sihjw",
        "detect-count-and-visualize-4",
        "8OQBCU7lFbC9ogYMmbB7",
        2);

    public TransactionsService(HttpClient httpClient, RoboflowValidationService roboflowValidationService, IAuthState authState)
    {
        _httpClient = httpClient;
        _roboflowValidationService = roboflowValidationService;
        _authState = authState;
    }

    public async Task<RoboflowValidationResult> ValidatePhotoAsync(string imageBase64, string fileName, CancellationToken cancellationToken = default)
    {
        var settings = await GetCompanySettingsSnapshotAsync(cancellationToken);
        var request = new
        {
            api_key = settings.RoboflowApiKey,
            inputs = new
            {
                image = new
                {
                    type = "base64",
                    value = imageBase64
                }
            },
            use_cache = true,
            fileName
        };

        var roboflowUrl = $"{settings.RoboflowBaseUrl.TrimEnd('/')}/{settings.RoboflowWorkspace}/workflows/{settings.RoboflowWorkflow}";
        var response = await _httpClient.PostAsJsonAsync(roboflowUrl, request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return new RoboflowValidationResult(false, null, null, Array.Empty<ValidationBox>());
        }

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(responseJson))
            return new RoboflowValidationResult(false, null, null, Array.Empty<ValidationBox>());

        JsonNode? node;
        try
        {
            node = JsonNode.Parse(responseJson);
        }
        catch
        {
            return new RoboflowValidationResult(false, null, null, Array.Empty<ValidationBox>());
        }

        return _roboflowValidationService.AnalyzeValidation(node, imageBase64);
    }

    public async Task<int> GetValidationLabelCountAsync(CancellationToken cancellationToken = default)
    {
        var settings = await GetCompanySettingsSnapshotAsync(cancellationToken);
        return settings.ValidationLabelCount;
    }

    public async Task<bool> RegisterWithFilesAsync(Dictionary<string, string> fields, IEnumerable<(string FileName, byte[] Content)> files, CancellationToken cancellationToken = default)
    {
        ApplyAuthorizationHeader();
        using var form = new MultipartFormDataContent();
        var authContext = ResolveAuthContext();

        foreach (var field in fields)
        {
            form.Add(new StringContent(field.Value), field.Key);
        }

        if (!fields.ContainsKey("companyId"))
            form.Add(new StringContent(authContext.CompanyId), "companyId");
        if (!fields.ContainsKey("userId"))
            form.Add(new StringContent(authContext.UserId), "userId");
        if (!fields.ContainsKey("ip"))
            form.Add(new StringContent(authContext.Ip), "ip");
        if (!fields.ContainsKey("device"))
            form.Add(new StringContent(authContext.Device), "device");

        foreach (var file in files)
        {
            var content = new ByteArrayContent(file.Content);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse(ResolveMimeType(file.FileName, file.Content));
            form.Add(content, "photos", file.FileName);
        }

        var response = await _httpClient.PostAsync(RegisterWithFilesUrl, form, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RegisterTransactionAsync(Dictionary<string, object> payload, CancellationToken cancellationToken = default)
    {
        ApplyAuthorizationHeader();
        var authContext = ResolveAuthContext();
        payload["companyId"] = authContext.CompanyId;
        payload["userId"] = authContext.UserId;
        payload["ip"] = authContext.Ip;
        payload["device"] = authContext.Device;

        var response = await _httpClient.PostAsJsonAsync(RegisterUrl, payload, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> SendAlertMailAsync(string xmlDetails, string containerId, string eventName, string stepLabel, CancellationToken cancellationToken = default)
    {
        ApplyAuthorizationHeader();
        var encodedXml = HtmlEncoder.Default.Encode(FormatXmlForHtml(xmlDetails));
        var payload = new Dictionary<string, object?>
        {
            ["to"] = AlertMailTo,
            ["subject"] = $"Alerta RFID {stepLabel} - {containerId} - {eventName}",
            ["body"] = $"<pre style=\"font-family: monospace; white-space: pre-wrap;\">{encodedXml}</pre>",
            ["format"] = "HTML",
            ["cc"] = AlertMailCc,
            ["bcc"] = string.Empty
        };

        try
        {
            var authContext = ResolveAuthContext();
            payload["companyId"] = authContext.CompanyId;
            payload["userId"] = authContext.UserId;
            payload["ip"] = authContext.Ip;
            payload["device"] = authContext.Device;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MAIL ALERT: contexto auth no disponible, se envía payload base. {ex.Message}");
        }

        foreach (var url in AlertMailUrls)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, payload, cancellationToken);
                if (response.IsSuccessStatusCode)
                    return true;

                if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    var body = await response.Content.ReadAsStringAsync(cancellationToken);
                    System.Diagnostics.Debug.WriteLine($"MAIL ALERT: {url} respondió {(int)response.StatusCode}. Body: {body}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MAIL ALERT: error llamando {url}. {ex.Message}");
            }
        }

        return false;
    }

    private async Task<CompanySettingsSnapshot> GetCompanySettingsSnapshotAsync(CancellationToken cancellationToken)
    {
        if (_settingsSnapshot is not null)
            return _settingsSnapshot;

        await _settingsLock.WaitAsync(cancellationToken);
        try
        {
            if (_settingsSnapshot is not null)
                return _settingsSnapshot;

            _settingsSnapshot = await FetchCompanySettingsAsync(cancellationToken);
            return _settingsSnapshot;
        }
        finally
        {
            _settingsLock.Release();
        }
    }

    private async Task<CompanySettingsSnapshot> FetchCompanySettingsAsync(CancellationToken cancellationToken)
    {
        try
        {
            ApplyAuthorizationHeader();
            var authContext = ResolveAuthContext();
            var response = await _httpClient.GetAsync($"{CompanySettingsUrl}/{authContext.CompanyId}", cancellationToken);
            if (!response.IsSuccessStatusCode)
                return FallbackSettings;

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(body))
                return FallbackSettings;

            using var doc = JsonDocument.Parse(body);
            if (!doc.RootElement.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Array)
                return FallbackSettings;

            string? roboflowRaw = null;
            string? validationLabelRaw = null;

            foreach (var item in data.EnumerateArray())
            {
                if (!item.TryGetProperty("id", out var idElement) || !item.TryGetProperty("value", out var valueElement))
                    continue;

                var id = idElement.GetInt32();
                var value = valueElement.GetString();
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                if (id == 3)
                    roboflowRaw = value;
                else if (id == 2)
                    validationLabelRaw = value;
            }

            var parsedRoboflow = ParseRoboflowConfig(roboflowRaw);
            var labelCount = ParseValidationLabelCount(validationLabelRaw);
            return new CompanySettingsSnapshot(
                parsedRoboflow.BaseUrl,
                parsedRoboflow.Workspace,
                parsedRoboflow.Workflow,
                parsedRoboflow.ApiKey,
                labelCount);
        }
        catch
        {
            return FallbackSettings;
        }
    }

    private static int ParseValidationLabelCount(string? value)
    {
        if (!int.TryParse(value, out var parsed))
            return FallbackSettings.ValidationLabelCount;

        return Math.Clamp(parsed, 0, 2);
    }

    private static (string BaseUrl, string Workspace, string Workflow, string ApiKey) ParseRoboflowConfig(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return (FallbackSettings.RoboflowBaseUrl, FallbackSettings.RoboflowWorkspace, FallbackSettings.RoboflowWorkflow, FallbackSettings.RoboflowApiKey);

        string Resolve(string key, string fallback)
        {
            var regex = new Regex($@"{key}\s*:\s*'([^']+)'", RegexOptions.IgnoreCase);
            var match = regex.Match(value);
            return match.Success ? match.Groups[1].Value : fallback;
        }

        return (
            Resolve("baseUrl", FallbackSettings.RoboflowBaseUrl),
            Resolve("workspace", FallbackSettings.RoboflowWorkspace),
            Resolve("workflow", FallbackSettings.RoboflowWorkflow),
            Resolve("apiKey", FallbackSettings.RoboflowApiKey)
        );
    }

    private static string ResolveMimeType(string fileName, byte[] content)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => GuessMimeTypeFromBytes(content)
        };
    }

    private static string GuessMimeTypeFromBytes(byte[] content)
    {
        if (content.Length >= 8 &&
            content[0] == 0x89 && content[1] == 0x50 && content[2] == 0x4E && content[3] == 0x47 &&
            content[4] == 0x0D && content[5] == 0x0A && content[6] == 0x1A && content[7] == 0x0A)
            return "image/png";

        if (content.Length >= 3 && content[0] == 0xFF && content[1] == 0xD8 && content[2] == 0xFF)
            return "image/jpeg";

        return "application/octet-stream";
    }

    private sealed record CompanySettingsSnapshot(
        string RoboflowBaseUrl,
        string RoboflowWorkspace,
        string RoboflowWorkflow,
        string RoboflowApiKey,
        int ValidationLabelCount);

    private void ApplyAuthorizationHeader()
    {
        if (string.IsNullOrWhiteSpace(_authState.Token))
            return;

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authState.Token);
    }

    private (string CompanyId, string UserId, string Ip, string Device) ResolveAuthContext()
    {
        if (string.IsNullOrWhiteSpace(_authState.Token))
            throw new InvalidOperationException("No hay token de sesión para resolver companyId/userId.");

        try
        {
            var tokenParts = _authState.Token.Split('.');
            if (tokenParts.Length < 2)
                throw new InvalidOperationException("Token JWT inválido: no contiene payload.");

            var payloadPart = tokenParts[1].Replace('-', '+').Replace('_', '/');
            payloadPart = payloadPart.PadRight(payloadPart.Length + ((4 - payloadPart.Length % 4) % 4), '=');
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(payloadPart));

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var companyId = TryGetClaim(root, "companyId", "companyID", "company");
            var userId = TryGetClaim(root, "userId", "userID", "sub");
            var ip = TryGetClaim(root, "ip") ?? _authState.Ip;
            var device = TryGetClaim(root, "device") ?? _authState.Device;

            if (string.IsNullOrWhiteSpace(companyId) || string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("El JWT no contiene claims obligatorios companyId y/o userId.");

            if (string.IsNullOrWhiteSpace(ip))
                throw new InvalidOperationException("No se pudo resolver la IP del dispositivo para el payload.");

            return (companyId, userId, ip, string.IsNullOrWhiteSpace(device) ? "android" : device);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("No se pudo resolver el contexto de autenticación desde el JWT.", ex);
        }
    }

    private static string? TryGetClaim(JsonElement root, params string[] claimNames)
    {
        foreach (var claimName in claimNames)
        {
            if (!root.TryGetProperty(claimName, out var property))
                continue;

            if (property.ValueKind == JsonValueKind.String)
            {
                var value = property.GetString();
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }
            else if (property.ValueKind == JsonValueKind.Number && property.TryGetInt64(out var number))
            {
                return number.ToString();
            }
        }

        return null;
    }

    private static string FormatXmlForHtml(string xml)
    {
        var compactXml = xml.Replace(">\n<", "><", StringComparison.Ordinal).Trim();
        compactXml = compactXml.Replace(">\r\n<", "><", StringComparison.Ordinal);
        var parts = compactXml.Replace("><", ">\n<", StringComparison.Ordinal).Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var depth = 0;
        var lines = new List<string>(parts.Length);

        foreach (var part in parts)
        {
            var token = part.Trim();
            if (token.StartsWith("</", StringComparison.Ordinal))
            {
                depth = Math.Max(depth - 1, 0);
            }

            lines.Add($"{new string('\t', depth)}{token}");

            if (token.StartsWith("<", StringComparison.Ordinal)
                && !token.StartsWith("</", StringComparison.Ordinal)
                && !token.EndsWith("/>", StringComparison.Ordinal)
                && !token.Contains("</", StringComparison.Ordinal))
            {
                depth += 1;
            }
        }

        return string.Join('\n', lines);
    }
}
