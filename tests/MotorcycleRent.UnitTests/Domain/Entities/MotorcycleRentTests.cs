namespace MotorcycleRent.UnitTests.Domain.Entities;

public sealed class MotorcycleRentalTests
{
    [Fact]
    public void ActualCost_Calculates_Correctly()
    {
        // Arrange
        var MotorcycleRental = new MotorcycleRental
        {
            RentalCost = 100m,
            FeeCost = 50m
        };

        // Act
        var actualCost = MotorcycleRental.ActualCost;

        // Assert
        Assert.Equal(150m, actualCost);
    }

    [Fact]
    public void ActualCost_Rounds_Correctly()
    {
        // Arrange
        var MotorcycleRental = new MotorcycleRental
        {
            RentalCost = 100.12m,
            FeeCost = 50.95m
        };

        // Act
        var actualCost = MotorcycleRental.ActualCost;

        // Assert
        Assert.Equal(151.07m, actualCost); // Assumes rounding to nearest 0.01
    }
}
