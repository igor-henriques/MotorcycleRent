namespace MotorcycleRent.Application.Models.Options;

public sealed record SeedOptions
{
    public bool IsSeedingActive { get; init; }
    public bool IsMassiveSeedingActive { get; init; }
    public AdministratorDto? AdministratorSeedUser { get; init; }
    public DeliveryPartnerDto? DeliveryPartnerSeedUser { get; init; }
}
