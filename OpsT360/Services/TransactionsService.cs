using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace OpsT360.Services;

public sealed class TransactionsService : ITransactionsService
{
    private readonly HttpClient _httpClient;
    private readonly RoboflowValidationService _roboflowValidationService;

    private const string RegisterWithFilesUrl = "http://38.242.225.119:3000/api/transactions/register-with-files";

    private const string ActiveApiEnv = "prod"; // prod | localhost
    private static readonly Dictionary<string, (string BaseUrl, string Workspace, string Workflow, string ApiKey)> RoboflowConfig = new()
    {
        ["prod"] = (
            "https://serverless.roboflow.com",
            "mi-workspace-sihjw",
            "detect-count-and-visualize-4",
            "8OQBCU7lFbC9ogYMmbB7"
        ),
        ["localhost"] = (
            "http://localhost:9001",
            "mi-workspace-sihjw",
            "detect-count-and-visualize-4",
            "8OQBCU7lFbC9ogYMmbB7"
        )
    };

    private static string RoboflowUrl
    {
        get
        {
            var cfg = RoboflowConfig[ActiveApiEnv];
            return $"{cfg.BaseUrl.TrimEnd('/')}/{cfg.Workspace}/workflows/{cfg.Workflow}";
        }
    }

    private static string RoboflowApiKey => RoboflowConfig[ActiveApiEnv].ApiKey;

    public TransactionsService(HttpClient httpClient, RoboflowValidationService roboflowValidationService)
    {
        _httpClient = httpClient;
        _roboflowValidationService = roboflowValidationService;
    }

    public async Task<RoboflowValidationResult> ValidatePhotoAsync(string imageBase64, string fileName, CancellationToken cancellationToken = default)
    {
        var request = new
        {
            api_key = RoboflowApiKey,
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

        var response = await _httpClient.PostAsJsonAsync(RoboflowUrl, request, cancellationToken);
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

    public async Task<bool> RegisterWithFilesAsync(Dictionary<string, string> fields, IEnumerable<(string FileName, byte[] Content)> files, CancellationToken cancellationToken = default)
    {
        using var form = new MultipartFormDataContent();

        foreach (var field in fields)
        {
            form.Add(new StringContent(field.Value), field.Key);
        }

        foreach (var file in files)
        {
            form.Add(new ByteArrayContent(file.Content), "photos", file.FileName);
        }

        var response = await _httpClient.PostAsync(RegisterWithFilesUrl, form, cancellationToken);
        return response.IsSuccessStatusCode;
    }
}
