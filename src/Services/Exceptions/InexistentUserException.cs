namespace Services.Exceptions;

public class InexistentUserException : Exception
{
    public InexistentUserException()
        : base("The requested user doesn't exist.")
    {
    }
}
