namespace Services.Exceptions;

public class UnexpectedTaskStatusException : Exception
{
    public UnexpectedTaskStatusException()
        : base("Unexpected task status received.")
    {
    }
}
