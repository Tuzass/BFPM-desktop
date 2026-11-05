namespace API.Requests;

public class LoginRequest
{
    public string Email {get; set;} = null!;
    public string Password {get; set;} = null!;

    public LoginRequest(string email, string password)
    {
        Email = email;
        Password = password;
    }

    private LoginRequest()
    {
        
    }
}
