namespace MotorcycleRent.UnitTests.Application.Services;

public sealed class OrderServiceOrchestratorTests
{
    private readonly Mock<IBaseRepository<Order>> _orderRepositoryMock = new();
    private readonly Mock<ILogger<OrderServiceOrchestrator>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IPublisher<Order>> _publisherMock = new();
    private readonly Mock<IBaseRepository<DeliveryPartner>> _userRepositoryMock = new();
    private readonly Mock<IEmailClaimProvider> _emailClaimProviderMock = new();
    private readonly Mock<IOrderStatusManagementService> _orderStatusManagementServiceMock = new();
    private readonly OrderServiceOrchestrator _orchestrator;

    public OrderServiceOrchestratorTests()
    {
        _orchestrator = new OrderServiceOrchestrator(
            _orderRepositoryMock.Object,
            _loggerMock.Object, 
            _mapperMock.Object,
            _publisherMock.Object, 
            _emailClaimProviderMock.Object, 
            _userRepositoryMock.Object, 
            _orderStatusManagementServiceMock.Object);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldCreateAndPublishOrder()
    {
        // Arrange
        var orderDto = new OrderDto { DeliveryCost = 100, Status = EOrderStatus.Available };
        var order = Order.CreateNewOrder(10, EOrderStatus.Available);

        _orderRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _mapperMock.Setup(mapper => mapper.Map<OrderDto>(It.IsAny<Order>()))
            .Returns(orderDto);

        // Act
        var result = await _orchestrator.CreateOrderAsync(orderDto);

        // Assert
        _orderRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _publisherMock.Verify(pub => pub.PublishMessageAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(orderDto, result);
    }

    [Fact]
    public async Task CheckOrderAvailabilityAsync_ShouldOrderUnavailableException_IfOrderUnavailable()
    {
        // Arrange
        var publicOrderId = FriendlyIdGenerator.CreateFriendlyId(Guid.NewGuid());

        _orderRepositoryMock.Setup(repo => repo.GetByAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act & Assert
        await Assert.ThrowsAsync<OrderUnavailableException>(() => _orchestrator.CheckOrderAvailabilityAsync(publicOrderId));
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_ShouldUpdateStatus()
    {
        // Arrange
        var updateDto = new UpdateOrderStatusDto { PublicOrderId = "12345", Status = EOrderStatus.Accepted };
        var order = Order.CreateNewOrder(10, EOrderStatus.Available);
        var partner = new DeliveryPartner() { HasActiveRental = true };
        partner.Notifications.Add(OrderNotification.BuildFromOrder(order));

        _orderRepositoryMock.Setup(repo => repo.GetByAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _orderRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _userRepositoryMock.Setup(m => m.GetByAsync(It.IsAny<Expression<Func<DeliveryPartner, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(partner);

        // Act
        await _orchestrator.UpdateOrderStatusAsync(updateDto);

        // Assert
        _orderRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(repo => repo.GetByAsync(It.IsAny<Expression<Func<DeliveryPartner, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_ShouldThrowInvalidOperationException_WhenOrderNotFound()
    {
        // Arrange
        var updateDto = new UpdateOrderStatusDto { PublicOrderId = "12345", Status = EOrderStatus.Accepted };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _orchestrator.UpdateOrderStatusAsync(updateDto));        
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_ShouldThrowInvalidOperationException_WhenPartnerNotFound()
    {
        // Arrange
        var updateDto = new UpdateOrderStatusDto { PublicOrderId = "12345", Status = EOrderStatus.Accepted };
        var order = Order.CreateNewOrder(10, EOrderStatus.Available);
        _orderRepositoryMock.Setup(repo => repo.GetByAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _orchestrator.UpdateOrderStatusAsync(updateDto));
    }
}
