# Technical Architecture Document

## 🏗️ System Overview

EtsyAnalyzer is a professional market analysis tool built on **.NET 10** using **Clean Architecture** principles. The system analyzes Etsy marketplace data to help sellers identify profitable niches through comprehensive data-driven insights.

---

## 📐 Architecture Layers

### 1. Core Layer (`EtsyAnalyzer.Core`)
**Purpose**: Domain entities, business rules, and abstractions

**Components**:
- **Entities**: `Listing`, `Shop`, `SearchQuery`, `AnalysisResult`
- **Value Objects**: `CompetitionLevel`, `NicheScore`, `PriceRange`
- **DTOs**: Data transfer objects for cross-layer communication
- **Interfaces**: Abstractions for data sources, repositories, services
- **Exceptions**: Domain-specific exception hierarchy

**Dependencies**: None (pure domain layer)

**Key Design Decisions**:
- Value objects are immutable records
- Entities use EF Core navigation properties
- DTOs separate persistence from presentation
- Interfaces enable dependency inversion

---

### 2. Analytics Layer (`EtsyAnalyzer.Analytics`)
**Purpose**: Business logic and analysis algorithms

**Components**:
- **PriceAnalyzer**: Statistical price analysis (avg, median, quartiles, ranges)
- **KeywordAnalyzer**: Frequency analysis, tag extraction, top keywords
- **CompetitionAnalyzer**: Listing distribution, shop concentration
- **NicheScoreCalculator**: Multi-factor weighted scoring (1-10)
- **AnalyticsService**: Orchestration and workflow coordination

**Dependencies**: Core

**Algorithms**:
```
Niche Score = (Price Score × 0.3) + (Competition Score × 0.4) + (Diversity Score × 0.3)

Competition Level:
- Low: < 50 listings
- Medium: 50-200 listings  
- High: > 200 listings

Price Score:
- Based on average price range
- $30-100 = optimal (score 8-10)
- < $10 or > $200 = suboptimal (score 3-5)
```

---

### 3. Data Integration Layer (`EtsyAnalyzer.EtsyApi`)
**Purpose**: External API integration

**Components**:
- **EtsyApiClient**: HTTP client wrapper for Etsy API v3
- **Response Models**: API-specific DTOs
- **Mappers**: Convert API responses to Core DTOs
- **EtsyDataSource**: IDataSource implementation
- **Configuration**: API key, URL, timeout settings

**Dependencies**: Core, System.Net.Http

**API Integration**:
- Endpoint: `https://openapi.etsy.com/v3`
- Authentication: API key header (`x-api-key`)
- Rate Limit: 10 requests/second
- Max Results: 200 listings per query

---

### 4. Infrastructure Layer (`EtsyAnalyzer.Infrastructure`)
**Purpose**: Persistence, reporting, cross-cutting concerns

#### 4.1 Persistence
- **DbContext**: Entity Framework Core with SQLite
- **Repositories**: Generic + specialized repositories
- **UnitOfWork**: Transaction coordination
- **Configurations**: Fluent API entity mappings

**Database Schema**:
```sql
Listings (Id, ListingId*, Title, Price, Currency, ShopId, SearchQueryId, ...)
Shops (Id, ShopId*, ShopName, ListingCount, Rating, ...)
SearchQueries (Id, Query, ExecutedAt, ResultCount, DataSource)
AnalysisResults (Id, SearchQueryId, NicheScore, Competition, ...)

* = Unique index
```

#### 4.2 Reporting
- **ExcelReportGenerator**: ClosedXML-based XLSX generation
- **Sheet Formatters**: 
  - `SummarySheetFormatter`: Overview metrics
  - `ListingsSheetFormatter`: Detailed product data
  - `ShopsSheetFormatter`: Competitor ranking
  - `KeywordsSheetFormatter`: Keyword frequency

