# Project Structure & Dependencies

## 📦 Solution Overview

**EtsyAnalyzer** consists of 5 projects organized using Clean Architecture:

```
EtsyAnalyzer.slnx
├── src/
│   ├── EtsyAnalyzer.Core/              [Domain Layer]
│   ├── EtsyAnalyzer.Analytics/         [Business Logic]
│   ├── EtsyAnalyzer.EtsyApi/           [External Integration]
│   ├── EtsyAnalyzer.Infrastructure/    [Data & Cross-cutting]
│   └── EtsyAnalyzer.ConsoleApp/        [Presentation]
```

---

## 1️⃣ EtsyAnalyzer.Core

**Type**: Class Library  
**Target Framework**: net10.0  
**Purpose**: Domain entities, business rules, abstractions

### 📁 Folder Structure
```
EtsyAnalyzer.Core/
├── Entities/
│   ├── Listing.cs
│   ├── Shop.cs
│   ├── SearchQuery.cs
│   └── AnalysisResult.cs
├── ValueObjects/
│   ├── CompetitionLevel.cs
│   ├── NicheScore.cs
│   └── PriceRange.cs
├── DTOs/
│   ├── ListingDto.cs
│   ├── ShopDto.cs
│   ├── AnalyticsSummaryDto.cs
│   ├── PriceStatisticsDto.cs
│   ├── KeywordFrequencyDto.cs
│   └── ShopStatisticsDto.cs
├── Interfaces/
│   ├── IDataSource.cs
│   ├── IRepository.cs
│   ├── IUnitOfWork.cs
│   ├── IAnalyticsService.cs
│   └── IReportGenerator.cs
└── Exceptions/
	├── EtsyAnalyzerException.cs
	├── DataSourceException.cs
	└── AnalysisException.cs
```

### 📦 NuGet Dependencies
None (pure domain layer)

### 🔗 Project Dependencies
None

---

## 2️⃣ EtsyAnalyzer.Analytics

**Type**: Class Library  
**Target Framework**: net10.0  
**Purpose**: Business logic and analysis algorithms

### 📁 Folder Structure
```
EtsyAnalyzer.Analytics/
├── Analyzers/
│   ├── PriceAnalyzer.cs
│   ├── KeywordAnalyzer.cs
│   ├── CompetitionAnalyzer.cs
│   └── NicheScoreCalculator.cs
└── Services/
	└── AnalyticsService.cs
```

### 📦 NuGet Dependencies
```xml
<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="10.0.0" />
```

### 🔗 Project Dependencies
```xml
<ProjectReference Include="..\EtsyAnalyzer.Core\EtsyAnalyzer.Core.csproj" />
```

### 🎯 Key Classes

#### PriceAnalyzer
- Calculates avg, median, min, max, quartiles
- Price distribution ranges
- Currency code handling

#### KeywordAnalyzer
- Word frequency analysis
- Tag extraction
- Top keyword identification

#### CompetitionAnalyzer
- Listing count categorization
- Shop distribution analysis
- Market concentration metrics

#### NicheScoreCalculator
- Multi-factor weighted scoring (1-10)
- Price score (30%), Competition (40%), Diversity (30%)

---

## 3️⃣ EtsyAnalyzer.EtsyApi

**Type**: Class Library  
**Target Framework**: net10.0  
**Purpose**: Etsy API v3 integration

### 📁 Folder Structure
```
EtsyAnalyzer.EtsyApi/
├── Client/
│   └── EtsyApiClient.cs
├── Models/
│   ├── EtsySearchResponse.cs
│   ├── EtsyListing.cs
│   ├── EtsyShop.cs
│   └── EtsyImage.cs
├── Mappers/
│   ├── ListingMapper.cs
│   └── ShopMapper.cs
├── Configuration/
│   └── EtsyApiConfiguration.cs
└── EtsyDataSource.cs
```

### 📦 NuGet Dependencies
```xml
<PackageReference Include="Microsoft.Extensions.Http" Version="10.0.0" />
<PackageReference Include="Microsoft.Extensions.Options" Version="10.0.0" />
<PackageReference Include="System.Text.Json" Version="10.0.0" />
```

### 🔗 Project Dependencies
```xml
<ProjectReference Include="..\EtsyAnalyzer.Core\EtsyAnalyzer.Core.csproj" />
```

### 🌐 API Details
- Base URL: `https://openapi.etsy.com/v3`
- Authentication: Header `x-api-key`
- Rate Limit: 10 requests/second
- Max Results: 200 per query

---

## 4️⃣ EtsyAnalyzer.Infrastructure

**Type**: Class Library  
**Target Framework**: net10.0  
**Purpose**: Database, reporting, cross-cutting concerns

### 📁 Folder Structure
```
EtsyAnalyzer.Infrastructure/
├── Persistence/
│   ├── EtsyAnalyzerDbContext.cs
│   ├── Configurations/
│   │   ├── ListingConfiguration.cs
│   │   ├── ShopConfiguration.cs
│   │   ├── SearchQueryConfiguration.cs
│   │   └── AnalysisResultConfiguration.cs
│   ├── Repositories/
│   │   ├── ListingRepository.cs
│   │   ├── ShopRepository.cs
│   │   └── SearchQueryRepository.cs
│   └── UnitOfWork.cs
└── Reporting/
	├── ExcelReportGenerator.cs
	└── SheetFormatters/
		├── SummarySheetFormatter.cs
		├── ListingsSheetFormatter.cs
		├── ShopsSheetFormatter.cs
		└── KeywordsSheetFormatter.cs
```

