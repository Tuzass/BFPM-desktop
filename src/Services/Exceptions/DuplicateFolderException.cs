namespace Services.Exceptions;

public class DuplicateFolderException : Exception
{
    public DuplicateFolderException()
        : base("Folders owned by the same user must have unique names.")
    {
    }
}
