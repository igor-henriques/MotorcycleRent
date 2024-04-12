namespace MotorcycleRent.Infrastructure.Services;

public sealed class MonthlyRentCostCalculatorService : BaseRentCostCalculatorService, IRentCostCalculatorService
{
    public override int RentPeriodDays => 30;

    public MonthlyRentCostCalculatorService(IOptions<RentOptions> options) : base(options) { }

    public override bool CanCalculate(ERentPlan rentPlan)
    {
        return rentPlan is ERentPlan.Monthly;
    }
}
