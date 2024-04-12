namespace MotorcycleRent.Domain.Helpers;

public static class RentPlanHelper
{
    /// <summary>
    /// Determines the appropriate rent plan based on the duration of the rental period.
    /// </summary>
    /// <param name="rentPeriod">A <see cref="DateTimeRange"/> object representing the start and end date of the rental period.</param>
    /// <returns>
    /// Returns an <see cref="ERentPlan"/> enumeration that indicates the rent plan:
    /// <list type="bullet">
    /// <item>
    /// <description>Weekly: If the number of days is less than or equal to 7.</description>
    /// </item>
    /// <item>
    /// <description>Biweekly: If the number of days is more than 7 but less than 30.</description>
    /// </item>
    /// <item>
    /// <description>Monthly: If the number of days is 30 or more.</description>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// The method calculates the number of days in the rental period using the <see cref="DateTimeRange.NumberOfDays"/> method.
    /// It then determines the rent plan based on predefined constants:
    /// <see cref="Constants.DEFAULT_WEEKLY_DAYS"/>, <see cref="Constants.DEFAULT_BIWEEKLY_DAYS"/>, and <see cref="Constants.DEFAULT_MONTHLY_DAYS"/>.
    /// These constants define the thresholds for each rental plan type.
    /// </remarks>
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
