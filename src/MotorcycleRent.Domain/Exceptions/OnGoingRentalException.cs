namespace MotorcycleRent.Domain.Exceptions;

public sealed class OnGoingRentalException : DomainException
{
    public OnGoingRentalException(DateTimeRange rentPeriod) : base($"A rental was found for the current partner. Expected return date {rentPeriod.End:R}") { }
    public OnGoingRentalException(DateTimeRange rentPeriod, string motorcyclePlate) : base($"A rental was found for the motorcycle plate {motorcyclePlate}. Expected return date {rentPeriod.End:R}") { }
}
