using System.Text.Json.Serialization;

namespace OpsT360.Models;

public sealed class LoginResponse
{
    [JsonPropertyName("data")]
    public LoginData? Data { get; set; }
}

public sealed class LoginData
{
    [JsonPropertyName("token")]
    public string? Token { get; set; }
}
