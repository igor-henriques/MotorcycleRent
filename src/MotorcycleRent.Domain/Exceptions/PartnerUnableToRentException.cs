namespace MotorcycleRent.Domain.Exceptions;

public sealed class PartnerUnableToRentException : Exception
{
    public PartnerUnableToRentException() : base("Partner not able to rent a motorcycle due to a problem with the driver license")
    {

    }
}
