namespace MotorcycleRent.Application.Models.Dtos;

public sealed record MotorcycleSearchCriteria
{
    public Guid? Id { get; init; }
    public int? Year { get; init; }
    public string? Model { get; init; }
    public string? Plate { get; init; }
    public EMotorcycleStatus? Status { get; init; }
}
