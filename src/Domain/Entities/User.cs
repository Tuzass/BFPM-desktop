namespace Domain.Entities;

public class User
{
    public int ID {get; private set;}
    public string Username {get; set;} = null!;
    public string Email {get; set;} = null!;

    private User() { }

    public User(int id, string username, string email)
    {
        ID = id;
        Username = username;
        Email = email;
    }
}
