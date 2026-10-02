namespace Services.Exceptions;

public class UserNotRegisteredLocally : Exception
{
    public UserNotRegisteredLocally()
        : base("Selected user is not registered locally.")
    {
    }
}
