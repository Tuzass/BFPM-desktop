namespace Services.Exceptions;

public class InexistentFolderException : Exception
{
    public InexistentFolderException()
        : base("The requested folder doesn't exist.")
    {
    }
}
