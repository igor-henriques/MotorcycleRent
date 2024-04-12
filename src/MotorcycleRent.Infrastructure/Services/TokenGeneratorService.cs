namespace MotorcycleRent.Infrastructure.Services;

/// <summary>
/// Provides services for generating JWT (JSON Web Tokens) for authentication purposes.
/// </summary>
public sealed class TokenGeneratorService : ITokenGeneratorService
{
    private readonly JwtAuthenticationOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenGeneratorService"/> class with specified JWT settings.
    /// </summary>
    /// <param name="options">The JWT configuration options.</param>
    public TokenGeneratorService(IOptions<JwtAuthenticationOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>
    /// Generates a JWT token containing the provided claims.
    /// </summary>
    /// <param name="claims">A collection of claims to include in the token.</param>
    /// <returns>A <see cref="JwtToken"/> object containing the token string and its expiration date.</returns>
    /// <remarks>
    /// This method creates a JWT token using the specified claims, signing the token with HMAC SHA256 algorithm.
    /// The expiration of the token is set based on the configured duration in <see cref="JwtAuthenticationOptions"/>.
    /// </remarks>
    public JwtToken GenerateToken(IEnumerable<Claim> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_options.Key!);
        var expiresAt = DateTime.UtcNow.AddHours(_options.TokenHoursDuration);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        if (claims != null)
        {
            tokenDescriptor.Subject = new ClaimsIdentity(claims);
        }

        var token = tokenHandler.CreateToken(tokenDescriptor);

        var jwt = tokenHandler.WriteToken(token);

        return new JwtToken
        {
            Token = jwt,
            ExpiresAt = expiresAt
        };
    }

    /// <summary>
    /// Generates a JWT token containing a single claim.
    /// </summary>
    /// <param name="claim">The claim to include in the token.</param>
    /// <returns>A <see cref="JwtToken"/> object containing the token string and its expiration date.</returns>
    public JwtToken GenerateToken(Claim claim)
    {
        return GenerateToken([claim]);
    }
}
