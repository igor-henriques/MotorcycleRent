namespace MotorcycleRent.Domain.Interfaces;

public interface IRentCostCalculatorService
{
    /// <summary>
    /// Gets the number of days in the rent period used for calculating base rent cost.
    /// This value is specific to the implementing subclass.
    /// </summary>
    int RentPeriodDays { get; }

    /// <summary>
    /// Checks whether the class can calculate the rent cost for the specified rental plan.
    /// </summary>
    /// <param name="rentPlan">The rental plan to check.</param>
    /// <returns>True if the class can calculate the rent cost for the plan, false otherwise.</returns>
    bool CanCalculate(ERentPlan rentPlan);

    /// <summary>
    /// Calculates the rent cost for a motorcycle rental.
    /// </summary>
    /// <param name="motorcycleRent">The motorcycle rent information.</param>
    /// <returns>The updated motorcycle rent object with calculated rent cost and fee cost.</returns>
    Entities.MotorcycleRent CalculateRentCost(Entities.MotorcycleRent motorcycleRent);
}
