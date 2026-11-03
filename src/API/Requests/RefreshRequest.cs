namespace API.Requests;

public class RefreshRequest
{
    public string RefreshToken {get; set;}

    public RefreshRequest(string refreshToken)
    {
        RefreshRequest = refreshToken;
    }

    private RefreshRequest()
    {
        
    }
}
