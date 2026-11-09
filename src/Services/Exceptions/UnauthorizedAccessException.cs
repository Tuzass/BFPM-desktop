namespace Services.Exceptions;

public class UnauthorizedAccessException : Exception
{
    public UnauthorizedAccessException()
        : base("User is not authorized to access this page.")
    {
    }
}
