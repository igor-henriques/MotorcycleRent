namespace MotorcycleRent.Infrastructure.Providers;

public abstract class ClaimProvider
{
    private readonly ILogger<ClaimProvider> _logger;
    private readonly IHttpContextAccessor _contextAccessor;

    public ClaimProvider(IHttpContextAccessor contextAccessor, ILogger<ClaimProvider> logger)
    {
        _contextAccessor = contextAccessor;
        _logger = logger;
    }

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
