using EtsyAnalyzer.ConsoleApp.Configuration;
using EtsyAnalyzer.ConsoleApp.UI;
using EtsyAnalyzer.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

try
{
    // Build configuration
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .AddEnvironmentVariables()
        .Build();

    // Configure Serilog
    DependencyInjection.ConfigureSerilog(configuration);

    Log.Information("Starting EtsyAnalyzer application");

    // Build host
    var host = Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureServices((context, services) =>
        {
            services.AddEtsyAnalyzerServices(configuration);
            services.AddSingleton<ConsoleUI>();
        })
        .Build();

    // Ensure database is created
    await DependencyInjection.EnsureDatabaseCreatedAsync(host.Services);

    Log.Information("EtsyAnalyzer initialized successfully");

    // Run console UI
    using (var scope = host.Services.CreateScope())
    {
        var consoleUI = scope.ServiceProvider.GetRequiredService<ConsoleUI>();
        await consoleUI.RunAsync();
    }

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    Console.WriteLine($"Fatal error: {ex.Message}");
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

