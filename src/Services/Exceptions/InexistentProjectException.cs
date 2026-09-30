namespace Services.Exceptions;

public class InexistentProjectException : Exception
{
    public InexistentProjectException()
        : base("The requested project doesn't exist.")
    {
    }
}
