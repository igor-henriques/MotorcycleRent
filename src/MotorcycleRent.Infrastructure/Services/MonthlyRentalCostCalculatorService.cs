﻿namespace MotorcycleRent.Infrastructure.Services;

public sealed class MonthlyRentalCostCalculatorService : BaseRentalCostCalculatorService, IRentalCostCalculatorService
{
    public override int RentPeriodDays => 30;

    public MonthlyRentalCostCalculatorService(IOptions<RentalOptions> options) : base(options) { }

    public override bool CanCalculate(ERentalPlan rentalPlan)
    {
        return rentalPlan is ERentalPlan.Monthly;
    }
}
