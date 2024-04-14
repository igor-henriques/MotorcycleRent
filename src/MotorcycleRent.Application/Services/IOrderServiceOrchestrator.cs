namespace MotorcycleRent.Application.Services;

/// <summary>
/// Orchestrates order creation, updating, and checking processes.
/// </summary>
/// <remarks>
/// This class provides high-level access to operations related to orders,
/// such as creating, checking availability, and updating order status,
/// while logging significant actions and publishing changes when necessary.
/// </remarks>
public interface IOrderServiceOrchestrator
{
    /// <summary>
    /// Creates a new order asynchronously based on the provided order data transfer object.
    /// </summary>
    /// <param name="orderDto">The order data transfer object with the details needed for order creation.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing the created order data transfer object.</returns>
    /// <exception cref="EntityCreationException">Thrown when the order cannot be created.</exception>
    Task<OrderDto> CreateOrderAsync(OrderDto orderDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks the availability of an order for delivery.
    /// </summary>
    /// <param name="publicOrderId">The public identifier for the order.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, containing the availability status as a boolean.</returns>
    /// <exception cref="OrderUnavailableException">Thrown when the order is not available for delivery.</exception>
    Task<bool> CheckOrderAvailabilityAsync(PublicOrderIdDto publicOrderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the status of an order asynchronously.
    /// </summary>
    /// <param name="updateOrderStatus">The data transfer object containing the new status and the order's public identifier.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <exception cref="InvalidOperationException">Thrown when the order does not exist or an error occurs during the update.</exception>
    Task UpdateOrderStatusAsync(UpdateOrderStatusDto updateOrderStatus, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all partners notified about an order asynchronously.
    /// </summary>
    /// <param name="publicOrderId">The public identifier for the order.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <exception cref="InvalidOperationException">Thrown when the order does not exist.</exception>
    Task<IEnumerable<DeliveryPartnerDto>> GetNotifiedPartnersAsync(PublicOrderIdDto publicOrderId, CancellationToken cancellationToken = default);
}