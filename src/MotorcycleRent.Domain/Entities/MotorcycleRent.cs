namespace MotorcycleRent.Domain.Entities;

public sealed record MotorcycleRent : BaseEntity
{
    public DeliveryPartner? DeliveryPartner { get; init; }
    public Motorcycle? Motorcycle { get; init; }
    public DateTimeRange RentPeriod { get; init; }
    public ERentPlan RentPlan { get; init; }
    public decimal RentCost { get; init; }
    public decimal FeeCost { get; init; }
    public decimal ActualCost => Math.Round(RentCost + FeeCost);
    public ERentStatus RentStatus { get; init; }
}