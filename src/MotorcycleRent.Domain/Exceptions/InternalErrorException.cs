namespace MotorcycleRent.Domain.Exceptions;

public sealed class InternalErrorException : Exception
{
    public InternalErrorException(string message) : base(message)
    {
    }
}
