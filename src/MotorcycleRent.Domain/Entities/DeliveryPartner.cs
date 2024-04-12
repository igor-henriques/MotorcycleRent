namespace MotorcycleRent.Domain.Entities;

public sealed record DeliveryPartner : User
{
    public string? FullName { get; init; }
    public string? NationalId { get; init; }
    public DateTime BirthDate { get; init; }
    public DriverLicense? DriverLicense { get; init; }
    public List<string> Notifications { get; init; } = [];

    /// <summary>
    /// Gets a value indicating whether the delivery partner is eligible to rent a vehicle.
    /// </summary>
    /// <value>
    /// <para>
    /// This property returns <c>true</c> if the delivery partner has a valid driver's license
    /// and the driver's license type is either A or AB, which are required for renting vehicles.
    /// </para>
    /// <para>
    /// It returns <c>false</c> if the delivery partner does not have a valid driver's license,
    /// or the driver's license type is not suitable for vehicle rental according to company policy.
    /// </para>
    /// </value>
    /// <remarks>
    /// The <see cref="DriverLicense"/> object should not be null, and it must have a <see cref="DriverLicense.DriverLicenseType"/>
    /// of either <see cref="EDriverLicenseType.A"/> or <see cref="EDriverLicenseType.AB"/> for this property to return <c>true</c>.
    /// This property is used to quickly determine rental eligibility without further checks.
    /// </remarks>
    public bool IsPartnerAbleToRent => DriverLicense != null && DriverLicense.DriverLicenseType is EDriverLicenseType.A or EDriverLicenseType.AB;
}