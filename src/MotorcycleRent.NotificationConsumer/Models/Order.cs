namespace MotorcycleRent.NotificationConsumer.Models;

internal sealed record Order
{
    public decimal DeliveryCost { get; init; }
    public DateTime CreationDate { get; init; }
    public DeliveryPartner? DeliveryPartner { get; init; }
    public EOrderStatus Status { get; init; }
    public string? PublicOrderId { get; init; }
}