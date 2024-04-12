using System.Text.Json.Serialization;

namespace MotorcycleRent.Application.Models.Dtos;

public sealed record UpdateMotorcycleStateDto
{
    public string? Plate { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EMotorcycleState State { get; init; }
}
