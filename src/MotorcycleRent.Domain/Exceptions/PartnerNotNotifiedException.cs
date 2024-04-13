namespace MotorcycleRent.Domain.Exceptions;

public sealed class PartnerNotNotifiedException : DomainException
{
    public PartnerNotNotifiedException() : base("Order cannot be accepted by a partner that was not previously notified about it.")
    {

    }
}
