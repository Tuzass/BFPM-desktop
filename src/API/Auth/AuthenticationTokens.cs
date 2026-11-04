namespace API.Auth;

public class AuthenticationTokens
{
    public string AccessToken {get; set;} = null!;
    public string RefreshToken {get; set;} = null!;
    public DateTimeOffset AccessTokenExpiresAt {get; set;}

    public AuthenticationTokens(string accessToken, string refreshToken, DateTimeOffset accessTokenExpiresAt)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        AccessTokenExpiresAt = accessTokenExpiresAt;
    }

    private AuthenticationTokens()
    {
        
    }
}
