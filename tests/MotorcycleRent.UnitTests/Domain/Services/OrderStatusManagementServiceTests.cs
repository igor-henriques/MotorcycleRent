namespace MotorcycleRent.UnitTests.Domain.Services;

public sealed class OrderStatusManagementServiceTests
{
    private readonly Mock<ILogger<OrderStatusManagementService>> _loggerMock;
    private readonly OrderStatusManagementService _service;

    public OrderStatusManagementServiceTests()
    {
        _loggerMock = new Mock<ILogger<OrderStatusManagementService>>();
        _service = new OrderStatusManagementService(_loggerMock.Object);
    }

    [Fact]
    public void HandleOrderStatusUpdate_WhenUpdateIsNotAllowed_ThrowsInvalidOperationException()
    {
        // Arrange
        var order = Order.CreateNewOrder(10, EOrderStatus.Available);
        var deliveryPartner = new DeliveryPartner() { HasActiveRental = true };
        var incomingStatus = EOrderStatus.Available;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            _service.HandleOrderStatusUpdate(order, deliveryPartner, incomingStatus));
    }

    [Fact]
    public void HandleOrderStatusUpdate_WhenOrderCannotBeAccepted_ThrowsPartnerUnableToAcceptOrderException()
    {
        // Arrange
        var order = Order.CreateNewOrder(10, EOrderStatus.Available);
        var deliveryPartner = new DeliveryPartner() { HasActiveRental = false };
        var incomingStatus = EOrderStatus.Accepted;

        // Act & Assert
        Assert.Throws<PartnerUnableToAcceptOrderException>(() =>
            _service.HandleOrderStatusUpdate(order, deliveryPartner, incomingStatus));
    }

    [Fact]
    public void HandleOrderStatusUpdate_WhenUpdateIsAllowed_ReturnsUpdatedOrderAndPartner()
    {
        // Arrange
        var order = Order.CreateNewOrder(10, EOrderStatus.Available);
        var deliveryPartner = new DeliveryPartner { IsAvailable = true, HasActiveRental = true };
        deliveryPartner.Notifications.Add(OrderNotification.BuildFromOrder(order));
        var incomingStatus = EOrderStatus.Accepted;

        // Act
        var result = _service.HandleOrderStatusUpdate(order, deliveryPartner, incomingStatus);

        // Assert
        Assert.Equal(EOrderStatus.Accepted, result.Item1.Status);
        Assert.False(result.Item2.IsAvailable);
    }

    [Fact]
    public void HandleOrderStatusUpdate_WhenWithdrawal_ShouldRemoveNotificationFromPartner()
    {
        // Arrange
        var order = Order.CreateNewOrder(10, EOrderStatus.Accepted);
        var deliveryPartner = new DeliveryPartner { IsAvailable = false, HasActiveRental = true };
        deliveryPartner.Notifications.Add(OrderNotification.BuildFromOrder(order));
        var incomingStatus = EOrderStatus.Available;

        // Act
        var result = _service.HandleOrderStatusUpdate(order, deliveryPartner, incomingStatus);

        // Assert
        Assert.Equal(incomingStatus, result.Item1.Status);
        Assert.True(result.Item2.IsAvailable);
        Assert.True(result.Item2.Notifications.Count is 0);
    }

    [Fact]
    public void HandleOrderStatusUpdate_WhenDelivered_ShouldRemoveNotificationFromPartner()
    {
        // Arrange
        var order = Order.CreateNewOrder(10, EOrderStatus.Accepted);
        var deliveryPartner = new DeliveryPartner { IsAvailable = false, HasActiveRental = true };
        deliveryPartner.Notifications.Add(OrderNotification.BuildFromOrder(order));
        var incomingStatus = EOrderStatus.Delivered;

        // Act
        var result = _service.HandleOrderStatusUpdate(order, deliveryPartner, incomingStatus);

        // Assert
        Assert.Equal(incomingStatus, result.Item1.Status);
        Assert.True(result.Item2.IsAvailable);
        Assert.True(result.Item2.Notifications.Count is 0);
    }
}