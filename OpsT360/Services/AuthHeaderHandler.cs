using System.Net.Http.Headers;

namespace OpsT360.Services;

public sealed class AuthHeaderHandler : DelegatingHandler
{
    private readonly IAuthState _authState;

    public AuthHeaderHandler(IAuthState authState)
    {
        _authState = authState;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_authState.Token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authState.Token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
