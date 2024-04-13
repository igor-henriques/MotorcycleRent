namespace MotorcycleRent.NotificationConsumer.Models;

internal sealed record Order : BaseEntity
{
    public decimal DeliveryCost { get; init; }
    public DateTime CreationDate { get; init; }
    public DeliveryPartner? DeliveryPartner { get; init; }
    public EOrderStatus Status { get; init; }
    public string? PublicOrderId { get; init; }
    public List<string> NotifiedPartnersEmails { get; init; } = [];
    public bool CanPartnersBeNotified => DeliveryPartner is null && NotifiedPartnersEmails.Count is 0 && Status is EOrderStatus.Available;
}