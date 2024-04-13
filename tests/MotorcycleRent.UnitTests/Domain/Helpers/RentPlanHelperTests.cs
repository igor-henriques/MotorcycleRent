namespace MotorcycleRent.UnitTests.Domain.Helpers;

public sealed class RentPlanHelperTests
{
    [Fact]
    public void GetRentPlan_ReturnsWeekly_WhenDaysAreSevenOrLess()
    {
        // Arrange
        var rentPeriod = new DateTimeRange(DateTime.Today, DateTime.Today.AddDays(6)); // 7 days or less

        // Act
        var plan = RentPlanHelper.GetRentalPlan(rentPeriod);

        // Assert
        Assert.Equal(ERentalPlan.Weekly, plan);
    }

    [Fact]
    public void GetRentPlan_ReturnsBiweekly_WhenDaysAreMoreThanSevenAndLessThanThirty()
    {
        // Arrange
        var rentPeriod = new DateTimeRange(DateTime.Today, DateTime.Today.AddDays(13)); // More than 7 days, less than 30

        // Act
        var plan = RentPlanHelper.GetRentalPlan(rentPeriod);

        // Assert
        Assert.Equal(ERentalPlan.Biweekly, plan);
    }

    [Fact]
    public void GetRentPlan_ReturnsMonthly_WhenDaysAreThirtyOrMore()
    {
        // Arrange
        var rentPeriod = new DateTimeRange(DateTime.Today, DateTime.Today.AddDays(30)); // 30 days or more

        // Act
        var plan = RentPlanHelper.GetRentalPlan(rentPeriod);

        // Assert
        Assert.Equal(ERentalPlan.Monthly, plan);
    }
}
