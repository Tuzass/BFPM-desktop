namespace API.Auth;

public class TokenStore
{
    private AuthenticationTokens? tokens;

    public async Task<AuthenticationTokens?> GetTokens()
    {
        return tokens;
    }

    public async Task SaveTokens(AuthenticationTokens newTokens)
    {
        tokens = newTokens;
    }

    public async Task ClearTokens()
    {
        tokens = null;
    }
}
