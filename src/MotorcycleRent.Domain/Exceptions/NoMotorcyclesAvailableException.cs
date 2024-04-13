namespace MotorcycleRent.Domain.Exceptions;

public sealed class NoMotorcyclesAvailableException : DomainException
{
    public NoMotorcyclesAvailableException() : base("No motorcycles available at the moment. Try again later.")
    {
    }
}
