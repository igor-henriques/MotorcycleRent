namespace MotorcycleRent.Application.Services;

/// <summary>
/// Manages updates to order statuses in coordination with delivery partners.
/// </summary>
/// <remarks>
/// This service handles the complex logic required to transition orders between different statuses
/// while ensuring that all business rules and validations are adhered to. It also logs significant events
/// and errors to facilitate debugging and tracking of order status changes.
/// </remarks>
public sealed class OrderStatusManagementService : IOrderStatusManagementService
{
    private readonly ILogger<OrderStatusManagementService> _logger;

    public OrderStatusManagementService(ILogger<OrderStatusManagementService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the process of updating the status of an order and coordinating with the delivery partner.
    /// </summary>
    /// <param name="orderBeingUpdated">The order whose status is to be updated.</param>
    /// <param name="deliveryPartner">The delivery partner associated with the order.</param>
    /// <param name="incomingStatus">The new status to be applied to the order.</param>
    /// <returns>A tuple containing the updated order and delivery partner.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the update is not allowed as per business rules.</exception>
    public (Order, DeliveryPartner) HandleOrderStatusUpdate(
        Order orderBeingUpdated,
        DeliveryPartner deliveryPartner,
        EOrderStatus incomingStatus)
    {
        if (!orderBeingUpdated.CanUpdateStatus(incomingStatus))
        {
            _logger.LogWarning("Order {OrderPublicId} cannot perform update from status {CurrentOrderStatus} to {NewOrderStatus}",
                orderBeingUpdated.PublicOrderId,
                orderBeingUpdated.Status,
                incomingStatus);

            throw new InvalidOperationException(Constants.Messages.ForbiddenOrderUpdate);
        }

        (var updatedOrder, var updatedPartner) = HandleUpdateFromAvailableToAccepted(orderBeingUpdated, deliveryPartner, incomingStatus);
        (updatedOrder, updatedPartner) = HandleUpdateFromAcceptedToDelivered(updatedOrder, updatedPartner, incomingStatus);
        (updatedOrder, updatedPartner) = HandlePartnerWithdrawal(updatedOrder, updatedPartner, incomingStatus);

        return (updatedOrder, updatedPartner);
    }

    /// <summary>
    /// Private method to handle the status transition from 'Accepted' to 'Delivered'.
    /// </summary>
    /// <param name="orderBeingUpdated">The order being updated.</param>
    /// <param name="deliveryPartner">The associated delivery partner.</param>
    /// <returns>A tuple containing the updated order and delivery partner.</returns>
    private (Order, DeliveryPartner) HandleUpdateFromAcceptedToDelivered(Order orderBeingUpdated, DeliveryPartner deliveryPartner, EOrderStatus incomingStatus)
    {
        if (incomingStatus is not EOrderStatus.Delivered)
        {
            return (orderBeingUpdated, deliveryPartner);
        }

        if (!orderBeingUpdated.CanBeDelivered(deliveryPartner))
        {
            _logger.LogInformation("Mismatch between order and delivery partner state machine");
            throw new InvalidOperationException(Constants.Messages.ForbiddenOrderUpdate);
        }

        var updatedPartner = deliveryPartner with { IsAvailable = true };
        var updatedOrder = orderBeingUpdated with { Status = EOrderStatus.Delivered };

        updatedPartner.Notifications.RemoveAll(x => x.PublicOrderId == orderBeingUpdated.PublicOrderId);

        return (updatedOrder, updatedPartner);
    }

    /// <summary>
    /// Private static method to handle the status transition from 'Available' to 'Accepted'.
    /// </summary>
    /// <param name="orderBeingUpdated">The order being updated.</param>
    /// <param name="deliveryPartner">The associated delivery partner.</param>
    /// <returns>A tuple containing the updated order and delivery partner.</returns>
    private static (Order, DeliveryPartner) HandleUpdateFromAvailableToAccepted(
        Order orderBeingUpdated,
        DeliveryPartner deliveryPartner,
        EOrderStatus incomingStatus)
    {
        if (incomingStatus is not EOrderStatus.Accepted)
        {
            return (orderBeingUpdated, deliveryPartner);
        }

        if (!orderBeingUpdated.CanBeAccepted(deliveryPartner))
        {
            throw new PartnerUnableToAcceptOrderException();
        }

        var updatedPartner = deliveryPartner with { IsAvailable = false };
        var updatedOrder = orderBeingUpdated with
        {
            Status = EOrderStatus.Accepted,
            DeliveryPartner = deliveryPartner
        };

        return (updatedOrder, updatedPartner);
    }

    /// <summary>
    /// Handles the withdrawal of a delivery partner, potentially returning the order to an 'Available' status.
    /// </summary>
    /// <param name="orderBeingUpdated">The order being affected by the withdrawal.</param>
    /// <param name="deliveryPartner">The delivery partner withdrawing from the order.</param>
    /// <param name="incomingStatus">The resulting status of the order post-withdrawal.</param>
    /// <returns>A tuple containing the updated order and delivery partner.</returns>
    private (Order, DeliveryPartner) HandlePartnerWithdrawal(
        Order orderBeingUpdated,
        DeliveryPartner deliveryPartner,
        EOrderStatus incomingStatus)
    {
        bool isDeliveryPartnerWithdrawn = orderBeingUpdated.IsOrderAvailableOnPartnerWithdrawal(incomingStatus);

        if (isDeliveryPartnerWithdrawn)
        {
            deliveryPartner = deliveryPartner with { IsAvailable = true };
            deliveryPartner.Notifications.RemoveAll(x => x.PublicOrderId == orderBeingUpdated.PublicOrderId);

            _logger.LogInformation("Order returning to Available status due to partner withdrawal");
        }

        var updatedOrder = orderBeingUpdated with
        {
            Status = incomingStatus,
            DeliveryPartner = isDeliveryPartnerWithdrawn ? null : deliveryPartner // if an order is being withdrawn, the partner should be removed from the order delivery partner property
        };

        return (updatedOrder, deliveryPartner);
    }
}
