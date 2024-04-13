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
    /// True if the order status is 'Available' and there is no assigned delivery partner; otherwise, false.
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

        if (Status is EOrderStatus.Available && incomingStatus is EOrderStatus.Delivered)
        {
            return false;
        }       

        return true;
    }

    public bool CanBeAccepted(DeliveryPartner partner) 
    {
        return Status is EOrderStatus.Available && partner.CanOrderBeAccepted(this);
    }

    public bool IsOrderAvailableOnPartnerWithdrawal(EOrderStatus incomingStatus)
    {
        return Status is EOrderStatus.Accepted && incomingStatus is EOrderStatus.Available;
    }
}
