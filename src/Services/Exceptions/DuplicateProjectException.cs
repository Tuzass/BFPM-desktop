namespace Services.Exceptions;

public class DuplicateProjectException : Exception
{
    public DuplicateProjectException()
        : base("Projects owned by the same user must have unique names.")
    {
    }
}
