namespace MotorcycleRent.Domain.Models;

public sealed record RentCost
{
    public decimal FullCost { get; init; }
    public decimal DiscountPercentage { get; init; }
    public decimal ExtraCosts { get; init; }
    public decimal ActualCost => FullCost - FullCost * DiscountPercentage / 100 + ExtraCosts;
}
