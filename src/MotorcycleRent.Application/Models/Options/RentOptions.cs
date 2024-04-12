namespace MotorcycleRent.Application.Models.Options;

public sealed record RentOptions
{
    public decimal DailyExceededFee { get; init; }
    public EarlyReturnFee? EarlyReturnFeePercentage { get; init; }
    public PlanCost? RentPlanCost { get; init; }

    private Dictionary<ERentPlan, int>? _earlyReturnFeePercentageDictionary;
    private Dictionary<ERentPlan, decimal>? _rentPlanCostDictionary;

    public Dictionary<ERentPlan, int> EarlyReturnFeePercentageDictionary
        => _earlyReturnFeePercentageDictionary ??= InitializeEarlyReturnFeePercentageDictionary();
    public Dictionary<ERentPlan, decimal> RentPlanCostDictionary
        => _rentPlanCostDictionary ??= InitializeRentPlanCostDictionary();

    private Dictionary<ERentPlan, int> InitializeEarlyReturnFeePercentageDictionary()
    {
        return new Dictionary<ERentPlan, int>
        {
            { ERentPlan.Weekly, EarlyReturnFeePercentage?.WeeklyPercentage ?? 0 },
            { ERentPlan.Biweekly, EarlyReturnFeePercentage?.BiweeklyPercentage ?? 0 },
            { ERentPlan.Monthly, EarlyReturnFeePercentage?.MonthlyPercentage ?? 0 }
        };
    }

    private Dictionary<ERentPlan, decimal> InitializeRentPlanCostDictionary()
    {
        return new Dictionary<ERentPlan, decimal>
        {
            { ERentPlan.Weekly, RentPlanCost?.WeeklyDailyCost ?? 0 },
            { ERentPlan.Biweekly, RentPlanCost?.BiweeklyDailyCost ?? 0 },
            { ERentPlan.Monthly, RentPlanCost?.MonthlyDailyCost ?? 0 }
        };
    }
    public decimal GetDailyPlanCost(ERentPlan rentPlan)
    {
        if (!RentPlanCostDictionary.TryGetValue(rentPlan, out decimal cost))
        {
            throw new InvalidOperationException("Invalid rent plan");
        }

        return cost;
    }

    public decimal GetEarlyReturnFeePercentage(ERentPlan rentPlan)
    {
        if (!EarlyReturnFeePercentageDictionary.TryGetValue(rentPlan, out int earlyReturnFeePercentage))
        {
            throw new InvalidOperationException("Invalid rent plan");
        }

        return earlyReturnFeePercentage;
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
