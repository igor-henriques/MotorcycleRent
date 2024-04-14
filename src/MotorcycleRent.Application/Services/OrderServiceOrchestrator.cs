namespace MotorcycleRent.Application.Services;

/// <summary>
/// Orchestrates order creation, updating, and checking processes.
/// </summary>
/// <remarks>
/// This class provides high-level access to operations related to orders,
/// such as creating, checking availability, and updating order status,
/// while logging significant actions and publishing changes when necessary.
/// </remarks>
public sealed class OrderServiceOrchestrator : IOrderServiceOrchestrator
{
    private readonly IBaseRepository<Order> _orderRepository;
    private readonly IBaseRepository<DeliveryPartner> _deliveryPartnerRepository;
    private readonly ILogger<OrderServiceOrchestrator> _logger;
    private readonly IMapper _mapper;
    private readonly IPublisher<Order> _orderPublisher;
    private readonly IEmailClaimProvider _emailClaimProvider;
    private readonly IOrderStatusManagementService _orderStatusManagementService;

    public OrderServiceOrchestrator(IBaseRepository<Order> repository,
                                    ILogger<OrderServiceOrchestrator> logger,
                                    IMapper mapper,
                                    IPublisher<Order> orderPublisher,
                                    IEmailClaimProvider emailClaimProvider,
                                    IBaseRepository<DeliveryPartner> userRepository,
                                    IOrderStatusManagementService orderStatusManagementService)
    {
        _orderRepository = repository;
        _logger = logger;
        _mapper = mapper;
        _orderPublisher = orderPublisher;
        _emailClaimProvider = emailClaimProvider;
        _deliveryPartnerRepository = userRepository;
        _orderStatusManagementService = orderStatusManagementService;
    }

    /// <summary>
    /// Creates a new order asynchronously based on the provided order data transfer object.
    /// </summary>
    /// <param name="orderDto">The order data transfer object with the details needed for order creation.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing the created order data transfer object.</returns>
    /// <exception cref="EntityCreationException">Thrown when the order cannot be created.</exception>
    public async Task<OrderDto> CreateOrderAsync(OrderDto orderDto, CancellationToken cancellationToken = default)
    {
        var incomingOrder = Order.CreateNewOrder(orderDto.DeliveryCost, orderDto.Status);

        // Create an index with the public order id to ensure uniqueness.
        // Only creates the index case it don't exist.                                                                   
        await _orderRepository.CreateIndexAsync(
            new CreateIndexModel<Order>(
                Builders<Order>.IndexKeys.Descending(d => d.PublicOrderId),
                new CreateIndexOptions { Unique = true, Name = "UniquePublicOrderId" }),
            cancellationToken);

        var createdOrder = await _orderRepository.CreateAsync(incomingOrder, cancellationToken)
            ?? throw new EntityCreationException(typeof(Order));        

        if (createdOrder.Status is EOrderStatus.Available)
        {
            await _orderPublisher.PublishMessageAsync(createdOrder, cancellationToken);
        }

        _logger.LogInformation("New order id '{OrderId}' successfully created and distributed", createdOrder.Id);
        var response = _mapper.Map<OrderDto>(createdOrder);
        return response;
    }

    /// <summary>
    /// Checks the availability of an order for delivery.
    /// </summary>
    /// <param name="publicOrderId">The public identifier for the order.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, containing the availability status as a boolean.</returns>
    /// <exception cref="OrderUnavailableException">Thrown when the order is not available for delivery.</exception>
    public async Task<bool> CheckOrderAvailabilityAsync(PublicOrderIdDto publicOrderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByAsync(o => o.PublicOrderId! == publicOrderId!.PublicOrderId, cancellationToken);

        if (order is null || !order.IsOrderAvailableToDelivery)
        {
            throw new OrderUnavailableException(publicOrderId!);
        }

        _logger.LogInformation("Order {PublicOrderId} availability was checked and it returned {OrderAvailability}",
            publicOrderId,
            order);

        return true;
    }

    /// <summary>
    /// Updates the status of an order asynchronously.
    /// </summary>
    /// <param name="updateOrderStatus">The data transfer object containing the new status and the order's public identifier.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <exception cref="InvalidOperationException">Thrown when the order does not exist or an error occurs during the update.</exception>
    public async Task UpdateOrderStatusAsync(UpdateOrderStatusDto updateOrderStatus, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByAsync(o => o.PublicOrderId! == updateOrderStatus.PublicOrderId!, cancellationToken)
            ?? throw new InvalidOperationException(Constants.Messages.InvalidOrder);

        var deliveryPartnerEmail = _emailClaimProvider.GetUserEmail() ?? string.Empty;

        var deliveryPartner = await _deliveryPartnerRepository.GetByAsync(u => u.Email == deliveryPartnerEmail, cancellationToken)
            ?? throw new InvalidOperationException(Constants.Messages.InvalidOrderUpdate);

        (var incomingUpdatedOrder, var incomingUpdatedDeliveryPartner) = _orderStatusManagementService.HandleOrderStatusUpdate(order, deliveryPartner, updateOrderStatus.Status);

        var partnerUpdateTask = _deliveryPartnerRepository.UpdateAsync(incomingUpdatedDeliveryPartner, cancellationToken)
            ?? throw new InvalidOperationException(Constants.Messages.InvalidOrderUpdate);

        var orderUpdateTask = _orderRepository.UpdateAsync(incomingUpdatedOrder, cancellationToken)
            ?? throw new InvalidOperationException(Constants.Messages.InvalidOrderUpdate);

        await Task.WhenAll(partnerUpdateTask, orderUpdateTask);

        _logger.LogInformation("Order {OrderPublicId} successfully updated from {OldOrderStatus} to {NewOrderStatus}",
            order.PublicOrderId,
            order.Status,
            updateOrderStatus.Status);
    }

    /// <summary>
    /// Gets all partners notified about an order asynchronously.
    /// </summary>
    /// <param name="publicOrderId">The public identifier for the order.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <exception cref="InvalidOperationException">Thrown when the order does not exist.</exception>
    public async Task<IEnumerable<DeliveryPartnerDto>> GetNotifiedPartnersAsync(PublicOrderIdDto publicOrderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByAsync(o => o.PublicOrderId! == publicOrderId.PublicOrderId, cancellationToken)
            ?? throw new InvalidOperationException(Constants.Messages.InvalidOrder);

        var partners = await _deliveryPartnerRepository.GetAllByAsync(d => order.NotifiedPartnersEmails.Contains(d.Email!), cancellationToken);

        return partners.Select(_mapper.Map<DeliveryPartnerDto>);
    }
}