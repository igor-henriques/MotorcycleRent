namespace MotorcycleRent.Domain.Entities;

[BsonIgnoreExtraElements]
public record User : BaseEntity
{
    public string? Email { get; init; }
    public string? HashedPassword { get; init; }
    public List<Claim> Claims { get; init; } = [];
}
