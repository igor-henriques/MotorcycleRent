namespace MotorcycleRent.Application.Models.Dtos;

public record UserDto : IDto
{
    public string? Email { get; init; }
    public string? Password { get; init; }
}
