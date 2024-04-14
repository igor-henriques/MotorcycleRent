namespace MotorcycleRent.NotificationConsumer.Models;

internal sealed record OrderNotification
{
    public string? PublicOrderId { get; init; }
    public decimal DeliveryCost { get; init; }
    public DateTime CreationDate { get; init; }

    public static OrderNotification BuildFromOrder(Order order)
    {
        return new OrderNotification()
        {
            CreationDate = order.CreationDate,
            DeliveryCost = order.DeliveryCost,
            PublicOrderId = order.PublicOrderId
        };
    }
}
