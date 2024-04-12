using System.Text.Json.Serialization;

namespace MotorcycleRent.Application.Models.Dtos;

public sealed class MotorcycleRentDto : IDto
{
    public DateTimeRange RentPeriod { get; init; }
    [JsonIgnore]
    public ERentPlan RentPlan => RentPlanHelper.GetRentPlan(RentPeriod);
}
