namespace MotorcycleRent.UnitTests.Entities;

public sealed class MotorcycleRentTests
{
    [Fact]
    public void ActualCost_Calculates_Correctly()
    {
        // Arrange
        var motorcycleRent = new MotorcycleRent.Domain.Entities.MotorcycleRent
        {
            RentCost = 100m,
            FeeCost = 50m
        };

        // Act
        var actualCost = motorcycleRent.ActualCost;

        // Assert
        Assert.Equal(150m, actualCost);
    }

    [Fact]
    public void ActualCost_Rounds_Correctly()
    {
        // Arrange
        var motorcycleRent = new MotorcycleRent.Domain.Entities.MotorcycleRent
        {
            RentCost = 100.12m,
            FeeCost = 50.95m
        };

        // Act
        var actualCost = motorcycleRent.ActualCost;

        // Assert
        Assert.Equal(151.07m, actualCost); // Assumes rounding to nearest 0.01
    }
}
