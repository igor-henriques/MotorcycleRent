namespace MotorcycleRent.Application.Models.Dtos;

public sealed record RentPriceDto
{
    public decimal RentCost { get; init; }
    public decimal FeeCost { get; init; }
    public decimal ActualCost => Math.Round(RentCost + FeeCost);
    public DateTimeRange RentPeriod { get; init; }
}
