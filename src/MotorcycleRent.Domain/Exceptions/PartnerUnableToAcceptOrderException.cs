namespace MotorcycleRent.Domain.Exceptions;

public sealed class PartnerUnableToAcceptOrderException : DomainException
{
    public PartnerUnableToAcceptOrderException() : base("Order cannot be accepted by this partner.")
    {

    }
}
