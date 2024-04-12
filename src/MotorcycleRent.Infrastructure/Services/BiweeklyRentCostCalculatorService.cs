namespace MotorcycleRent.Infrastructure.Services;

public sealed class BiweeklyRentCostCalculatorService : BaseRentCostCalculatorService, IRentCostCalculatorService
{
    public override int RentPeriodDays => 14;

    public BiweeklyRentCostCalculatorService(IOptions<RentOptions> options) : base(options) { }

    public override bool CanCalculate(ERentPlan rentPlan)
    {
        return rentPlan is ERentPlan.Biweekly;
    }
}
