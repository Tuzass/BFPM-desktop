namespace API.Requests;

public class RefreshRequest
{
    public string RefreshToken {get; set;} = null!;

    public RefreshRequest(string refreshToken)
    {
        RefreshToken = refreshToken;
    }

    private RefreshRequest()
    {
        
    }
}
