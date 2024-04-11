namespace MotorcycleRent.Application.Models.Dtos;

public sealed record MotorcycleDto : IDto
{
    public Guid? Id { get; init; }
    public int Year { get; init; }
    public string? Model { get; init; }
    public string? Plate { get; init; }
}
