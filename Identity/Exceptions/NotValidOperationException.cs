namespace AppIdentity.Exceptions;

public class NotValidOperationException : Exception
{
    public NotValidOperationException(string message) : base(message) { }
}