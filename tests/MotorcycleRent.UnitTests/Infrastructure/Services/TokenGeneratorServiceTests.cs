namespace MotorcycleRent.UnitTests.Infrastructure.Services;

public sealed class TokenGeneratorServiceTests
{
    private readonly Mock<IOptions<JwtAuthenticationOptions>> _optionsMock;
    private readonly TokenGeneratorService _tokenGeneratorService;
    private readonly JwtAuthenticationOptions _options;

    public TokenGeneratorServiceTests()
    {
        _optionsMock = new Mock<IOptions<JwtAuthenticationOptions>>();
        _options = new JwtAuthenticationOptions
        {
            Key = "QovXn7kwrFz9eGRVcxWCS7EzOH1XCh1V",
            TokenHoursDuration = 2
        };
        _optionsMock.Setup(x => x.Value).Returns(_options);
        _tokenGeneratorService = new TokenGeneratorService(_optionsMock.Object);
    }

    [Fact]
    public void GenerateToken_SingleClaim_ReturnsValidJwt()
    {
        // Arrange
        var claim = new Claim(ClaimTypes.Name, "TestUser");

        // Act
        var jwtToken = _tokenGeneratorService.GenerateToken(new List<Claim> { claim });

        // Assert
        Assert.NotNull(jwtToken);
        Assert.NotNull(jwtToken.Token);
        Assert.True(jwtToken.ExpiresAt > DateTime.UtcNow);
    }  

    [Fact]
    public void GenerateToken_MultipleClaims_ReturnsValidJwtWithClaims()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim(ClaimTypes.Role, "Administrator")
        };

        // Act
        var jwtToken = _tokenGeneratorService.GenerateToken(claims);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(jwtToken.Token);
        var tokenClaims = token.Claims.ToList();

        foreach (var claim in claims)
        {
            Assert.True(tokenClaims.Any(c => c.Value == claim.Value),
                $"Claim {claim.Type}: {claim.Value} not found in the token.");
        }

        Assert.True(jwtToken.ExpiresAt > DateTime.UtcNow);
    }
}
