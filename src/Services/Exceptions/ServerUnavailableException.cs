namespace Services.Exceptions;

public class ServerUnavailableException : Exception
{
    public ServerUnavailableException()
        : base("Server is currently unavailable, please try again later.")
    {
    }
}
