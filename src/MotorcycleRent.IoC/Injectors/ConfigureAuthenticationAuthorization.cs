namespace MotorcycleRent.IoC.Injectors;

public static class ConfigureAuthenticationAuthorization
{
    public static void AddCustomAuthentication(this IServiceCollection services, string? jwtBearerKey)
    {
        ArgumentValidator.ThrowIfNullOrEmpty(jwtBearerKey);

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(jwt =>
        {
            jwt.RequireHttpsMetadata = true;
            jwt.SaveToken = true;
            jwt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtBearerKey!)),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
        });
    }

    public static void AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("Administrator", policy => policy.RequireClaim(ClaimTypes.Role, "Administrator"))
            .AddPolicy("DeliveryPartner", policy => policy.RequireClaim(ClaimTypes.Role, "DeliveryPartner"));
    }
}
