namespace MotorcycleRent.Application.Models.Options;

public sealed record SeedOptions
{
    public bool IsSeedingActive { get; init; }
    public bool SeedMotorcycles { get; init; }
    public bool SeedOrders { get; init; }
    public bool SeedRents { get; init; }
    public AdministratorDto? AdministratorSeedUser { get; init; }
    public DeliveryPartnerDto? DeliveryPartnerSeedUser { get; init; }
}
