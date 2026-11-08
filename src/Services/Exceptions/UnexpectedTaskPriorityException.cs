namespace Services.Exceptions;

public class UnexpectedTaskPriorityException : Exception
{
    public UnexpectedTaskPriorityException()
        : base("Unexpected task priority received.")
    {
    }
}
