namespace API.Responses;

public class TokenResponse
{
    public string AccessToken {get; set;} = null!;
    public string TokenType {get; set;} = null!;
    public int ExpiresIn {get; set;}
    public string RefreshToken { get; set;} = null!;
}
