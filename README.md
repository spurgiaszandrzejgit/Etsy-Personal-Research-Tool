# EtsyAnalyzer

Professional Etsy Market Analysis Tool for .NET 10

## 📋 Overview

EtsyAnalyzer is a powerful tool designed to help sellers find profitable niches on Etsy by analyzing market data through the official Etsy API v3. The application provides comprehensive insights including pricing statistics, competition analysis, keyword frequency, and an overall niche score.

## 🎯 Features

- **Market Analysis**: Analyze Etsy listings for any search query
- **Price Statistics**: Average, median, min/max prices with distribution ranges
- **Competition Analysis**: Automatically categorize niches as Low/Medium/High competition
- **Keyword Analysis**: Most frequent words in titles and popular tags
- **Shop Insights**: Identify top competitors in your niche
- **Niche Scoring**: AI-driven score (1-10) based on multiple factors
- **Excel Reports**: Beautiful, formatted reports with 4 worksheets (Summary, Listings, Shops, Keywords)
- **SQLite Database**: Store and reanalyze historical data

## 🏗️ Architecture

The project follows **Clean Architecture** principles with clear separation of concerns:

```
EtsyAnalyzer/
├── src/
│   ├── EtsyAnalyzer.Core/           # Domain layer (entities, interfaces, DTOs)
│   ├── EtsyAnalyzer.Analytics/       # Business logic (price, keyword, competition analysis)
│   ├── EtsyAnalyzer.EtsyApi/         # Etsy API v3 integration
│   ├── EtsyAnalyzer.Infrastructure/  # Database (SQLite + EF Core), Excel reporting
│   └── EtsyAnalyzer.ConsoleApp/      # Console application entry point
```

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Etsy API Key](https://www.etsy.com/developers/register) (free)
- Visual Studio 2026+ or VS Code

### Quick Start

1. **Get your Etsy API key** from [Etsy Developers Portal](https://www.etsy.com/developers/)

2. **Configure the application**

   Edit `src/EtsyAnalyzer.ConsoleApp/appsettings.json`:
   ```json
   {
     "Etsy": {
       "ApiKey": "your_actual_api_key_here"
     }
   }
   ```

3. **Run the application**
   ```bash
   cd src/EtsyAnalyzer.ConsoleApp
   dotnet run
   ```

4. **Start analyzing!**
   - Choose "Analyze New Niche"
   - Enter a search query (e.g., "handmade wood cutting board")
   - View results and Excel report in `Reports/` folder

📖 **For detailed usage guide**, see [QUICKSTART.md](QUICKSTART.md)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/EtsyAnalyzer.git
   cd EtsyAnalyzer
   ```

2. **Configure Etsy API Key**

   Edit `src/EtsyAnalyzer.ConsoleApp/appsettings.json`:
   ```json
   {
	 "Etsy": {
	   "ApiKey": "YOUR_ETSY_API_KEY_HERE"
	 }
   }
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run the application**
   ```bash
   cd src/EtsyAnalyzer.ConsoleApp
   dotnet run
   ```

## 📚 Documentation

- **[📋 Setup Checklist](SETUP_CHECKLIST.md)** - Complete installation & configuration guide
- **[🚀 Quick Start Guide](QUICKSTART.md)** - Get started in 5 minutes
- **[🔧 Troubleshooting](TROUBLESHOOTING.md)** - Fix common errors (404, API key issues)
- **[📊 Usage Examples](EXAMPLES.md)** - Sample queries and output interpretation
- **[🏗️ Architecture](ARCHITECTURE.md)** - Technical design & patterns
- **[📦 Projects Structure](PROJECTS.md)** - Solution layout & dependencies
- **[💻 Developer Guide](DEVELOPER_GUIDE.md)** - Extension & customization examples
- **[✅ Project Status](PROJECT_STATUS.md)** - Current features & roadmap

## 📖 How to Get Etsy API Key

1. Go to [Etsy Developers](https://www.etsy.com/developers/)
2. Sign in with your Etsy account
3. Click "Create a New App"
4. Fill in the app details
5. Copy the **Keystring** (this is your API key)
6. Paste it into `appsettings.json`

## 📊 Usage Example

```csharp
// Example: Analyze "wood wall clock" niche
var query = "wood wall clock";

// 1. Fetch data from Etsy API
var listings = await dataSource.SearchListingsAsync(query, 200);

// 2. Save to database
await unitOfWork.SaveChangesAsync();

// 3. Perform analysis
var summary = await analyticsService.AnalyzeNicheAsync(query);

// 4. Generate Excel report
var reportPath = await reportGenerator.GenerateReportAsync(
	summary, 
	listings, 
	"Reports/wood_wall_clock_analysis.xlsx"
);
```

## 📈 What Gets Analyzed

### Price Analysis
- Average, Median, Min, Max prices
- Price distribution across 5 ranges
- Currency-aware calculations

### Competition Analysis
- Total listings count
- Competition level (Low < 50, Medium < 200, High >= 200)
- Top 10 competing shops
- Shop dominance percentage

### Keyword Analysis
- Top 20 keywords from titles (with stop-word filtering)
- Top 20 most used tags
- Frequency and percentage for each

### Niche Score (1-10)
Multi-factor weighted scoring based on:
- Competition level
- Market size (optimal: 50-200 listings)
- Price range diversity
- Average price point
- Shop diversity (less dominance = better)

## 🗂️ Excel Report Structure

Generated reports contain 4 worksheets:

1. **Summary**: Key metrics, niche score, price statistics, competition level
2. **Listings**: All found products with clickable URLs
3. **Shops**: Top competitors ranked by listing count
4. **Keywords**: Most frequent words and tags with visual indicators

All sheets include:
- ✅ Conditional formatting (color-coded metrics)
- ✅ Auto-filtered headers
- ✅ Frozen top rows
- ✅ Auto-sized columns
- ✅ Clickable hyperlinks

## ⚙️ Configuration

### appsettings.json

```json
{
  "Etsy": {
	"ApiKey": "YOUR_API_KEY",
	"RateLimitPerMinute": 10,
	"TimeoutSeconds": 30
  },
  "Database": {
	"ConnectionString": "Data Source=etsyanalyzer.db"
  },
  "Analytics": {
	"CompetitionThresholds": {
	  "Low": 50,
	  "Medium": 200
	}
  },
  "Reports": {
	"OutputDirectory": "./Reports"
  }
}
```

## 🧪 Testing

```bash
dotnet test
```

## 📦 Technology Stack

### Core Technologies
- **.NET 10** - Modern runtime platform
- **C# 13** - Latest language features
- **Clean Architecture** - Maintainable design

### Key Libraries
- **Entity Framework Core** - Database ORM
- **SQLite** - Lightweight database
- **ClosedXML** - Excel report generation
- **Spectre.Console** - Rich terminal UI
- **Serilog** - Structured logging
- **HttpClient** - Etsy API integration

For detailed package list, see [PROJECTS.md](PROJECTS.md)

## 🔮 Future Enhancements

See [PROJECT_STATUS.md](PROJECT_STATUS.md) for detailed roadmap.

**Phase 2 highlights:**
- Web UI (Blazor)
- Trend analysis over time
- Photo quality analysis (AI)
- Review sentiment analysis
- Multi-marketplace support
- Background job scheduler

## 🧪 Testing

Currently no automated tests. Recommended test coverage:
- Unit tests for analytics algorithms
- Integration tests for database operations
- E2E tests for full workflow

See [ARCHITECTURE.md](ARCHITECTURE.md) for testing strategy.

## 📄 License

MIT License - See [LICENSE](LICENSE) file for details.

Free to use for personal and commercial purposes.

## 👨‍💻 Contributing

Contributions welcome! Please:
1. Read [DEVELOPER_GUIDE.md](DEVELOPER_GUIDE.md) for extension patterns
2. Follow existing code style and architecture
3. Add tests for new features
4. Update documentation as needed
5. Submit a pull request

## 💡 Support

- 📖 Check [documentation files](.) for detailed guides
- 🐛 Report bugs via GitHub Issues
- 💬 Ask questions on Stack Overflow (tag: `etsy-api` + `c#`)
- 🔍 Review [EXAMPLES.md](EXAMPLES.md) for common scenarios

---

**Built with ❤️ for Etsy sellers worldwide**

*Last updated: June 27, 2026*
