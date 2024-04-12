namespace MotorcycleRent.Domain.Helpers;

public static class RentPlanHelper
{
    public static ERentPlan GetRentPlan(DateTimeRange rentPeriod)
    {
        return (int)rentPeriod.NumberOfDays() switch
        {
            <= Constants.DEFAULT_WEEKLY_DAYS => ERentPlan.Weekly,
            > Constants.DEFAULT_WEEKLY_DAYS and < Constants.DEFAULT_MONTHLY_DAYS => ERentPlan.Biweekly,
            >= Constants.DEFAULT_MONTHLY_DAYS => ERentPlan.Monthly,
        };
    }
}
