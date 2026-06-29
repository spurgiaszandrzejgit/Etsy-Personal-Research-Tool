using EtsyAnalyzer.Analytics.Services;
using EtsyAnalyzer.Core.Interfaces;
using EtsyAnalyzer.EtsyApi.Client;
using EtsyAnalyzer.EtsyApi.Configuration;
using EtsyAnalyzer.EtsyApi.Services;
using EtsyAnalyzer.Infrastructure.Persistence;
using EtsyAnalyzer.Infrastructure.Persistence.Repositories;
using EtsyAnalyzer.Infrastructure.Reporting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace EtsyAnalyzer.ConsoleApp.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddEtsyAnalyzerServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configuration
        services.Configure<EtsyApiConfiguration>(
            configuration.GetSection(EtsyApiConfiguration.SectionName));

        // Database
        var connectionString = configuration.GetConnectionString("Default") 
            ?? configuration["Database:ConnectionString"]
            ?? "Data Source=etsyanalyzer.db";

        services.AddDbContext<EtsyAnalyzerDbContext>(options =>
            options.UseSqlite(connectionString));

        // Repositories & Unit of Work
        services.AddScoped<IListingRepository, ListingRepository>();
        services.AddScoped<IShopRepository, ShopRepository>();
        services.AddScoped<ISearchQueryRepository, SearchQueryRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Etsy API Client
        services.AddHttpClient<EtsyApiClient>((serviceProvider, client) =>
        {
            var config = configuration.GetSection(EtsyApiConfiguration.SectionName)
                .Get<EtsyApiConfiguration>() ?? new EtsyApiConfiguration();

            client.BaseAddress = new Uri(config.ApiUrl);
            client.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);
        });

        // Data Source
        services.AddScoped<IDataSource, EtsyDataSource>();

        // Analytics Services
        services.AddScoped<PriceAnalyzer>();
        services.AddScoped<KeywordAnalyzer>();
        services.AddScoped<CompetitionAnalyzer>();
        services.AddScoped<NicheScoreCalculator>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();

        // Report Generator
        services.AddScoped<IReportGenerator, ExcelReportGenerator>();

        // Logging
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddSerilog(dispose: true);
        });

        return services;
    }

    public static void ConfigureSerilog(IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .CreateLogger();
    }

    public static async Task EnsureDatabaseCreatedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EtsyAnalyzerDbContext>();

        try
        {
            await context.Database.EnsureCreatedAsync();
            Log.Information("Database initialized successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating database");
            throw;
        }
    }
}
