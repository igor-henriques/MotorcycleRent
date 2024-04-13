namespace MotorcycleRent.Application.Models.Options;

public sealed record RentalOptions
{
    public decimal DailyExceededFee { get; init; }
    public EarlyReturnFee? EarlyReturnFeePercentage { get; init; }
    public PlanCost? RentalPlanCost { get; init; }

    private Dictionary<ERentalPlan, int>? _earlyReturnFeePercentageDictionary;
    private Dictionary<ERentalPlan, decimal>? _rentPlanCostDictionary;

    public Dictionary<ERentalPlan, int> EarlyReturnFeePercentageDictionary
        => _earlyReturnFeePercentageDictionary ??= InitializeEarlyReturnFeePercentageDictionary();
    public Dictionary<ERentalPlan, decimal> RentPlanCostDictionary
        => _rentPlanCostDictionary ??= InitializeRentPlanCostDictionary();

    private Dictionary<ERentalPlan, int> InitializeEarlyReturnFeePercentageDictionary()
    {
        return new Dictionary<ERentalPlan, int>
        {
            { ERentalPlan.Weekly, EarlyReturnFeePercentage?.WeeklyPercentage ?? 0 },
            { ERentalPlan.Biweekly, EarlyReturnFeePercentage?.BiweeklyPercentage ?? 0 },
            { ERentalPlan.Monthly, EarlyReturnFeePercentage?.MonthlyPercentage ?? 0 }
        };
    }

    private Dictionary<ERentalPlan, decimal> InitializeRentPlanCostDictionary()
    {
        return new Dictionary<ERentalPlan, decimal>
        {
            { ERentalPlan.Weekly, RentalPlanCost?.WeeklyDailyCost ?? 0 },
            { ERentalPlan.Biweekly, RentalPlanCost?.BiweeklyDailyCost ?? 0 },
            { ERentalPlan.Monthly, RentalPlanCost?.MonthlyDailyCost ?? 0 }
        };
    }
    public decimal GetDailyPlanCost(ERentalPlan rentPlan)
    {
        if (!RentPlanCostDictionary.TryGetValue(rentPlan, out decimal cost))
        {
            throw new InvalidOperationException(Constants.Messages.InvalidRentalPlan);
        }

        return cost;
    }

    public decimal GetEarlyReturnFeePercentage(ERentalPlan rentPlan)
    {
        if (!EarlyReturnFeePercentageDictionary.TryGetValue(rentPlan, out int earlyReturnFeePercentage))
        {
            throw new InvalidOperationException(Constants.Messages.InvalidRentalPlan);
        }

        return (decimal)earlyReturnFeePercentage / (decimal)100;
    }

    public sealed record EarlyReturnFee
    {
        public int WeeklyPercentage { get; init; }
        public int BiweeklyPercentage { get; init; }
        public int MonthlyPercentage { get; init; }
    }

    public sealed record PlanCost
    {
        public decimal WeeklyDailyCost { get; init; }
        public decimal BiweeklyDailyCost { get; init; }
        public decimal MonthlyDailyCost { get; init; }
    }
}
