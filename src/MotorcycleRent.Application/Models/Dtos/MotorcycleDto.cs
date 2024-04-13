using System.Text.Json.Serialization;

namespace MotorcycleRent.Application.Models.Dtos;

public sealed record MotorcycleDto : IDto
{
    [SwaggerSchema(ReadOnly = true)]
    public Guid? Id { get; init; }
    public int Year { get; init; }
    public string? Model { get; init; }
    public string? Plate { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EMotorcycleStatus Status { get; init; }
}
