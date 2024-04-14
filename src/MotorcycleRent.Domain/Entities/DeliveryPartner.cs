namespace MotorcycleRent.Domain.Entities;

public sealed record DeliveryPartner : User
{
    public string? FullName { get; init; }
    public string? NationalId { get; init; }
    public DateTime BirthDate { get; init; }
    public DriverLicense? DriverLicense { get; init; }
    public List<OrderNotification> Notifications { get; init; } = [];
    public bool HasActiveRental { get; init; }
    public bool IsAvailable { get; init; }

    /// <summary>
    /// Gets a value indicating whether the delivery partner is eligible to rent a vehicle.
    /// </summary>
    /// <value>
    /// <para>
    /// This property returns <c>true</c> if the delivery partner has a valid driver's license
    /// and the driver's license type is either A or AB, which are required for renting vehicles,
    /// and if the partner does not have any active rental.
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
    public bool IsPartnerAbleToRent => !HasActiveRental && 
        DriverLicense != null && 
        DriverLicense.DriverLicenseType is EDriverLicenseType.A or EDriverLicenseType.AB;

    /// <summary>
    /// Determines whether the delivery partner can accept a specific order.
    /// </summary>
    /// <param name="order">The order to be potentially accepted by the delivery partner.</param>
    /// <returns>
    /// <c>true</c> if the delivery partner has an active rental, the order is already notified to the partner, and the partner is available;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method checks three conditions for an order to be accepted by the delivery partner:
    /// 1. The partner must have an active rental. This implies that the partner is engaged in active delivery tasks.
    /// 2. The order must already be in the partner's notification list. This ensures that the partner is aware of the order and has previously been considered for it.
    /// 3. The partner must be currently available to take new orders. Availability ensures that the partner can manage additional tasks.
    /// If any of these conditions are not met, the order cannot be accepted by the partner.
    /// </remarks>
    public bool CanOrderBeAccepted(Order order) => HasActiveRental && Notifications.Any(o => o.PublicOrderId == order.PublicOrderId) && IsAvailable;
}