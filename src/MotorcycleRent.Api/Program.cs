try
{
    var builder = WebApplication.CreateBuilder(args);

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
          .CreateLogger();

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();
    builder.Services.AddCustomAuthentication(builder.Configuration.GetValue<string>("JwtAuthenticationOptions:Key"));
    builder.Services.AddCustomAuthorization();
    builder.Services.AddSwagger();
    builder.Services.AddCustomOptions(builder.Configuration);
    builder.Services.AddRepositories();
    builder.Services.AddApplicationServices();
    builder.Services.AddDomainServices();
    builder.Services.AddInfrastructureServices();
    builder.Services.AddValidators();
    builder.Services.AddMapperProfiles();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHealthChecks();
    builder.Services.AddCors();
    builder.Services.AddHttpContextAccessor();
    builder.Services.Configure<JsonOptions>(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

    var app = builder.Build();

    app.UseMiddleware<HttpLoggingDetailsMiddleware>();
    app.UseExceptionHandler();

    if (app.Environment.IsDevelopment())
    {
        app.UseStaticFiles();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapHealthChecks(Routes.Health);
    app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    app.UseHttpsRedirection();

    app.ConfigureMotorcycleEndpoints();
    app.ConfigureUserEndpoints();
    app.ConfigureRentEndpoints();
    app.ConfigureDriverLicenseEndpoints();
    app.ConfigureOrderEndpoints();

    await SeedDatabaseAsync(app);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex.ToString());
    throw;
}
finally
{
    Log.CloseAndFlush();
}

static async Task SeedDatabaseAsync(WebApplication app)
{
    var seedService = app.Services.GetRequiredService<IDatabaseSeedService>();
    await seedService.SeedAsync();
}