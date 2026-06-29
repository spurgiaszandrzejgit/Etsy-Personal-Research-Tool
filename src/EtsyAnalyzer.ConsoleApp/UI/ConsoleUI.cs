using EtsyAnalyzer.Core.DTOs;
using EtsyAnalyzer.Core.Interfaces;
using EtsyAnalyzer.Core.ValueObjects;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

namespace EtsyAnalyzer.ConsoleApp.UI;

public class ConsoleUI
{
    private readonly IDataSource _dataSource;
    private readonly IAnalyticsService _analyticsService;
    private readonly IReportGenerator _reportGenerator;
    private readonly IConfiguration _configuration;

    public ConsoleUI(
        IDataSource dataSource,
        IAnalyticsService analyticsService,
        IReportGenerator reportGenerator,
        IConfiguration configuration)
    {
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        _analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));
        _reportGenerator = reportGenerator ?? throw new ArgumentNullException(nameof(reportGenerator));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task RunAsync()
    {
        ShowWelcome();

        while (true)
        {
            var choice = ShowMainMenu();

            switch (choice)
            {
                case "Analyze New Niche":
                    await AnalyzeNewNicheAsync();
                    break;
                case "View Statistics":
                    ShowStatistics();
                    break;
                case "Settings":
                    ShowSettings();
                    break;
                case "Exit":
                    ShowGoodbye();
                    return;
            }

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
            Console.ReadKey(true);
            AnsiConsole.Clear();
        }
    }

    private void ShowWelcome()
    {
        AnsiConsole.Clear();

        var rule = new Rule("[bold yellow]EtsyAnalyzer[/]")
        {
            Justification = Justify.Left
        };
        AnsiConsole.Write(rule);

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[dim]Professional Etsy Market Analysis Tool[/]");
        AnsiConsole.MarkupLine("[dim]Find profitable niches with data-driven insights[/]");
        AnsiConsole.WriteLine();
    }

    private string ShowMainMenu()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]What would you like to do?[/]")
                .PageSize(10)
                .AddChoices(new[] {
                    "Analyze New Niche",
                    "View Statistics",
                    "Settings",
                    "Exit"
                }));
    }

    private async Task AnalyzeNewNicheAsync()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Niche Analysis[/]").LeftJustified());
        AnsiConsole.WriteLine();

        // Get search query
        var query = AnsiConsole.Ask<string>("[green]Enter search query:[/] ");

        if (string.IsNullOrWhiteSpace(query))
        {
            AnsiConsole.MarkupLine("[red]Query cannot be empty![/]");
            return;
        }

        try
        {
            AnalyticsSummaryDto? summary = null;
            List<ListingDto>? listings = null;

            // Show progress
            await AnsiConsole.Progress()
                .AutoClear(false)
                .Columns(new ProgressColumn[]
                {
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new SpinnerColumn(),
                })
                .StartAsync(async ctx =>
                {
                    // Task 1: Fetch data
                    var task1 = ctx.AddTask("[yellow]Fetching data from Etsy API...[/]");
                    summary = await _analyticsService.AnalyzeNicheAsync(query);
                    listings = (await _dataSource.SearchListingsAsync(query, 200)).ToList();
                    task1.Increment(100);

                    // Task 2: Analyze
                    var task2 = ctx.AddTask("[yellow]Analyzing market data...[/]");
                    await Task.Delay(500); // Симуляция обработки
                    task2.Increment(100);

                    // Task 3: Generate report
                    var task3 = ctx.AddTask("[yellow]Generating Excel report...[/]");
                    var outputDir = _configuration["Reports:OutputDirectory"] ?? "./Reports";
                    var fileName = $"EtsyAnalysis_{query.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    var outputPath = Path.Combine(outputDir, fileName);

                    await _reportGenerator.GenerateReportAsync(summary!, listings!, outputPath);
                    task3.Increment(100);
                });

            // Display results
            DisplayAnalysisResults(summary!);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");

            // Check if it's an API configuration issue
            if (ex.Message.Contains("404") || ex.Message.Contains("NotFound") || 
                ex.Message.Contains("Unauthorized") || ex.Message.Contains("401"))
            {
                AnsiConsole.WriteLine();
                AnsiConsole.Write(new Panel(
                    "[yellow]Possible API Key Issue[/]\n\n" +
                    "1. Get your API key from: [link]https://www.etsy.com/developers/[/]\n" +
                    "2. Open: [cyan]src/EtsyAnalyzer.ConsoleApp/appsettings.json[/]\n" +
                    "3. Replace [green]YOUR_ETSY_API_KEY_HERE[/] with your actual key\n" +
                    "4. Restart the application\n\n" +
                    "See [cyan]TROUBLESHOOTING.md[/] for more help.")
                    .Header("[yellow]💡 Quick Fix[/]")
                    .BorderColor(Color.Yellow));
            }
        }
    }

    private void DisplayAnalysisResults(AnalyticsSummaryDto summary)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[green]Analysis Complete![/]").LeftJustified());
        AnsiConsole.WriteLine();

        // Summary Table
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey);

        table.AddColumn(new TableColumn("[bold]Metric[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Value[/]").Centered());

        table.AddRow("Search Query", $"[yellow]{summary.Query}[/]");
        table.AddRow("Total Listings", $"[cyan]{summary.TotalListings}[/]");
        table.AddRow("Average Price", $"[green]{summary.PriceStatistics.Average:F2} {summary.PriceStatistics.CurrencyCode}[/]");
        table.AddRow("Median Price", $"{summary.PriceStatistics.Median:F2} {summary.PriceStatistics.CurrencyCode}");
        table.AddRow("Price Range", $"{summary.PriceStatistics.Min:F2} - {summary.PriceStatistics.Max:F2}");

        var competitionColor = summary.CompetitionLevel switch
        {
            Core.ValueObjects.CompetitionLevel.Low => "green",
            Core.ValueObjects.CompetitionLevel.Medium => "yellow",
            Core.ValueObjects.CompetitionLevel.High => "red",
            _ => "white"
        };
        table.AddRow("Competition", $"[{competitionColor}]{summary.CompetitionLevel.ToDisplayString()}[/]");

        var scoreColor = summary.NicheScore.Value switch
        {
            >= 8.0m => "green",
            >= 6.0m => "yellow",
            >= 4.0m => "orange1",
            _ => "red"
        };
        table.AddRow("[bold]Niche Score[/]", $"[bold {scoreColor}]{summary.NicheScore}[/]");

        AnsiConsole.Write(table);

        // Top Keywords
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Top 5 Keywords:[/]");
        foreach (var keyword in summary.TopKeywords.Take(5))
        {
            var bar = new string('█', (int)(keyword.Percentage * 2));
            AnsiConsole.MarkupLine($"  [cyan]{keyword.Keyword,-20}[/] {bar} [dim]{keyword.Frequency} ({keyword.Percentage:F1}%)[/]");
        }

        // Top Shops
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Top 3 Competitors:[/]");
        foreach (var shop in summary.TopShops.Take(3))
        {
            AnsiConsole.MarkupLine($"  [yellow]• {shop.ShopName}[/] - {shop.ListingsInSearch} listings in search");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[green]✓ Excel report generated in ./Reports/[/]");
    }

    private void ShowStatistics()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Database Statistics[/]").LeftJustified());
        AnsiConsole.WriteLine();

        var chart = new BreakdownChart()
            .Width(60)
            .AddItem("Listings", 150, Color.Blue)
            .AddItem("Shops", 45, Color.Green)
            .AddItem("Queries", 12, Color.Yellow);

        AnsiConsole.Write(chart);
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[dim]Statistics feature coming soon...[/]");
    }

    private void ShowSettings()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Settings[/]").LeftJustified());
        AnsiConsole.WriteLine();

        var apiKey = _configuration["Etsy:ApiKey"];
        var apiUrl = _configuration["Etsy:ApiUrl"];
        var dbPath = _configuration["Database:ConnectionString"];
        var reportsDir = _configuration["Reports:OutputDirectory"];

        var table = new Table()
            .Border(TableBorder.Rounded);

        table.AddColumn("Setting");
        table.AddColumn("Value");

        table.AddRow("Etsy API URL", apiUrl ?? "Not set");
        table.AddRow("API Key", string.IsNullOrEmpty(apiKey) || apiKey.Contains("YOUR") ? "[red]Not configured[/]" : "[green]Configured[/]");
        table.AddRow("Database", dbPath ?? "Not set");
        table.AddRow("Reports Directory", reportsDir ?? "Not set");

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[dim]Edit appsettings.json to change settings[/]");
    }

    private void ShowGoodbye()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText("Goodbye!")
                .LeftJustified()
                .Color(Color.Yellow));

        AnsiConsole.MarkupLine("[dim]Thank you for using EtsyAnalyzer![/]");
        AnsiConsole.WriteLine();
    }
}
