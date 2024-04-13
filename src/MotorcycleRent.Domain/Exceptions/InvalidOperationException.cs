namespace MotorcycleRent.Domain.Exceptions;

public sealed class InvalidOperationException : DomainException
{
    public InvalidOperationException(string message) : base(message) { }
}
