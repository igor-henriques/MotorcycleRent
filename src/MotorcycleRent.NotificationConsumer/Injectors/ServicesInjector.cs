namespace MotorcycleRent.NotificationConsumer.Injectors;

internal static class ServicesInjector
{
    public static IHostBuilder ConfigureServices(this IHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.Configure<ConsumerOptions>(context.Configuration.GetSection(nameof(ConsumerOptions)));
            services.Configure<DatabaseOptions>(context.Configuration.GetSection(nameof(DatabaseOptions)));
            services.AddHostedService<OrderNotificationWorker>();
            services.AddRepositories();
        });

        return builder;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton(GetMongoClient);
        services.AddSingleton(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        return services;
    }

    private static MongoClient GetMongoClient(IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>();
        return new MongoClient(options.Value.ConnectionString);
    }
}
