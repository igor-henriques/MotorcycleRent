namespace MotorcycleRent.Infrastructure.Services;

public abstract class BaseRentCostCalculatorService
{
    public abstract int RentPeriodDays { get; }
    private readonly RentOptions _options;

    protected BaseRentCostCalculatorService(IOptions<RentOptions> options)
    {
        _options = options.Value;
    }

    public Domain.Entities.MotorcycleRent CalculateRentCost(Domain.Entities.MotorcycleRent motorcycleRent)
    {
        decimal dailyCost = _options.GetDailyPlanCost(motorcycleRent.RentPlan);
        decimal days = Math.Round((decimal)motorcycleRent.RentPeriod.NumberOfDays());

        int planRenovations = (int)Math.Floor(days / RentPeriodDays);
        if (planRenovations is 0)
        {
            planRenovations = 1;
        }

        decimal baseCost = Math.Round(planRenovations * RentPeriodDays * dailyCost);
        decimal earlyReturnFee = GetEarlyReturnFee(motorcycleRent, planRenovations);
        decimal lateReturnFee = GetLateReturnFee(motorcycleRent, planRenovations);

        return motorcycleRent with
        {
            RentCost = baseCost,
            FeeCost = earlyReturnFee + lateReturnFee
        };
    }

    private decimal GetLateReturnFee(Domain.Entities.MotorcycleRent motorcycleRent, int planRenovations)
    {
        var expectedReturnDate = motorcycleRent.RentPeriod.Start.AddDays(RentPeriodDays * planRenovations);
        var actualReturnDate = motorcycleRent.RentPeriod.End;
        var lateDays = Math.Round((actualReturnDate - expectedReturnDate).TotalDays);

        if (lateDays <= 0)
        {
            return 0;
        }

        return (decimal)lateDays * _options.DailyExceededFee;
    }

    private decimal GetEarlyReturnFee(Domain.Entities.MotorcycleRent motorcycleRent, int planRenovations)
    {
        var expectedReturnDate = motorcycleRent.RentPeriod.Start.AddDays(RentPeriodDays * planRenovations);
        var actualReturnDate = motorcycleRent.RentPeriod.End;

        double days = Math.Round((expectedReturnDate - actualReturnDate).TotalDays);
        if (days <= 0)
        {
            return 0;
        }

        var finePercentage = _options.GetEarlyReturnFeePercentage(motorcycleRent.RentPlan);
        return (decimal)days * _options.GetDailyPlanCost(motorcycleRent.RentPlan) * (finePercentage / 100);
    }

    public abstract bool CanCalculate(ERentPlan rentPlan);
}
