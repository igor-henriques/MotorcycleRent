namespace MotorcycleRent.NotificationConsumer.Models;

internal sealed record DeliveryPartner
{
    public string? Email { get; init; }
    public string? FullName { get; init; }
    public string? NationalId { get; init; }
    public DateTime BirthDate { get; init; }
    public DriverLicense? DriverLicense { get; init; }
    public List<string> Notifications { get; init; } = [];
    public bool IsPartnerAbleToRent => DriverLicense != null && DriverLicense.DriverLicenseType is EDriverLicenseType.A or EDriverLicenseType.AB;
}