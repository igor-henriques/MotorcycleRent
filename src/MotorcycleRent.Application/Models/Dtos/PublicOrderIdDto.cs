namespace MotorcycleRent.Application.Models.Dtos;

/// <summary>
/// Represents a strong-typed data transfer object for public order identifiers, useful to apply validators
/// </summary>
public sealed record PublicOrderIdDto : IDto
{
    public string? PublicOrderId { get; init; }

    /// <summary>
    /// Tries to parse a public order identifier from a string, used by .NET internally to parse the information coming from the query parameters.
    /// </summary>
    /// <param name="publicOrderId">The string to parse.</param>
    /// <param name="publicOrderIdDto">The parsed <see cref="PublicOrderIdDto"/> if successful, otherwise null.</param>
    /// <returns><c>true</c> if parsing was successful, otherwise <c>false</c>.</returns>
    /// <remarks>
    /// This method returns false and sets the output to null if the input string is null or whitespace.
    /// </remarks>
    public static bool TryParse(string? publicOrderId, [NotNullWhen(true)] out PublicOrderIdDto? publicOrderIdDto)
    {
        if (string.IsNullOrWhiteSpace(publicOrderId))
        {
            publicOrderIdDto = null;
            return false;
        }

        publicOrderIdDto = new PublicOrderIdDto
        {
            PublicOrderId = publicOrderId
        };

        return true;
    }

    /// <summary>
    /// Converts a <see cref="PublicOrderIdDto"/> to a string representing the public order ID.
    /// </summary>
    /// <param name="publicOrderIdDto">The <see cref="PublicOrderIdDto"/> instance to convert.</param>
    /// <returns>A string that represents the public order ID.</returns>
    public static implicit operator string(PublicOrderIdDto publicOrderIdDto)
    {
        return publicOrderIdDto.PublicOrderId!;
    }

    /// <summary>
    /// Converts a string to a <see cref="PublicOrderIdDto"/>.
    /// </summary>
    /// <param name="publicOrder">The string to convert into a <see cref="PublicOrderIdDto"/>.</param>
    /// <returns>A new instance of <see cref="PublicOrderIdDto"/> initialized with the provided string.</returns>
    public static implicit operator PublicOrderIdDto(string publicOrder)
    {
        return new PublicOrderIdDto() { PublicOrderId = publicOrder };
    }
}
