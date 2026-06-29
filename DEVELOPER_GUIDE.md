# Developer Guide - Extension Examples

## 📚 Table of Contents
1. [Adding a New Data Source](#adding-a-new-data-source)
2. [Creating Custom Analyzers](#creating-custom-analyzers)
3. [Customizing Excel Reports](#customizing-excel-reports)
4. [Adding New Domain Entities](#adding-new-domain-entities)
5. [Implementing Background Jobs](#implementing-background-jobs)

---

## 1. Adding a New Data Source

Example: Adding Amazon Handmade integration

### Step 1: Create API Client Project

```bash
dotnet new classlib -n EtsyAnalyzer.AmazonApi -f net10.0
dotnet sln add src/EtsyAnalyzer.AmazonApi/EtsyAnalyzer.AmazonApi.csproj
```

### Step 2: Add Project Reference to Core

```xml
<ItemGroup>
  <ProjectReference Include="..\EtsyAnalyzer.Core\EtsyAnalyzer.Core.csproj" />
</ItemGroup>
```

### Step 3: Implement IDataSource

```csharp
// src/EtsyAnalyzer.AmazonApi/AmazonDataSource.cs
using EtsyAnalyzer.Core.DTOs;
using EtsyAnalyzer.Core.Interfaces;

namespace EtsyAnalyzer.AmazonApi;

public class AmazonDataSource : IDataSource
{
	private readonly AmazonApiClient _client;

	public string SourceName => "Amazon Handmade";

	public AmazonDataSource(AmazonApiClient client)
	{
		_client = client;
	}

	public async Task<IEnumerable<ListingDto>> SearchListingsAsync(
		string query, 
		int limit = 100)
	{
		var results = await _client.SearchProductsAsync(query, limit);
		return results.Select(MapToListingDto);
	}

	public async Task<ShopDto?> GetShopDetailsAsync(string shopId)
	{
		var seller = await _client.GetSellerAsync(shopId);
		return MapToShopDto(seller);
	}

	private ListingDto MapToListingDto(AmazonProduct product)
	{
		return new ListingDto
		{
			ListingId = product.ASIN,
			Title = product.Title,
			Price = product.Price,
			CurrencyCode = "USD",
			Description = product.Description,
			// ... map other fields
		};
	}
}
```

### Step 4: Register in DI

```csharp
// src/EtsyAnalyzer.ConsoleApp/Configuration/DependencyInjection.cs

public static IServiceCollection AddEtsyAnalyzerServices(
	this IServiceCollection services,
	IConfiguration configuration)
{
	// ... existing registrations

	// Register Amazon data source
	services.Configure<AmazonApiConfiguration>(
		configuration.GetSection("Amazon"));
	services.AddHttpClient<AmazonApiClient>();
	services.AddScoped<IDataSource, AmazonDataSource>();

	return services;
}
```

### Step 5: Update appsettings.json

```json
{
  "Amazon": {
	"AccessKey": "your_access_key",
	"SecretKey": "your_secret_key",
	"AssociateTag": "your_tag",
	"Region": "us-east-1"
  }
}
```

### Step 6: Use Multiple Data Sources

```csharp
// In console UI or service
public class MultiSourceAnalyzer
{
	private readonly IEnumerable<IDataSource> _dataSources;

	public MultiSourceAnalyzer(IEnumerable<IDataSource> dataSources)
	{
		_dataSources = dataSources;
	}

	public async Task<Dictionary<string, AnalyticsSummaryDto>> AnalyzeAllSourcesAsync(string query)
	{
		var results = new Dictionary<string, AnalyticsSummaryDto>();

		foreach (var source in _dataSources)
		{
			var listings = await source.SearchListingsAsync(query);
			var summary = await _analyticsService.AnalyzeDataAsync(listings, query);
			results[source.SourceName] = summary;
		}

		return results;
	}
}
```

---

## 2. Creating Custom Analyzers

Example: Adding a Photo Quality Analyzer

### Step 1: Create Analyzer Interface

```csharp
// src/EtsyAnalyzer.Core/Interfaces/IPhotoQualityAnalyzer.cs
namespace EtsyAnalyzer.Core.Interfaces;

public interface IPhotoQualityAnalyzer
{
	Task<PhotoQualityDto> AnalyzeImageAsync(string imageUrl);
	Task<PhotoStatisticsDto> AnalyzeListingPhotosAsync(IEnumerable<ListingDto> listings);
}
```

### Step 2: Create DTO

```csharp
// src/EtsyAnalyzer.Core/DTOs/PhotoQualityDto.cs
namespace EtsyAnalyzer.Core.DTOs;

public record PhotoQualityDto
{
	public string ImageUrl { get; init; } = string.Empty;
	public int Width { get; init; }
	public int Height { get; init; }
	public double QualityScore { get; init; } // 0-10
	public bool HasWatermark { get; init; }
	public string DominantColor { get; init; } = string.Empty;
}

public record PhotoStatisticsDto
{
	public double AverageQuality { get; init; }
	public int HighQualityCount { get; init; }
	public int LowQualityCount { get; init; }
	public string MostCommonColor { get; init; } = string.Empty;
}
```

### Step 3: Implement Analyzer

```csharp
// src/EtsyAnalyzer.Analytics/Analyzers/PhotoQualityAnalyzer.cs
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace EtsyAnalyzer.Analytics.Analyzers;

public class PhotoQualityAnalyzer : IPhotoQualityAnalyzer
{
	private readonly HttpClient _httpClient;

	public PhotoQualityAnalyzer(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<PhotoQualityDto> AnalyzeImageAsync(string imageUrl)
	{
		var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);

		using var image = Image.Load<Rgba32>(imageBytes);

		var quality = CalculateQualityScore(image);
		var dominantColor = GetDominantColor(image);

		return new PhotoQualityDto
		{
			ImageUrl = imageUrl,
			Width = image.Width,
			Height = image.Height,
			QualityScore = quality,
			HasWatermark = DetectWatermark(image),
			DominantColor = dominantColor
		};
	}

	private double CalculateQualityScore(Image<Rgba32> image)
	{
		// Resolution score
		var resolutionScore = CalculateResolutionScore(image.Width, image.Height);

		// Sharpness score (simplified)
		var sharpnessScore = CalculateSharpness(image);

		// Weighted average
		return (resolutionScore * 0.6) + (sharpnessScore * 0.4);
	}

	private double CalculateResolutionScore(int width, int height)
	{
		var pixels = width * height;

		if (pixels >= 2000 * 2000) return 10.0;
		if (pixels >= 1500 * 1500) return 8.0;
		if (pixels >= 1000 * 1000) return 6.0;
		if (pixels >= 800 * 800) return 4.0;
		return 2.0;
	}
}
```

### Step 4: Integrate into AnalyticsService

```csharp
// Update src/EtsyAnalyzer.Analytics/Services/AnalyticsService.cs

public class AnalyticsService : IAnalyticsService
{
	private readonly IPhotoQualityAnalyzer _photoAnalyzer;

	// ... constructor

	private async Task<AnalyticsSummaryDto> PerformAnalysis(
		IEnumerable<ListingDto> listings,
		string query)
	{
		// ... existing analysis

		// Add photo analysis
		var photoStats = await _photoAnalyzer.AnalyzeListingPhotosAsync(listings);

		return new AnalyticsSummaryDto
		{
			// ... existing properties
			PhotoStatistics = photoStats
		};
	}
}
```

---

## 3. Customizing Excel Reports

Example: Adding a Trends Worksheet

### Step 1: Create Sheet Formatter

```csharp
// src/EtsyAnalyzer.Infrastructure/Reporting/SheetFormatters/TrendsSheetFormatter.cs
using ClosedXML.Excel;

namespace EtsyAnalyzer.Infrastructure.Reporting.SheetFormatters;

public class TrendsSheetFormatter
{
	public void Format(IXLWorksheet worksheet, TrendsDto trends)
	{
		worksheet.Cell(1, 1).Value = "Trend Analysis";
		worksheet.Range(1, 1, 1, 4).Merge();
		FormatHeader(worksheet.Range(1, 1, 1, 4));

		// Headers
		var headers = new[] { "Period", "Avg Price", "Listings Count", "Competition" };
		for (int i = 0; i < headers.Length; i++)
		{
			var cell = worksheet.Cell(3, i + 1);
			cell.Value = headers[i];
			cell.Style.Font.Bold = true;
			cell.Style.Fill.BackgroundColor = XLColor.LightGray;
		}

		// Data rows
		int row = 4;
		foreach (var period in trends.Periods.OrderBy(p => p.Date))
		{
			worksheet.Cell(row, 1).Value = period.Date.ToString("MMM yyyy");
			worksheet.Cell(row, 2).Value = period.AveragePrice;
			worksheet.Cell(row, 2).Style.NumberFormat.Format = "$#,##0.00";
			worksheet.Cell(row, 3).Value = period.ListingCount;
			worksheet.Cell(row, 4).Value = period.CompetitionLevel.ToString();

			row++;
		}

		// Add chart
		var chart = worksheet.Workbook.Worksheets.First()
			.AddChart("Price Trend");
		chart.SetChartType(XLChartType.Line);

		var dataRange = worksheet.Range(4, 1, row - 1, 2);
		chart.AddSerie(dataRange);

		worksheet.Columns().AdjustToContents();
	}
}
```

### Step 2: Update Report Generator

```csharp
// src/EtsyAnalyzer.Infrastructure/Reporting/ExcelReportGenerator.cs

public async Task GenerateReportAsync(
	AnalyticsSummaryDto summary,
	IEnumerable<ListingDto> listings,
	string outputPath)
{
	// ... existing code

	// Add trends sheet
	if (summary.TrendsData != null)
	{
		var trendsSheet = workbook.Worksheets.Add("Trends");
		_trendsFormatter.Format(trendsSheet, summary.TrendsData);
	}

	workbook.SaveAs(outputPath);
}
```

---

## 4. Adding New Domain Entities

Example: Adding Review tracking

### Step 1: Create Entity

```csharp
// src/EtsyAnalyzer.Core/Entities/Review.cs
namespace EtsyAnalyzer.Core.Entities;

public class Review
{
	public int Id { get; set; }
	public string ReviewId { get; set; } = string.Empty;
	public string ListingId { get; set; } = string.Empty;
	public Listing Listing { get; set; } = null!;

	public int Rating { get; set; }
	public string ReviewText { get; set; } = string.Empty;
	public string ReviewerName { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }

	public string? Sentiment { get; set; } // Positive/Negative/Neutral
	public double? SentimentScore { get; set; }
}
```

### Step 2: Add DbSet

```csharp
// src/EtsyAnalyzer.Infrastructure/Persistence/EtsyAnalyzerDbContext.cs

public DbSet<Review> Reviews => Set<Review>();
```

### Step 3: Configure Entity

```csharp
// src/EtsyAnalyzer.Infrastructure/Persistence/Configurations/ReviewConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtsyAnalyzer.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
	public void Configure(EntityTypeBuilder<Review> builder)
	{
		builder.ToTable("Reviews");
		builder.HasKey(r => r.Id);

		builder.Property(r => r.ReviewId)
			.IsRequired()
			.HasMaxLength(100);

		builder.HasIndex(r => r.ReviewId)
			.IsUnique();

		builder.Property(r => r.ReviewText)
			.HasMaxLength(5000);

		builder.HasOne(r => r.Listing)
			.WithMany()
			.HasForeignKey(r => r.ListingId)
			.HasPrincipalKey(l => l.ListingId);
	}
}
```

### Step 4: Create Migration

```bash
cd src/EtsyAnalyzer.Infrastructure
dotnet ef migrations add AddReviews --startup-project ../EtsyAnalyzer.ConsoleApp
dotnet ef database update --startup-project ../EtsyAnalyzer.ConsoleApp
```

---

## 5. Implementing Background Jobs

Example: Scheduled niche tracking

### Step 1: Install Hangfire

```bash
dotnet add package Hangfire.Core
dotnet add package Hangfire.SQLite
```

### Step 2: Create Background Job

```csharp
// src/EtsyAnalyzer.Analytics/Jobs/NicheTrackingJob.cs
namespace EtsyAnalyzer.Analytics.Jobs;

public class NicheTrackingJob
{
	private readonly IAnalyticsService _analyticsService;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ILogger<NicheTrackingJob> _logger;

	public NicheTrackingJob(
		IAnalyticsService analyticsService,
		IUnitOfWork unitOfWork,
		ILogger<NicheTrackingJob> logger)
	{
		_analyticsService = analyticsService;
		_unitOfWork = unitOfWork;
		_logger = logger;
	}

	public async Task TrackSavedNichesAsync()
	{
		_logger.LogInformation("Starting niche tracking job");

		// Get saved queries to track
		var savedQueries = await _unitOfWork.SearchQueries
			.GetAllAsync();

		foreach (var query in savedQueries)
		{
			try
			{
				_logger.LogInformation("Tracking niche: {Query}", query.Query);

				var summary = await _analyticsService.AnalyzeNicheAsync(query.Query);

				// Save historical snapshot
				await SaveSnapshot(query.Query, summary);

				// Check for alerts (price changes, new competitors, etc.)
				await CheckAlertsAsync(query.Query, summary);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error tracking niche: {Query}", query.Query);
			}
		}

		_logger.LogInformation("Niche tracking job completed");
	}

	private async Task SaveSnapshot(string query, AnalyticsSummaryDto summary)
	{
		// Implementation
	}
}
```

### Step 3: Configure Hangfire

```csharp
// src/EtsyAnalyzer.ConsoleApp/Configuration/DependencyInjection.cs

public static IServiceCollection AddEtsyAnalyzerServices(
	this IServiceCollection services,
	IConfiguration configuration)
{
	// ... existing code

	// Add Hangfire
	services.AddHangfire(config => config
		.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
		.UseSimpleAssemblyNameTypeSerializer()
		.UseRecommendedSerializerSettings()
		.UseSQLiteStorage(configuration.GetConnectionString("HangfireConnection")));

	services.AddHangfireServer();

	// Register jobs
	services.AddScoped<NicheTrackingJob>();

	return services;
}
```

### Step 4: Schedule Jobs

```csharp
// src/EtsyAnalyzer.ConsoleApp/Program.cs

// After building host
using (var scope = host.Services.CreateScope())
{
	var recurringJobManager = scope.ServiceProvider
		.GetRequiredService<IRecurringJobManager>();

	// Track niches daily at 2 AM
	recurringJobManager.AddOrUpdate<NicheTrackingJob>(
		"niche-tracking",
		job => job.TrackSavedNichesAsync(),
		Cron.Daily(2));
}
```

---

## 🔧 Best Practices

### 1. Dependency Injection
Always register services with appropriate lifetime:
- **Transient**: Stateless services, lightweight objects
- **Scoped**: Per-request services, DbContext, UnitOfWork
- **Singleton**: Expensive objects, caches, configuration

### 2. Async/Await
Use async methods for I/O operations:
```csharp
// Good
public async Task<AnalyticsSummaryDto> AnalyzeAsync(string query)
{
	var data = await _dataSource.SearchListingsAsync(query);
	return await ProcessAsync(data);
}

// Bad - blocking
public AnalyticsSummaryDto Analyze(string query)
{
	var data = _dataSource.SearchListingsAsync(query).Result;
	return Process(data);
}
```

### 3. Error Handling
Use specific exceptions and proper logging:
```csharp
try
{
	return await _apiClient.FetchDataAsync();
}
catch (HttpRequestException ex)
{
	_logger.LogError(ex, "API request failed for query: {Query}", query);
	throw new DataSourceException("Failed to fetch data from API", ex);
}
```

### 4. Configuration
Never hardcode values:
```csharp
// Good - configuration
var apiUrl = _configuration["Etsy:ApiUrl"];

// Bad - hardcoded
var apiUrl = "https://openapi.etsy.com/v3";
```

---

**Happy coding! 🚀**
