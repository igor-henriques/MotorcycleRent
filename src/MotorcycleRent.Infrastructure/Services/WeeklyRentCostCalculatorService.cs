namespace MotorcycleRent.Infrastructure.Services;

public sealed class WeeklyRentCostCalculatorService : BaseRentCostCalculatorService, IRentCostCalculatorService
{
    public override int RentPeriodDays => 7;

    public WeeklyRentCostCalculatorService(IOptions<RentOptions> options) : base(options) { }

    public override bool CanCalculate(ERentPlan rentPlan)
    {
        return rentPlan is ERentPlan.Weekly;
    }
}