### 📦 NuGet Dependencies
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.0">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
<PackageReference Include="ClosedXML" Version="0.104.2" />
```

### 🔗 Project Dependencies
```xml
<ProjectReference Include="..\EtsyAnalyzer.Core\EtsyAnalyzer.Core.csproj" />
```

### 🗄️ Database
- **Provider**: SQLite
- **File**: `etsyanalyzer.db`
- **Tables**: Listings, Shops, SearchQueries, AnalysisResults

---

## 5️⃣ EtsyAnalyzer.ConsoleApp

**Type**: Console Application  
**Target Framework**: net10.0  
**Purpose**: User interface and application entry point

### 📁 Folder Structure
```
EtsyAnalyzer.ConsoleApp/
├── Program.cs
├── Configuration/
│   └── DependencyInjection.cs
├── UI/
│   └── ConsoleUI.cs
├── appsettings.json
└── appsettings.Development.json
```

### 📦 NuGet Dependencies
```xml
<PackageReference Include="Microsoft.Extensions.Hosting" Version="10.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration" Version="10.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="10.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration.EnvironmentVariables" Version="10.0.0" />
<PackageReference Include="Serilog" Version="4.2.0" />
<PackageReference Include="Serilog.Extensions.Hosting" Version="10.0.0" />
<PackageReference Include="Serilog.Sinks.Console" Version="6.0.0" />
<PackageReference Include="Serilog.Sinks.File" Version="6.0.0" />
<PackageReference Include="Serilog.Settings.Configuration" Version="8.0.4" />
<PackageReference Include="Spectre.Console" Version="0.57.1" />
```

### 🔗 Project Dependencies
```xml
<ProjectReference Include="..\EtsyAnalyzer.Core\EtsyAnalyzer.Core.csproj" />
<ProjectReference Include="..\EtsyAnalyzer.Analytics\EtsyAnalyzer.Analytics.csproj" />
<ProjectReference Include="..\EtsyAnalyzer.EtsyApi\EtsyAnalyzer.EtsyApi.csproj" />
<ProjectReference Include="..\EtsyAnalyzer.Infrastructure\EtsyAnalyzer.Infrastructure.csproj" />
```

### ⚙️ Configuration Files
- `appsettings.json`: Production settings
- `appsettings.Development.json`: Dev overrides
- Supports environment variables

---

## 🔄 Dependency Graph

```
┌─────────────────────────┐
│  EtsyAnalyzer.ConsoleApp│ (Entry Point)
└───────────┬─────────────┘
			│
	┌───────┼───────┬─────────────┐
	│       │       │             │
	▼       ▼       ▼             ▼
┌────────┐┌───────┐┌──────────┐┌────────────┐
│Analytics││EtsyApi││Infrastructure││  Core     │
└────┬───┘└───┬───┘└─────┬────┘└────────────┘
	 │        │          │
	 └────────┼──────────┘
			  │
			  ▼
		┌──────────┐
		│   Core   │ (No dependencies)
		└──────────┘
```

**Dependency Rule**: Dependencies point inward  
- ConsoleApp → All layers  
- Analytics, EtsyApi, Infrastructure → Core only  
- Core → Nothing

---

## 📊 Lines of Code (Approximate)

| Project | Files | LOC | Purpose |
|---------|-------|-----|---------|
| Core | 18 | ~800 | Domain & abstractions |
| Analytics | 5 | ~600 | Business logic |
| EtsyApi | 7 | ~400 | API integration |
| Infrastructure | 12 | ~1200 | Data & reporting |
| ConsoleApp | 3 | ~400 | UI & bootstrap |
| **Total** | **45** | **~3400** | Full solution |

---

## 🔧 Build & Run Commands

### Build Solution
```bash
dotnet build EtsyAnalyzer.slnx
```

### Build Specific Project
```bash
dotnet build src/EtsyAnalyzer.Core
```

### Run Tests (when added)
```bash
dotnet test
```

### Run Application
```bash
cd src/EtsyAnalyzer.ConsoleApp
dotnet run
```

### Publish for Production
```bash
dotnet publish src/EtsyAnalyzer.ConsoleApp -c Release -r win-x64 --self-contained
```

---

## 📦 Package Versions Summary

### Microsoft Packages
- .NET Runtime: **10.0**
- Entity Framework Core: **10.0.0**
- Extensions.* (Hosting, DI, Config): **10.0.0**
- Serilog.Extensions.Hosting: **10.0.0**

### Third-Party Packages
- Serilog: **4.2.0**
- Serilog.Sinks.Console: **6.0.0**
- Serilog.Sinks.File: **6.0.0**
- Serilog.Settings.Configuration: **8.0.4**
- ClosedXML: **0.104.2**
- Spectre.Console: **0.57.1**

### Known Issues
⚠️ **SQLitePCLRaw.lib.e_sqlite3** 2.1.11 has vulnerability NU1903  
- Impact: Low (local dev)
- Workaround: Monitor for updates

---

## 🚀 Quick Start Checklist

- [ ] Install .NET 10 SDK
- [ ] Clone repository
- [ ] Get Etsy API key
- [ ] Edit `appsettings.json` with API key
- [ ] Run `dotnet restore`
- [ ] Run `dotnet build`
- [ ] Run `dotnet run --project src/EtsyAnalyzer.ConsoleApp`
- [ ] Enter search query and analyze!

---

## 📚 Further Reading

- [README.md](README.md) - Project overview
- [QUICKSTART.md](QUICKSTART.md) - User guide
- [ARCHITECTURE.md](ARCHITECTURE.md) - Technical details
- [DEVELOPER_GUIDE.md](DEVELOPER_GUIDE.md) - Extension examples
- [PROJECT_STATUS.md](PROJECT_STATUS.md) - Current status & roadmap

---

**Last Updated**: June 27, 2026  
**Solution Version**: 1.0.0  
**Target Framework**: .NET 10
