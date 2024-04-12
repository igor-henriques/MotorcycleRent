namespace MotorcycleRent.IoC.Injectors;

public static class ConfigureRepositories
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton(GetMongoClient);
        services.AddSingleton(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        return services;
    }

    private static MongoClient GetMongoClient(IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetRequiredService<IOptions<MotorcycleRentDatabaseOptions>>();
        return new MongoClient(options.Value.ConnectionString);
    }
}