**Report Structure**:
```
Worksheet 1: Summary (KPIs, charts, highlights)
Worksheet 2: Listings (200 rows × 15 columns)
Worksheet 3: Shops (top competitors ranked)
Worksheet 4: Keywords (frequency analysis)
```

**Dependencies**: Core, Entity Framework Core, ClosedXML

---

### 5. Presentation Layer (`EtsyAnalyzer.ConsoleApp`)
**Purpose**: User interface and application entry point

**Components**:
- **Program.cs**: Host bootstrap, DI configuration
- **ConsoleUI**: Spectre.Console-based interactive menu
- **DependencyInjection**: Service registration
- **Configuration**: appsettings.json management

**User Flow**:
```
Start → Main Menu → Analyze Niche
				  ↓
			Enter Query → API Fetch → DB Save
				  ↓
			Analysis → Excel Report → Display Results
				  ↓
			Return to Menu → View Stats / Settings / Exit
```

**Dependencies**: All other layers, Spectre.Console, Serilog

---

## 🔄 Data Flow

```
User Input (query)
	↓
ConsoleUI
	↓
AnalyticsService.AnalyzeNicheAsync()
	↓
IDataSource.SearchListingsAsync() → Etsy API v3
	↓
UnitOfWork.SaveChanges() → SQLite Database
	↓
PriceAnalyzer + KeywordAnalyzer + CompetitionAnalyzer
	↓
NicheScoreCalculator
	↓
AnalyticsSummaryDto
	↓
ExcelReportGenerator.GenerateReportAsync()
	↓
XLSX File Output + Console Display
```

---

## 🛠️ Technology Stack

### Frameworks & Libraries
| Layer | Technology | Version | Purpose |
|-------|-----------|---------|---------|
| Core | .NET | 10.0 | Runtime |
| Data | Entity Framework Core | 10.x | ORM |
| Database | SQLite | 3.x | Persistence |
| HTTP | HttpClient | Built-in | API calls |
| Logging | Serilog | 4.x | Structured logging |
| Config | Microsoft.Extensions.Configuration | 10.x | Settings |
| DI | Microsoft.Extensions.DependencyInjection | 10.x | IoC container |
| Reporting | ClosedXML | 0.104.x | Excel generation |
| UI | Spectre.Console | 0.57.x | Terminal UI |

### Design Patterns
- **Clean Architecture**: Dependency rule enforcement
- **Repository Pattern**: Data access abstraction
- **Unit of Work**: Transaction boundary
- **Strategy Pattern**: IDataSource for multiple providers
- **Dependency Injection**: Loose coupling throughout
- **DTO Pattern**: Layer isolation
- **Factory Pattern**: Service creation in DI

---

## 🔐 Security Considerations

### API Key Management
- ✅ Stored in appsettings.json (gitignored)
- ✅ Never hardcoded in source
- ✅ Environment variable override support
- ⚠️ **Production**: Use Azure Key Vault or similar

### Data Privacy
- ✅ Local SQLite database (no cloud by default)
- ✅ No personal user data stored
- ✅ Public marketplace data only

### Known Vulnerabilities
- ⚠️ SQLitePCLRaw.lib.e_sqlite3 2.1.11 (NU1903)
  - Impact: Low for local development
  - Mitigation: Monitor for package updates

---

## 📊 Performance Characteristics

### Benchmarks (Typical Workflow)
| Operation | Time | Notes |
|-----------|------|-------|
| API Fetch (200 listings) | ~2s | Network dependent |
| Database Save | ~0.3s | SQLite insert |
| Analysis (all calculators) | ~0.5s | In-memory processing |
| Excel Generation | ~0.8s | ClosedXML rendering |
| **Total Workflow** | **~4s** | Query → Report |

### Scalability
- **Current**: Single-threaded, synchronous analysis
- **Bottleneck**: Etsy API rate limit (10 req/s)
- **Future**: Batch processing, caching, async pipelines

