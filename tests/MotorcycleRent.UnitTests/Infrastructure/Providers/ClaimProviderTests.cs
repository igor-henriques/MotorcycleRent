namespace MotorcycleRent.UnitTests.Infrastructure.Providers;

public sealed class ClaimProviderTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
    private readonly Mock<ILogger<ClaimProvider>> _loggerMock = new();
    private readonly Mock<HttpContext> _httpContextMock = new();
    private readonly ClaimsPrincipal _user = new(new ClaimsIdentity(
    [
        new Claim(ClaimTypes.Email, "test@example.com")
    ], "TestAuthentication"));

    public ClaimProviderTests()
    {
        _httpContextMock.Setup(ctx => ctx.User).Returns(_user);
        _httpContextAccessorMock.Setup(accessor => accessor.HttpContext).Returns(_httpContextMock.Object);
    }

    [Fact]
    public void GetClaimValue_ShouldReturnClaim_IfPresent()
    {
        // Arrange
        var provider = new TestClaimProvider(_httpContextAccessorMock.Object, _loggerMock.Object);

        // Act
        var result = provider.GetClaimValue(ClaimTypes.Email);

        // Assert
        Assert.Equal("test@example.com", result);
    }

    [Fact]
    public async Task GetClaimValue_ShouldThrow_IfUserNotAuthenticated()
    {
        // Arrange
        _httpContextMock.Setup(h => h.User.Identity!.IsAuthenticated).Returns(false);
        var provider = new TestClaimProvider(_httpContextAccessorMock.Object, _loggerMock.Object);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => Task.FromResult(provider.GetClaimValue(ClaimTypes.Email)));
        Assert.Equal("No authenticated user.", ex.Message);
    }

    [Fact]
    public void GetClaimValue_ShouldThrow_IfClaimIsMissing()
    {
        // Arrange
        var emptyUser = new ClaimsPrincipal(new ClaimsIdentity());
        _httpContextMock.Setup(ctx => ctx.User).Returns(emptyUser);
        var provider = new TestClaimProvider(_httpContextAccessorMock.Object, _loggerMock.Object);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => provider.GetClaimValue(ClaimTypes.Email));
        Assert.Contains("No authenticated user", ex.Message);
    }

    private class TestClaimProvider : ClaimProvider
    {
        public TestClaimProvider(IHttpContextAccessor contextAccessor, ILogger<ClaimProvider> logger) : base(contextAccessor, logger)
        {
        }

        public new string GetClaimValue(string claimType) => base.GetClaimValue(claimType);
    }
}
