namespace MotorcycleRent.Infrastructure.Services;

public sealed class WeeklyRentalCostCalculatorService : BaseRentalCostCalculatorService, IRentalCostCalculatorService
{
    public override int RentPeriodDays => 7;

    public WeeklyRentalCostCalculatorService(IOptions<RentalOptions> options) : base(options) { }

    public override bool CanCalculate(ERentalPlan rentalPlan)
    {
        return rentalPlan is ERentalPlan.Weekly;
    }
}
