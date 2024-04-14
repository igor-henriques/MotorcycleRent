namespace MotorcycleRent.Infrastructure.Services;

/// <summary>
/// Base class for calculating the rental cost of a motorcycle rental.
/// </summary>
public abstract class BaseRentalCostCalculatorService
{
    /// <summary>
    /// Gets the number of days in the rent period used for calculating base rent cost.
    /// This value is specific to the implementing subclass.
    /// </summary>
    public abstract int ExpectedRentalPeriodDays { get; }
    private readonly RentalOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseRentalCostCalculatorService"/> class.
    /// </summary>
    /// <param name="options">The configuration options for rent calculations.</param>
    protected BaseRentalCostCalculatorService(IOptions<RentalOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>
    /// Calculates the rental cost for a motorcycle rental.
    /// </summary>
    /// <param name="motorcycleRental">The motorcycle rent information.</param>
    /// <returns>The updated motorcycle rent object with calculated rent cost and fee cost.</returns>
    public MotorcycleRental CalculateRentalCost(MotorcycleRental motorcycleRental)
    {
        decimal dailyCost = _options.GetDailyPlanCost(motorcycleRental.RentalPlan);
        decimal days = Math.Round((decimal)motorcycleRental.RentalPeriod.NumberOfDays());
        int planRenovations = GetPlanRenovations(days);

        decimal baseCost = GetBaseCost(dailyCost, planRenovations);
        decimal earlyReturnFee = GetEarlyReturnFee(motorcycleRental, planRenovations);
        decimal lateReturnFee = GetLateReturnFee(motorcycleRental, planRenovations);

        return motorcycleRental with
        {
            RentalCost = baseCost,
            FeeCost = earlyReturnFee + lateReturnFee
        };
    }

    /// <summary>
    /// Get the plan renovations based on the number of days in the rent period.
    /// It should never be 0, as it'd affect the math for the base cost calculation and fees.
    /// </summary>
    /// <param name="days">Number of days</param>
    /// <returns></returns>
    private int GetPlanRenovations(decimal days)
    {
        var planRenovations = (int)Math.Floor(days / ExpectedRentalPeriodDays);
        return Math.Max(1, planRenovations);            
    }

    /// <summary>
    /// Calculates the base rental cost based on the pre-defined daily cost and the plan renovations amount
    /// </summary>
    /// <param name="MotorcycleRental">The motorcycle rent information.</param>
    /// <param name="planRenovations">The number of plan renovations.</param>
    /// <returns>The late return fee amount.</returns>
    private decimal GetBaseCost(decimal dailyCost, int planRenovations)
    {
        return Math.Round(planRenovations * ExpectedRentalPeriodDays * dailyCost, 2);
    }

    /// <summary>
    /// Calculates the late return fee based on the actual return date and expected return date.
    /// </summary>
    /// <param name="MotorcycleRental">The motorcycle rent information.</param>
    /// <param name="planRenovations">The number of plan renovations.</param>
    /// <returns>The late return fee amount.</returns>
    private decimal GetLateReturnFee(MotorcycleRental MotorcycleRental, int planRenovations)
    {
        var expectedReturnDate = MotorcycleRental.RentalPeriod.Start.AddDays(ExpectedRentalPeriodDays * planRenovations);
        var actualReturnDate = MotorcycleRental.RentalPeriod.End;
        var lateDays = Math.Max(0, Math.Round((actualReturnDate - expectedReturnDate).TotalDays));

        return (decimal)lateDays * _options.DailyExceededFee;
    }

    /// <summary>
    /// Calculates the early return fee based on the actual return date and expected return date.
    /// </summary>
    /// <param name="motorcycleRental">The motorcycle rent information.</param>
    /// <param name="planRenovations">The number of plan renovations.</param>
    /// <returns>The early return fee amount.</returns>
    private decimal GetEarlyReturnFee(MotorcycleRental motorcycleRental, int planRenovations)
    {
        var expectedReturnDate = motorcycleRental.RentalPeriod.Start.AddDays(ExpectedRentalPeriodDays * planRenovations);
        var actualReturnDate = motorcycleRental.RentalPeriod.End;

        decimal days = Math.Max(0, (decimal)Math.Round((expectedReturnDate - actualReturnDate).TotalDays));
        decimal feePercentage = _options.GetEarlyReturnFeePercentage(motorcycleRental.RentalPlan);

        return days * _options.GetDailyPlanCost(motorcycleRental.RentalPlan) * feePercentage;
    }

    /// <summary>
    /// Checks whether the class can calculate the rent cost for the specified rental plan.
    /// </summary>
    /// <param name="rentalPlan">The rental plan to check.</param>
    /// <returns>True if the class can calculate the rent cost for the plan, false otherwise.</returns>
    public abstract bool CanCalculate(ERentalPlan rentalPlan);
}
