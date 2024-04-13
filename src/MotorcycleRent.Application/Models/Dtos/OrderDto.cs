using System.Text.Json.Serialization;

namespace MotorcycleRent.Application.Models.Dtos;

public sealed record OrderDto : IDto
{
    [SwaggerSchema(ReadOnly = true)]
    public string? PublicOrderId { get; init; }
    public decimal DeliveryCost { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EOrderStatus Status { get; init; }
}