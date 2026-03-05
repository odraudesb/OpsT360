namespace OpsT360.Services;

public interface IAuthState
{
    string? Token { get; }
    void SetToken(string token);
}
