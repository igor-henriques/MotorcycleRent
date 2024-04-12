namespace MotorcycleRent.Application.Services;

/// <summary>
/// Provides an authentication token generator service.
/// </summary>
public interface ITokenGeneratorService
{
    /// <summary>
    /// Generates a new authentication token.
    /// </summary>
    /// <param name="claim"></param>
    /// <returns></returns>
    JwtToken GenerateToken(Claim claim);

    /// <summary>
    /// Generates a new authentication token.
    /// </summary>
    /// <param name="claims"></param>
    /// <returns></returns>
    JwtToken GenerateToken(IEnumerable<Claim> claims);
}
