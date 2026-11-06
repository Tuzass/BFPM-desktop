namespace Services.Exceptions;

public class DuplicateUserNameException : Exception
{
    public DuplicateUserNameException()
        : base("This username is already taken.")
    {
    }
}
