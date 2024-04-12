namespace MotorcycleRent.Infrastructure.Services;

/// <summary>
/// Base class for calculating the rent cost of a motorcycle rental.
/// </summary>
public abstract class BaseRentCostCalculatorService
{
    /// <summary>
    /// Gets the number of days in the rent period used for calculating base rent cost.
    /// This value is specific to the implementing subclass.
    /// </summary>
    public abstract int RentPeriodDays { get; }
    private readonly RentOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseRentCostCalculatorService"/> class.
    /// </summary>
    /// <param name="options">The configuration options for rent calculations.</param>
    protected BaseRentCostCalculatorService(IOptions<RentOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>
    /// Calculates the rent cost for a motorcycle rental.
    /// </summary>
    /// <param name="motorcycleRent">The motorcycle rent information.</param>
    /// <returns>The updated motorcycle rent object with calculated rent cost and fee cost.</returns>
    public Domain.Entities.MotorcycleRent CalculateRentCost(Domain.Entities.MotorcycleRent motorcycleRent)
    {
        decimal dailyCost = _options.GetDailyPlanCost(motorcycleRent.RentPlan);
        decimal days = Math.Round((decimal)motorcycleRent.RentPeriod.NumberOfDays());

        int planRenovations = (int)Math.Floor(days / RentPeriodDays);
        if (planRenovations is 0)
        {
            planRenovations = 1;
        }

        decimal baseCost = Math.Round(planRenovations * RentPeriodDays * dailyCost, 2);
        decimal earlyReturnFee = GetEarlyReturnFee(motorcycleRent, planRenovations);
        decimal lateReturnFee = GetLateReturnFee(motorcycleRent, planRenovations);

        return motorcycleRent with
        {
            RentCost = baseCost,
            FeeCost = earlyReturnFee + lateReturnFee
        };
    }

    /// <summary>
    /// Calculates the late return fee based on the actual return date and expected return date.
    /// </summary>
    /// <param name="motorcycleRent">The motorcycle rent information.</param>
    /// <param name="planRenovations">The number of plan renovations.</param>
    /// <returns>The late return fee amount.</returns>
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

    /// <summary>
    /// Calculates the early return fee based on the actual return date and expected return date.
    /// </summary>
    /// <param name="motorcycleRent">The motorcycle rent information.</param>
    /// <param name="planRenovations">The number of plan renovations.</param>
    /// <returns>The early return fee amount.</returns>
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

    /// <summary>
    /// Checks whether the class can calculate the rent cost for the specified rental plan.
    /// </summary>
    /// <param name="rentPlan">The rental plan to check.</param>
    /// <returns>True if the class can calculate the rent cost for the plan, false otherwise.</returns>
    public abstract bool CanCalculate(ERentPlan rentPlan);
}
