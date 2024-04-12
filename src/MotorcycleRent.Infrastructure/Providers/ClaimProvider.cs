namespace MotorcycleRent.Infrastructure.Providers;

/// <summary>
/// Provides a base class for retrieving specific claim values from the current authenticated user's claims.
/// </summary>
public abstract class ClaimProvider
{
    private readonly ILogger<ClaimProvider> _logger;
    private readonly IHttpContextAccessor _contextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimProvider"/> class.
    /// </summary>
    /// <param name="contextAccessor">Provides access to the HttpContext of the current request.</param>
    /// <param name="logger">A logger for logging errors or important information.</param>
    public ClaimProvider(IHttpContextAccessor contextAccessor, ILogger<ClaimProvider> logger)
    {
        _contextAccessor = contextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves the value of a specified claim type from the current user's claims.
    /// </summary>
    /// <param name="claimType">The type of the claim to retrieve.</param>
    /// <returns>The value of the requested claim type if it exists and is valid.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no authenticated user is found or the claim is missing or invalid.</exception>
    /// <remarks>
    /// This method checks if the user is authenticated and if the specified claim type exists in the user's claims.
    /// If the user is not authenticated or the claim does not exist or has no value, an exception is thrown.
    /// </remarks>
    public string GetClaimValue(string claimType)
    {
        if (!_contextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? true)
        {
            throw new InvalidOperationException("No authenticated user.");
        }

        var claim = _contextAccessor.HttpContext!.User.Claims.FirstOrDefault(c => c.Type == claimType);

        if (claim == null || string.IsNullOrWhiteSpace(claim.Value))
        {
            _logger.LogError("Claim type {ClaimType} is missing or invalid.", claimType);
            throw new InvalidOperationException($"An error occurred related to authentication");
        }

        return claim.Value;
    }
}
