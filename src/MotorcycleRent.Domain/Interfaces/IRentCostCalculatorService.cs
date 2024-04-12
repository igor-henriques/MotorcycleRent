namespace MotorcycleRent.Domain.Interfaces;

public interface IRentCostCalculatorService
{
    int RentPeriodDays { get; }
    bool CanCalculate(ERentPlan rentPlan);
    Entities.MotorcycleRent CalculateRentCost(Entities.MotorcycleRent motorcycleRent);
}
