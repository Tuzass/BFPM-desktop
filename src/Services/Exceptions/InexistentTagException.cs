namespace Services.Exceptions;

public class InexistentTagException : Exception
{
    public InexistentTagException()
        : base("The requested tag doesn't exist.")
    {
    }
}
