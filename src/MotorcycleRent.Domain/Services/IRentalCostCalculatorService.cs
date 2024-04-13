namespace MotorcycleRent.Domain.Interfaces;

public interface IRentalCostCalculatorService
{
    /// <summary>
    /// Gets the number of days in the rent period used for calculating base rent cost.
    /// This value is specific to the implementing subclass.
    /// </summary>
    int RentPeriodDays { get; }

    /// <summary>
    /// Checks whether the class can calculate the rent cost for the specified rental plan.
    /// </summary>
    /// <param name="rentalPlan">The rental plan to check.</param>
    /// <returns>True if the class can calculate the rent cost for the plan, false otherwise.</returns>
    bool CanCalculate(ERentalPlan rentalPlan);

    /// <summary>
    /// Calculates the rental cost for a motorcycle rental.
    /// </summary>
    /// <param name="MotorcycleRental">The motorcycle rent information.</param>
    /// <returns>The updated motorcycle rent object with calculated rent cost and fee cost.</returns>
    MotorcycleRental CalculateRentalCost(MotorcycleRental MotorcycleRental);
}
