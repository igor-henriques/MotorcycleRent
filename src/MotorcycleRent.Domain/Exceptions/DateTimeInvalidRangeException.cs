namespace MotorcycleRent.Domain.Exceptions;

public sealed class DateTimeInvalidRangeException : Exception
{
    public DateTimeInvalidRangeException(string message) : base(message)
    {

    }
}
