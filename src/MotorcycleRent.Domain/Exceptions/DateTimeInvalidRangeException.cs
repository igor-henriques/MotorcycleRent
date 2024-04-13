namespace MotorcycleRent.Domain.Exceptions;

public sealed class DateTimeInvalidRangeException : DomainException
{
    public DateTimeInvalidRangeException(string message) : base(message)
    {

    }
}
