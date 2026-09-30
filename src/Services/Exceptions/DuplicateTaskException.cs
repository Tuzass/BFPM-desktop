namespace Services.Exceptions;

public class DuplicateTaskException : Exception
{
    public DuplicateTaskException()
        : base("Tasks in the same project must have unique names.")
    {
    }
}
