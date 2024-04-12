namespace MotorcycleRent.Domain.Entities;

public sealed record OrderNotification : BaseEntity
{
    public Order? Order { get; init; }
    public List<DeliveryPartner> NotifiedPartners { get; init; } = [];
}
