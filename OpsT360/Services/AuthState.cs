namespace OpsT360.Services;

public sealed class AuthState : IAuthState
{
    public string? Token { get; private set; }

    public void SetToken(string token)
    {
        Token = token;
    }
}
