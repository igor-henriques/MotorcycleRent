namespace MotorcycleRent.UnitTests.Domain.Entities;

public sealed class OrderTests
{
    [Fact]
    public void CreateNewOrder_SetsAllPropertiesCorrectly()
    {
        // Arrange
        var deliveryCost = 100m;
        var status = EOrderStatus.Available;

        // Act
        var order = Order.CreateNewOrder(deliveryCost, status);

        // Assert
        Assert.True(order.Id != Guid.Empty);
        Assert.True((DateTime.UtcNow - order.CreationDate).TotalSeconds < 1);
        Assert.Equal(deliveryCost, order.DeliveryCost);
        Assert.Equal(status, order.Status);
    }

    [Theory]
    [InlineData(EOrderStatus.Available, false, true)]
    [InlineData(EOrderStatus.Available, true, false)]
    [InlineData(EOrderStatus.Accepted, true, false)]
    [InlineData(EOrderStatus.Delivered, true, false)]
    public void IsOrderAvailableToPickup_ReturnsCorrectValue(EOrderStatus status, bool hasDeliveryPartner, bool expected)
    {
        // Arrange
        var order = Order.CreateNewOrder(5, status) with { DeliveryPartner = hasDeliveryPartner ? new() : null };

        // Act
        var isAvailable = order.IsOrderAvailableToDelivery;

        // Assert
        Assert.Equal(expected, isAvailable);
    }

    [Theory]
    [InlineData(EOrderStatus.Available, EOrderStatus.Available, false)]
    [InlineData(EOrderStatus.Available, EOrderStatus.Accepted, true)]
    [InlineData(EOrderStatus.Accepted, EOrderStatus.Available, true)]
    [InlineData(EOrderStatus.Delivered, EOrderStatus.Accepted, false)]
    [InlineData(EOrderStatus.Delivered, EOrderStatus.Available, false)]
    [InlineData(EOrderStatus.Accepted, EOrderStatus.Delivered, true)]
    public void CanUpdateStatus_ReturnsCorrectValue(EOrderStatus currentStatus, EOrderStatus newStatus, bool expected)
    {
        // Arrange
        var order = Order.CreateNewOrder(1, currentStatus);

        // Act
        var CanUpdateStatus = order.CanUpdateStatus(newStatus);

        // Assert
        Assert.Equal(expected, CanUpdateStatus);
    }

    [Fact]
    public void CanBeAccepted_ReturnsTrue_WhenStatusIsAvailableAndPartnerCanAccept()
    {
        // Arrange        
        var order = Order.CreateNewOrder(1, EOrderStatus.Available);
        var partner = new DeliveryPartner() { IsAvailable = true, HasActiveRental = true };
        partner.Notifications.Add(OrderNotification.BuildFromOrder(order));

        // Act
        bool result = order.CanBeAccepted(partner);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanBeAccepted_ReturnsFalse_WhenPartnerHasNoActiveRental()
    {
        // Arrange        
        var order = Order.CreateNewOrder(1, EOrderStatus.Available);
        var partner = new DeliveryPartner() { IsAvailable = true, HasActiveRental = false };
        partner.Notifications.Add(OrderNotification.BuildFromOrder(order));

        // Act
        bool result = order.CanBeAccepted(partner);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanBeAccepted_ReturnsFalse_WhenPartnerIsUnavailable()
    {
        // Arrange        
        var order = Order.CreateNewOrder(1, EOrderStatus.Available);
        var partner = new DeliveryPartner() { IsAvailable = false, HasActiveRental = true };
        partner.Notifications.Add(OrderNotification.BuildFromOrder(order));

        // Act
        bool result = order.CanBeAccepted(partner);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanBeAccepted_ReturnsFalse_WhenPartnerWasNotNotified()
    {
        // Arrange        
        var order = Order.CreateNewOrder(1, EOrderStatus.Available);
        var partner = new DeliveryPartner() { IsAvailable = true, HasActiveRental = true };        

        // Act
        bool result = order.CanBeAccepted(partner);

        // Assert
        Assert.False(result);
    }
}
