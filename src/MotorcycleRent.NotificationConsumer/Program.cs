try
{
    await Host.CreateDefaultBuilder()
        .AddEnvironmentConfigurations(args)
        .AddLogging()
        .ConfigureServices()
        .Build()
        .RunAsync();
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Startup failed: {ex}");
}