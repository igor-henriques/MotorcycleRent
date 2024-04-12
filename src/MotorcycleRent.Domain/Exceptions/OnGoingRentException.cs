namespace MotorcycleRent.Domain.Exceptions;

public sealed class OnGoingRentException : Exception
{
    public OnGoingRentException(DateTimeRange rentPeriod) : base($"A rent was found for the current partner. Expected return date {rentPeriod.End:R}")
    {

    }
}
