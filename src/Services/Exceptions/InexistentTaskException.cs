namespace Services.Exceptions;

public class InexistentTaskException : Exception
{
    public InexistentTaskException()
        : base("The requested task doesn't exist.")
    {
    }
}
