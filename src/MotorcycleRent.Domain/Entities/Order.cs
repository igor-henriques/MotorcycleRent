namespace MotorcycleRent.Domain.Entities;

public sealed record Order : BaseEntity
{
    public decimal DeliveryCost { get; init; }
    public DateTime CreationDate { get; init; }
    public DeliveryPartner? DeliveryPartner { get; init; }
}
