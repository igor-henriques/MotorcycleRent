namespace MotorcycleRent.Domain.Entities;

public sealed record Order : BaseEntity
{
    public decimal DeliveryCost { get; init; }
    public DateTime CreationDate { get; init; }
    public DeliveryPartner? DeliveryPartner { get; init; }
    public EOrderStatus Status { get; init; }
    public List<string> NotifiedPartnersEmails { get; init; } = [];

    /// <summary>
    /// Gets a value indicating whether the order is available for delivery by any of the partners.
    /// </summary>
    /// <value>
    /// True if the order status is <see cref="EOrderStatus.Available"/> and there is no assigned delivery partner; otherwise, false.
    /// </value>
    /// <remarks>
    /// This property is useful for quickly determining the delivery availability of an order based on its status and the presence of a delivery partner.
    /// It should be checked whenever there is a need to verify if an order can be picked up by the delivery partner.
    /// </remarks>
    public bool IsOrderAvailableToDelivery => Status is EOrderStatus.Available && DeliveryPartner is null;

    /// <summary>
    /// Represents a public-friendly, unique identifier for the order based on its GUID.
    /// </summary>
    /// <remarks>
    /// This property utilizes the <see cref="FriendlyIdGenerator.CreateFriendlyId(Guid)"/> method to convert the order's GUID
    /// into a more human-readable and shorter string. This ID is intended for use in user interfaces and external communications
    /// where a more compact and less complex identifier is preferable to a standard GUID.
    /// </remarks>
    /// <returns>A string representing the unique, public-friendly ID of the order.</returns>
    public string? PublicOrderId { get; init; }    

    private Order() { }

    /// <summary>
    /// Creates a new order with the specified delivery cost and order status.
    /// </summary>
    /// <param name="deliveryCost">The cost associated with the delivery of the order.</param>
    /// <param name="status">The status of the order, indicating its current processing status.</param>
    /// <returns>A new instance of <see cref="Order"/> initialized with the provided values and a unique identifier.</returns>
    /// <remarks>
    /// This method initializes the order's unique identifier and sets the creation date to the current UTC time. 
    /// It is intended for creating new order records with fresh identifiers and should be used whenever a new order is created.
    /// The reason why <see cref="Id"/> is created by the code in this case, despite all others, is because we need to compute 
    /// <see cref="PublicOrderId"/> in runtime, before saving the entire object into the database.
    /// </remarks>
    public static Order CreateNewOrder(decimal deliveryCost, EOrderStatus status)
    {
        Guid orderId = Guid.NewGuid();

        return new Order
        {
            Id = Guid.NewGuid(),
            CreationDate = DateTime.UtcNow,
            DeliveryCost = deliveryCost,
            Status = status,
            PublicOrderId = FriendlyIdGenerator.CreateFriendlyId(orderId)
        };
    }

    /// <summary>
    /// Determines if the order status can be updated to a specified new status.
    /// </summary>
    /// <param name="incomingStatus">The proposed new status for the order.</param>
    /// <returns>True if the status can be updated; otherwise, false.</returns>
    /// <remarks>
    /// The method checks if the proposed status is different from the current status and ensures that an order
    /// cannot be updated if it is already in the <see cref="EOrderStatus.Delivered"/> status.
    /// </remarks>
    public bool CanUpdateStatus(EOrderStatus incomingStatus)
    {
        if (Status == incomingStatus)
        {
            return false;
        }

        if (Status is EOrderStatus.Delivered)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Determines whether an order can be accepted by a specified delivery partner.
    /// </summary>
    /// <param name="partner">The delivery partner who may accept the order.</param>
    /// <returns>True if the order is in <see cref="EOrderStatus.Available"/> status and the partner is eligible to accept it; otherwise, false.</returns>
    /// <remarks>
    /// This method checks if the order status is <see cref="EOrderStatus.Available"/> and defers to the delivery partner's logic
    /// to determine if they can accept the order.
    /// </remarks>
    public bool CanBeAccepted(DeliveryPartner partner) 
    {
        return Status is EOrderStatus.Available && partner.CanOrderBeAccepted(this);
    }

    /// <summary>
    /// Determines whether the order can be marked as delivered by a specified delivery partner.
    /// </summary>
    /// <param name="partner">The delivery partner handling the order.</param>
    /// <returns>True if the order is in <see cref="EOrderStatus.Accepted"/> status and the partner is not currently available; otherwise, false.</returns>
    /// <remarks>
    /// Checks that the order is <see cref="EOrderStatus.Accepted"/> and that the partner handling the order is not marked as available,
    /// implying that they are actively engaged in a delivery.
    /// </remarks>
    public bool CanBeDelivered(DeliveryPartner partner)
    {
        return Status is EOrderStatus.Accepted && !partner.IsAvailable;
    }

    /// <summary>
    /// Determines if the order can return to <see cref="EOrderStatus.Available"/> status upon withdrawal of a delivery partner.
    /// </summary>
    /// <param name="incomingStatus">The new status to be applied if the delivery partner withdraws.</param>
    /// <returns>True if the order was <see cref="EOrderStatus.Accepted"/> and the new status is <see cref="EOrderStatus.Available"/>; otherwise, false.</returns>
    /// <remarks>
    /// This method checks if the order is transitioning from <see cref="EOrderStatus.Accepted"/> to <see cref="EOrderStatus.Available"/> due to a partner withdrawal,
    /// indicating that no delivery partner is currently assigned to complete the delivery.
    /// </remarks>
    public bool IsOrderAvailableOnPartnerWithdrawal(EOrderStatus incomingStatus)
    {
        return Status is EOrderStatus.Accepted && incomingStatus is EOrderStatus.Available;
    }
}