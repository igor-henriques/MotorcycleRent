namespace MotorcycleRent.NotificationConsumer.Models;

public sealed record DriverLicense
{
    public string? DriverLicenseId { get; init; }
    public string? DriverLicenseImageUrl { get; init; }
    public EDriverLicenseType DriverLicenseType { get; init; }
}
