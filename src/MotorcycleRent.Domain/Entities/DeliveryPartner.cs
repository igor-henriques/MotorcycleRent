namespace MotorcycleRent.Domain.Entities;

public sealed record DeliveryPartner : User
{
    public string? FullName { get; init; }
    public string? NationalId { get; init; }
    public DateTime BirthDate { get; init; }
    public DriverLicense? DriverLicense { get; init; }

    public bool IsPartnerAbleToRent => DriverLicense != null && DriverLicense.DriverLicenseType is EDriverLicenseType.A or EDriverLicenseType.AB;
}