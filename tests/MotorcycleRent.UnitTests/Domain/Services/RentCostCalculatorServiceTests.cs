namespace MotorcycleRent.UnitTests.Domain.Services;

public sealed class RentCostCalculatorServiceTests
{
    private readonly WeeklyRentCostCalculatorService _weeklyRentCostCalculatorService;
    private readonly RentOptions _rentOptions;

    public RentCostCalculatorServiceTests()
    {
        _rentOptions = new RentOptions()
        {
            DailyExceededFee = 50,
            EarlyReturnFeePercentage = new RentOptions.EarlyReturnFee()
            {
                WeeklyPercentage = 20,
                BiweeklyPercentage = 40,
                MonthlyPercentage = 60,
            },
            RentPlanCost = new RentOptions.PlanCost()
            {
                WeeklyDailyCost = 30,
                BiweeklyDailyCost = 28,
                MonthlyDailyCost = 22
            }
        };

        var options = Options.Create(_rentOptions);
        _weeklyRentCostCalculatorService = new WeeklyRentCostCalculatorService(options);
    }

    [Fact]
    public void CalculateRentCost_ShouldAddFee_WhenEarlyReturning()
    {
        // Arrange
        int earlyReturnDaysCount = 2;
        var motorcycleRent = new MotorcycleRent.Domain.Entities.MotorcycleRent()
        {
            RentPeriod = new DateTimeRange(
                DateTime.Now.AddDays((_weeklyRentCostCalculatorService.RentPeriodDays * -1) + earlyReturnDaysCount),
                DateTime.Now),
            RentPlan = ERentPlan.Weekly
        };

        // Act
        var calculatedRent = _weeklyRentCostCalculatorService.CalculateRentCost(motorcycleRent);

        // Assert
        Assert.True(calculatedRent.FeeCost > 0);
        Assert.True(calculatedRent.ActualCost > calculatedRent.RentCost);
    }

    [Fact]
    public void CalculateRentCost_ShouldAddFee_WhenLateReturning()
    {
        // Arrange
        int lateReturnDaysCount = 2;
        var motorcycleRent = new MotorcycleRent.Domain.Entities.MotorcycleRent()
        {
            RentPeriod = new DateTimeRange(
                DateTime.Now.AddDays((_weeklyRentCostCalculatorService.RentPeriodDays * -1) - lateReturnDaysCount),
                DateTime.Now),
            RentPlan = ERentPlan.Weekly
        };

        // Act
        var calculatedRent = _weeklyRentCostCalculatorService.CalculateRentCost(motorcycleRent);

        // Assert
        Assert.True(calculatedRent.FeeCost > 0);
        Assert.True(calculatedRent.ActualCost > calculatedRent.RentCost);
    }

    [Fact]
    public void CalculateRentCost_ShouldNotAddFee_WhenRegularRent()
    {
        // Arrange
        int earlyReturnDaysCount = 0;
        var motorcycleRent = new MotorcycleRent.Domain.Entities.MotorcycleRent()
        {
            RentPeriod = new DateTimeRange(
                DateTime.Now.AddDays((_weeklyRentCostCalculatorService.RentPeriodDays * -1) + earlyReturnDaysCount),
                DateTime.Now),
            RentPlan = ERentPlan.Weekly
        };

        // Act
        var calculatedRent = _weeklyRentCostCalculatorService.CalculateRentCost(motorcycleRent);

        // Assert
        Assert.True(calculatedRent.FeeCost == 0);
        Assert.True(calculatedRent.ActualCost == calculatedRent.RentCost);
    }
}
