namespace OpsT360.Services;

public interface IAuthState
{
    string? Token { get; }
    string Ip { get; }
    string Device { get; }
    void SetToken(string token);
    void SetSession(string token, string ip, string device);
}
