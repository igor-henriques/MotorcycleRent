namespace MotorcycleRent.Application.Services;

/// <summary>
/// Provides services for generating JWT (JSON Web Tokens) for authentication purposes.
/// </summary>
public interface ITokenGeneratorService
{
    /// <summary>
    /// Generates a JWT token containing the provided claims.
    /// </summary>
    /// <param name="claims">A collection of claims to include in the token.</param>
    /// <returns>A <see cref="JwtToken"/> object containing the token string and its expiration date.</returns>
    /// <remarks>
    /// This method creates a JWT token using the specified claims, signing the token with HMAC SHA256 algorithm.
    /// The expiration of the token is set based on the configured duration in <see cref="JwtAuthenticationOptions"/>.
    /// </remarks>
    JwtToken GenerateToken(Claim claim);

    /// <summary>
    /// Generates a JWT token containing a single claim.
    /// </summary>
    /// <param name="claim">The claim to include in the token.</param>
    /// <returns>A <see cref="JwtToken"/> object containing the token string and its expiration date.</returns>
    JwtToken GenerateToken(IEnumerable<Claim> claims);
}
