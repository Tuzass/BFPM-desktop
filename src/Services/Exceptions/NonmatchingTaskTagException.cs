namespace Services.Exceptions;

public class NonmatchingTaskTagException : Exception
{
    public NonmatchingTaskTagException()
        : base("Task and tag don't belong to the same project.")
    {
    }
}
