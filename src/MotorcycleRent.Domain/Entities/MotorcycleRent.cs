namespace MotorcycleRent.Domain.Entities;

public sealed record MotorcycleRent : BaseEntity
{
    public DeliveryPartner? DeliveryPartner { get; init; }
    public Motorcycle? Motorcycle { get; init; }
    public DateTimeRange RentPeriod { get; init; }
    public ERentPlan RentPlan { get; init; }
}