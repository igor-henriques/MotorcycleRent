namespace MotorcycleRent.Application.Providers;

public sealed class EmailClaimProvider : ClaimProvider, IEmailClaimProvider
{
    public EmailClaimProvider(IHttpContextAccessor contextAccessor, ILogger<ClaimProvider> logger) : base(contextAccessor, logger)
    {
    }

    public string GetUserEmail()
    {
        return GetClaimValue(ClaimTypes.Email);
    }
}