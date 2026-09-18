namespace Domain.Entities;

public class User
{
    public int? ID {get; private set;}
    public string Username {get; private set;} = null!;
    public string Email {get; private set;} = null!;
    public bool IsActive {get; set;}

    private User() { }

    public User(int? id, string username, string email)
    {
        ID = id;
        Username = username;
        Email = email;
        IsActive = true;
    }

    private static bool IsUsernameValid(string username)
    {
        if (username.Length >= 3 && username.Length <= 20)
        {
            return true;
        }

        return false;
    }

    public bool Rename(string newUsername)
    {
        if (!IsUsernameValid(newUsername))
        {
            return false;
        }

        Username = newUsername;
        return true;
    }
}
