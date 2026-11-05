namespace API.Auth;

public class AuthenticationTokens
{
    public string RefreshToken {get; set;} = null!;
    public string? AccessToken {get; set;}
    public DateTimeOffset? AccessTokenExpiresAt {get; set;}

    public AuthenticationTokens(string refreshToken, string? accessToken = null, DateTimeOffset? accessTokenExpiresAt = null)
    {
        RefreshToken = refreshToken;
        AccessToken = accessToken;
        AccessTokenExpiresAt = accessTokenExpiresAt;
    }

    private AuthenticationTokens()
    {
        
    }
}
