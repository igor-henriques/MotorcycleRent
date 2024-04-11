namespace MotorcycleRent.Domain.Entities;

public sealed record Motorcycle : BaseEntity
{
    public int Year { get; init; }
    public string? Model { get; init; }
    public string? Plate { get; init; }
}