### Resource Usage
- **Memory**: ~50MB typical, ~150MB peak (Excel generation)
- **Disk**: ~10KB per analysis (database)
- **Network**: ~500KB per query (API response)

---

## 🧪 Testing Strategy (Recommended)

### Unit Tests
```csharp
// Example
[Fact]
public void NicheScore_HighCompetition_ReturnsLowScore()
{
	// Arrange
	var listings = GenerateListings(count: 500);
	var calculator = new NicheScoreCalculator();

	// Act
	var score = calculator.Calculate(listings);

	// Assert
	Assert.InRange(score.Value, 1.0m, 4.0m);
}
```

### Integration Tests
- EF Core migrations and queries
- Etsy API client (use test API key)
- Full analysis workflow

### Test Coverage Goals
- Core domain logic: 90%+
- Analytics algorithms: 85%+
- Data access: 70%+
- API integration: 60%+ (external dependency)

---

## 🚀 Deployment Options

### Development
```bash
dotnet run --project src/EtsyAnalyzer.ConsoleApp
```

### Production (Console)
```bash
dotnet publish -c Release -r win-x64 --self-contained
./bin/Release/net10.0/win-x64/publish/EtsyAnalyzer.ConsoleApp.exe
```

### Docker (Future)
```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:10.0
COPY ./publish /app
WORKDIR /app
ENTRYPOINT ["./EtsyAnalyzer.ConsoleApp"]
```

### Cloud Migration (Azure)
- **App Service**: Host Web UI version
- **Functions**: Scheduled niche tracking
- **SQL Database**: Replace SQLite for scale
- **Key Vault**: Secure API key storage
- **Application Insights**: Telemetry

---

## 📝 Configuration

### appsettings.json Structure
```json
{
  "Etsy": {
	"ApiKey": "...",
	"ApiUrl": "https://openapi.etsy.com/v3",
	"RequestTimeout": "00:00:30",
	"MaxRetries": 3
  },
  "Database": {
	"ConnectionString": "Data Source=etsyanalyzer.db"
  },
  "Reports": {
	"OutputDirectory": "./Reports",
	"FileNameFormat": "EtsyAnalysis_{query}_{date}.xlsx"
  },
  "Serilog": {
	"MinimumLevel": "Information",
	"WriteTo": [
	  { "Name": "Console" },
	  { "Name": "File", "Args": { "path": "logs/log-.txt" } }
	]
  }
}
```

### Environment Variables
```bash
ETSY_API_KEY=your_key_here
DATABASE_PATH=/data/etsyanalyzer.db
REPORTS_DIR=/output/reports
```

---

## 🔄 Extension Points

### Adding New Data Sources
Implement `IDataSource` interface:
```csharp
public interface IDataSource
{
	string SourceName { get; }
	Task<IEnumerable<ListingDto>> SearchListingsAsync(string query, int limit);
	Task<ShopDto?> GetShopDetailsAsync(string shopId);
}
```

### Adding New Analyzers
Create analyzer service and call from `AnalyticsService`:
```csharp
public interface ICustomAnalyzer
{
	Task<CustomDto> AnalyzeAsync(IEnumerable<ListingDto> listings);
}
```

### Custom Report Formats
Implement `IReportGenerator`:
```csharp
public interface IReportGenerator
{
	Task GenerateReportAsync(
		AnalyticsSummaryDto summary,
		IEnumerable<ListingDto> listings,
		string outputPath);
}
```

---

## 📚 References

- [Etsy API v3 Documentation](https://developer.etsy.com/documentation/reference)
- [Clean Architecture (Robert C. Martin)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Entity Framework Core Docs](https://docs.microsoft.com/ef/core)
- [ClosedXML Documentation](https://closedxml.readthedocs.io/)
- [Spectre.Console Guide](https://spectreconsole.net/)

---

**Document Version**: 1.0  
**Last Updated**: June 27, 2026  
**Maintained By**: Development Team
