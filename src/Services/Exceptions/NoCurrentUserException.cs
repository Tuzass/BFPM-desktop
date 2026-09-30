namespace Services.Exceptions;

public class NoCurrentUserException : Exception
{
    public NoCurrentUserException()
        : base("No user is currently selected.")
    {
    }
}
