namespace Services.Exceptions;

public class UnauthorizedAccessException : Exception
{
    public UnauthorizedAccessException()
        : base("Task and tag don't belong to the same project.")
    {
    }
}
