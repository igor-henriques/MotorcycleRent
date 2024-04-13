namespace MotorcycleRent.Domain.Exceptions;

public sealed class OrderUnavailableException  : DomainException
{
    public OrderUnavailableException(string publicOrderId) : base($"Order with public order id '{publicOrderId}' is unavailable for delivery")
    {
        
    }
}