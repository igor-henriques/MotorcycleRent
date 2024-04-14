namespace MotorcycleRent.Infrastructure.Services;

public sealed class BiweeklyRentalCostCalculatorService : BaseRentalCostCalculatorService, IRentalCostCalculatorService
{
    public override int ExpectedRentalPeriodDays => (int)ERentalPlan.Biweekly;

    public BiweeklyRentalCostCalculatorService(IOptions<RentalOptions> options) : base(options) { }

    public override bool CanCalculate(ERentalPlan rentalPlan)
    {
        return rentalPlan is ERentalPlan.Biweekly;
    }
}
