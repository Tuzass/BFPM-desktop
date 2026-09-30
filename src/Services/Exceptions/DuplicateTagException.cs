namespace Services.Exceptions;

public class DuplicateTagException : Exception
{
    public DuplicateTagException()
        : base("Tags in the same project must have unique names.")
    {
    }
}
