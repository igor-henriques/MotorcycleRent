namespace MotorcycleRent.Domain.Entities;

public sealed record DriverLicense
{
    public string? DriverLicenseId { get; init; }
    public string? DriverLicenseImageUrl { get; init; }
    public EDriverLicenseType DriverLicenseType { get; init; }
}