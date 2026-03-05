namespace OpsT360.Models;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Ip { get; set; } = "127.0.0.1";
    public string Device { get; set; } = "web";
}
