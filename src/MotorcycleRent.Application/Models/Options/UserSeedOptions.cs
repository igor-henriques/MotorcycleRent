namespace MotorcycleRent.Application.Models.Options;

public sealed record UserSeedOptions
{
    public bool IsSeedingActive { get; init; }
    public AdministratorDto? AdministratorSeedUser { get; init; }
    public DeliveryPartnerDto? DeliveryPartnerSeedUser { get; init; }
}
