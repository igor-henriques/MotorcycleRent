namespace MotorcycleRent.Application.Models.Dtos;

public sealed record DeliveryPartnerDto : UserDto
{
    public string? FullName { get; init; }
    public string? NationalId { get; init; }
    public DateTime BirthDate { get; init; }
}
