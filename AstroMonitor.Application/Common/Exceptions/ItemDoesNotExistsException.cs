namespace AstroMonitor.Application.Common.Exceptions;

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message = "Item does not exists.") : base(message)
    {
        
    }
}