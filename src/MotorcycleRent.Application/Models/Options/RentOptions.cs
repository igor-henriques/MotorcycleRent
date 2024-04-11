namespace MotorcycleRent.Application.Models.Options;

public sealed record RentOptions
{
    public decimal DailyExceedingFine { get; init; }
    public RentFine? RentFinePercentage { get; init; }

    public sealed record RentFine
    {
        public int Weekly { get; init; }
        public int Biweekly { get; init; }
        public int Monthly { get; init; }
    }
}
