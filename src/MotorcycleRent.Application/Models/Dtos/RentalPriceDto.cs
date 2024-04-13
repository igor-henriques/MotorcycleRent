namespace MotorcycleRent.Application.Models.Dtos;

public sealed record RentalPriceDto
{
    public decimal RentalBaseCost { get; init; }
    public decimal FeeCost { get; init; }
    public decimal ActualCost => Math.Round(RentalBaseCost + FeeCost, 2);
    public DateTimeRange RentPeriod { get; init; }
}
