namespace OpsT360.Services;

public sealed class AuthState : IAuthState
{
    public string? Token { get; private set; }
    public string Ip { get; private set; } = "127.0.0.1";
    public string Device { get; private set; } = "android";

    public void SetToken(string token)
    {
        Token = token;
        if (string.IsNullOrWhiteSpace(token))
        {
            Ip = "127.0.0.1";
            Device = "android";
        }
    }

    public void SetSession(string token, string ip, string device)
    {
        Token = token;
        Ip = string.IsNullOrWhiteSpace(ip) ? "127.0.0.1" : ip.Trim();
        Device = string.IsNullOrWhiteSpace(device) ? "android" : device.Trim();
    }
}
