using System.Text.Json.Serialization;

namespace MotorcycleRent.Application.Models.Dtos;

public sealed record UpdateMotorcycleStatusDto
{
    public string? Plate { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EMotorcycleStatus Status { get; init; }
}
