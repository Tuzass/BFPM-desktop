namespace Services.Exceptions;

public class DuplicateEmailException : Exception
{
    public DuplicateEmailException()
        : base("This email is already taken.")
    {
    }
}
