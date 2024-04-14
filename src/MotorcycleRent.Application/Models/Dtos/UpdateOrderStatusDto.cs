using System.Text.Json.Serialization;

namespace MotorcycleRent.Application.Models.Dtos;

public sealed record UpdateOrderStatusDto : IDto
{
    public string? PublicOrderId { get; init; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EOrderStatus Status { get; init; }
}
