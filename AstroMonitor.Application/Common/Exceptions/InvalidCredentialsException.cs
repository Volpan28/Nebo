namespace AstroMonitor.Application.Common.Exceptions;

public class InvalidCredentialsExceptions : Exception
{
    public InvalidCredentialsExceptions(string message = "Invalid email or password.") : base(message)
    {
        
    }
}