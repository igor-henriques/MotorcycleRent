using System.Text.Json.Serialization;

namespace MotorcycleRent.Application.Models.Dtos;

public sealed class MotorcycleRentalDto : IDto
{
    public DateTimeRange RentalPeriod { get; init; }
    [JsonIgnore]
    public ERentalPlan RentalPlan => RentPlanHelper.GetRentalPlan(RentalPeriod);
}
