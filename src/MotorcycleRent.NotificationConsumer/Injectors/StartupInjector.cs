using Serilog;

namespace MotorcycleRent.NotificationConsumer.Injectors;

internal static class StartupInjector
{
    public static IHostBuilder AddEnvironmentConfigurations(this IHostBuilder builder, params string[] args)
    {
        builder.ConfigureHostConfiguration(configurationBuilder => configurationBuilder.AddCommandLine(args));

        builder.ConfigureAppConfiguration((hostingContext, config) =>
        {            
            config.AddEnvironmentVariables();
            config.SetBasePath(Directory.GetCurrentDirectory());
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            config.AddUserSecrets<Program>();
        });

        return builder;
    }

    public static IHostBuilder AddLogging(this IHostBuilder builder)
    {
        builder.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());

        return builder;
    }
}
