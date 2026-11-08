namespace Services.Exceptions;

public class UnexpectedProjectStatusException : Exception
{
    public UnexpectedProjectStatusException()
        : base("Unexpected project status received.")
    {
    }
}
