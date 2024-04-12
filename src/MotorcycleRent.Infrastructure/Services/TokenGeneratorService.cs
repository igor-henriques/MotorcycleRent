namespace MotorcycleRent.Infrastructure.Services;

public sealed class TokenGeneratorService : ITokenGeneratorService
{
    private readonly JwtAuthenticationOptions _options;

    public TokenGeneratorService(IOptions<JwtAuthenticationOptions> options)
    {
        _options = options.Value;
    }

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

    public JwtToken GenerateToken(Claim claim)
    {
        return GenerateToken([claim]);
    }
}
