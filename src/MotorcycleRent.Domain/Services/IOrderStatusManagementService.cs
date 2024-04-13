namespace MotorcycleRent.Domain.Services;

/// <summary>
/// Manages updates to order statuses in coordination with delivery partners.
/// </summary>
/// <remarks>
/// This service handles the complex logic required to transition orders between different statuses
/// while ensuring that all business rules and validations are adhered to. It also logs significant events
/// and errors to facilitate debugging and tracking of order status changes.
/// </remarks>
public interface IOrderStatusManagementService
{
    /// <summary>
    /// Handles the process of updating the status of an order and coordinating with the delivery partner.
    /// </summary>
    /// <param name="orderBeingUpdated">The order whose status is to be updated.</param>
    /// <param name="deliveryPartner">The delivery partner associated with the order.</param>
    /// <param name="incomingStatus">The new status to be applied to the order.</param>
    /// <returns>A tuple containing the updated order and delivery partner.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the update is not allowed as per business rules.</exception>
    (Order, DeliveryPartner) HandleOrderStatusUpdate(
        Order orderBeingUpdated,
        DeliveryPartner deliveryPartner,
        EOrderStatus incomingStatus);
}