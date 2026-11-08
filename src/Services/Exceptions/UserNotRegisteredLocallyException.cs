namespace Services.Exceptions;

public class UserNotRegisteredLocallyException : Exception
{
    public UserNotRegisteredLocallyException()
        : base("Selected user is not registered locally.")
    {
    }
}
