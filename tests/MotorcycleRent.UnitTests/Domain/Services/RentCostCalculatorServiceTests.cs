namespace MotorcycleRent.UnitTests.Domain.Services;

public sealed class RentCostCalculatorServiceTests
{
    private readonly WeeklyRentalCostCalculatorService _weeklyRentCostCalculatorService;
    private readonly RentalOptions _rentalOptions;

    public RentCostCalculatorServiceTests()
    {
        _rentalOptions = new RentalOptions()
        {
            DailyExceededFee = 50,
            EarlyReturnFeePercentage = new RentalOptions.EarlyReturnFee()
            {
                WeeklyPercentage = 20,
                BiweeklyPercentage = 40,
                MonthlyPercentage = 60,
            },
            RentalPlanCost = new RentalOptions.PlanCost()
            {
                WeeklyDailyCost = 30,
                BiweeklyDailyCost = 28,
                MonthlyDailyCost = 22
            }
        };

        var options = Options.Create(_rentalOptions);
        _weeklyRentCostCalculatorService = new WeeklyRentalCostCalculatorService(options);
    }

    [Fact]
    public void CalculateRentalCost_ShouldAddFee_WhenEarlyReturning()
    {
        // Arrange
        int earlyReturnDaysCount = 2;
        var MotorcycleRental = new MotorcycleRental()
        {
            RentalPeriod = new DateTimeRange(
                DateTime.UtcNow.AddDays((_weeklyRentCostCalculatorService.RentPeriodDays * -1) + earlyReturnDaysCount),
                DateTime.UtcNow),
            RentalPlan = ERentalPlan.Weekly
        };

        // Act
        var calculatedRent = _weeklyRentCostCalculatorService.CalculateRentalCost(MotorcycleRental);

        // Assert
        Assert.True(calculatedRent.FeeCost > 0);
        Assert.True(calculatedRent.ActualCost > calculatedRent.RentalCost);
    }

    [Fact]
    public void CalculateRentalCost_ShouldAddFee_WhenLateReturning()
    {
        // Arrange
        int lateReturnDaysCount = 2;
        var motorcycleRental = new MotorcycleRental()
        {
            RentalPeriod = new DateTimeRange(
                DateTime.UtcNow.AddDays((_weeklyRentCostCalculatorService.RentPeriodDays * -1) - lateReturnDaysCount),
                DateTime.UtcNow),
            RentalPlan = ERentalPlan.Weekly
        };

        // Act
        var calculatedRent = _weeklyRentCostCalculatorService.CalculateRentalCost(motorcycleRental);

        // Assert
        Assert.True(calculatedRent.FeeCost > 0);
        Assert.True(calculatedRent.ActualCost > calculatedRent.RentalCost);
    }

    [Fact]
    public void CalculateRentalCost_ShouldNotAddFee_WhenRegularRental()
    {
        // Arrange
        int earlyReturnDaysCount = 0;
        var motorcycleRental = new MotorcycleRental()
        {
            RentalPeriod = new DateTimeRange(
                DateTime.UtcNow.AddDays((_weeklyRentCostCalculatorService.RentPeriodDays * -1) + earlyReturnDaysCount),
                DateTime.UtcNow),
            RentalPlan = ERentalPlan.Weekly
        };

        // Act
        var calculatedRent = _weeklyRentCostCalculatorService.CalculateRentalCost(motorcycleRental);

        // Assert
        Assert.True(calculatedRent.FeeCost == 0);
        Assert.True(calculatedRent.ActualCost == calculatedRent.RentalCost);
    }

    [Fact]
    public void CalculateRentalCost_ShouldConsiderPlanRenovations_WhenRegularRental()
    {
        // Arrange        
        var motorcycleRental = new MotorcycleRental()
        {
            RentalPeriod = new DateTimeRange(
                DateTime.UtcNow.AddDays((_weeklyRentCostCalculatorService.RentPeriodDays * -1) * 2),
                DateTime.UtcNow),
            RentalPlan = ERentalPlan.Weekly
        };        

        // Act
        var calculatedRent = _weeklyRentCostCalculatorService.CalculateRentalCost(motorcycleRental);

        // Assert
        Assert.True(calculatedRent.FeeCost == 0);
        Assert.True(calculatedRent.ActualCost == calculatedRent.RentalCost);
    }
}
