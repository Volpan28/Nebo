namespace AstroMonitor.Application.Common.Exceptions;

public class ItemAlreadyExistsException : Exception
{
    public ItemAlreadyExistsException(string message = "Item is already exists.") : base(message)
    {
        
    }
}   