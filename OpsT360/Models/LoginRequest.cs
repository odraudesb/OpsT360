using System.Text.Json.Serialization;

namespace OpsT360.Models;

public class LoginRequest
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [JsonPropertyName("ip")]
    public string Ip { get; set; } = "127.0.0.1";

    [JsonPropertyName("device")]
    public string Device { get; set; } = "android";
}
