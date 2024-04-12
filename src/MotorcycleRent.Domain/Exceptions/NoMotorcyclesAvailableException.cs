namespace MotorcycleRent.Domain.Exceptions;

public sealed class NoMotorcyclesAvailableException : Exception
{
    public NoMotorcyclesAvailableException() : base("No motorcycles available at the moment. Try again later.")
    {
    }
}
