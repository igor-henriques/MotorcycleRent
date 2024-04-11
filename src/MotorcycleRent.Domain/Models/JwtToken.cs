namespace MotorcycleRent.Domain.Models;

public sealed record JwtToken
{
    public string? Token { get; init; }
    public DateTime ExpiresAt { get; init; }
}
