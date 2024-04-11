namespace MotorcycleRent.Application.Models.Dtos;

public sealed record UpdateMotorcyclePlateDto
{
    public string? OldPlate { get; init; }
    public string? NewPlate { get; init; }
}
