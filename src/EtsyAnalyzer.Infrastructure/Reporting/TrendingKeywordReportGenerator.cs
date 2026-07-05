using ClosedXML.Excel;
using EtsyAnalyzer.Core.DTOs;
using EtsyAnalyzer.Core.Interfaces;
using EtsyAnalyzer.Core.ValueObjects;
using EtsyAnalyzer.Infrastructure.Reporting.Utilities;
using Microsoft.Extensions.Configuration;

namespace EtsyAnalyzer.Infrastructure.Reporting;

/// <summary>
/// Генератор Excel отчёта по трендовым ключевым словам
/// </summary>
public class TrendingKeywordReportGenerator : ITrendingKeywordReportGenerator
{
    private readonly IConfiguration _configuration;

    public TrendingKeywordReportGenerator(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<string> GenerateReportAsync(TrendingKeywordSummary summary)
    {
        var workbook = new XLWorkbook();

        // Create sheets
        CreateOverviewSheet(workbook, summary);
        CreateBestOpportunitiesSheet(workbook, summary);
        CreateRiskyNichesSheet(workbook, summary);
        CreateDetailedResultsSheet(workbook, summary);

        // Save
        var outputDir = _configuration["Reports:OutputDirectory"] ?? "Reports";
        Directory.CreateDirectory(outputDir);

        var filename = $"Trending_Keywords_Report_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
        var filepath = Path.Combine(outputDir, filename);

        workbook.SaveAs(filepath);

        return await Task.FromResult(filepath);
    }

    private void CreateOverviewSheet(IXLWorkbook workbook, TrendingKeywordSummary summary)
    {
        var ws = workbook.Worksheets.Add("Overview");
        int row = 1;

        // Title
        ws.Cell(row, 1).Value = "Trending Keywords Analysis - Overview";
        ws.Cell(row, 1).Style.Font.FontSize = 16;
        ws.Cell(row, 1).Style.Font.Bold = true;
        row += 2;

        // Summary stats
        ws.Cell(row, 1).Value = "Analysis Date:";
        ws.Cell(row, 2).Value = summary.AnalyzedAt;
        CellFormatter.FormatAsLabel(ws.Cell(row, 1));
        row++;

        ws.Cell(row, 1).Value = "Source:";
        ws.Cell(row, 2).Value = summary.Source.ToString();
        CellFormatter.FormatAsLabel(ws.Cell(row, 1));
        row++;

        ws.Cell(row, 1).Value = "Total Keywords Analyzed:";
        ws.Cell(row, 2).Value = summary.TotalKeywordsAnalyzed;
        CellFormatter.FormatAsLabel(ws.Cell(row, 1));
        CellFormatter.FormatAsInteger(ws.Cell(row, 2));
        row++;

        ws.Cell(row, 1).Value = "✅ Recommended:";
        ws.Cell(row, 2).Value = summary.RecommendedCount;
        CellFormatter.FormatAsLabel(ws.Cell(row, 1));
        CellFormatter.FormatAsInteger(ws.Cell(row, 2));
        ws.Cell(row, 2).Style.Fill.BackgroundColor = ReportColors.Success;
        row++;

        ws.Cell(row, 1).Value = "⚠️ Risky:";
        ws.Cell(row, 2).Value = summary.NotRecommendedCount;
        CellFormatter.FormatAsLabel(ws.Cell(row, 1));
        CellFormatter.FormatAsInteger(ws.Cell(row, 2));
        ws.Cell(row, 2).Style.Fill.BackgroundColor = ReportColors.Warning;
        row += 2;

        // Best opportunity
        if (summary.BestOpportunity != null)
        {
            ws.Cell(row, 1).Value = "🏆 Best Opportunity:";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Font.FontSize = 14;
            row++;

            ws.Cell(row, 1).Value = "Keyword:";
            ws.Cell(row, 2).Value = summary.BestOpportunity.Keyword;
            CellFormatter.FormatAsLabel(ws.Cell(row, 1));
            row++;

            ws.Cell(row, 1).Value = "Niche Score:";
            ws.Cell(row, 2).Value = summary.BestOpportunity.NicheScore;
            CellFormatter.FormatAsLabel(ws.Cell(row, 1));
            CellFormatter.FormatAsDecimal(ws.Cell(row, 2), 1);
            CellFormatter.ApplyScoreColor(ws.Cell(row, 2), (double)summary.BestOpportunity.NicheScore);
            row++;

            ws.Cell(row, 1).Value = "Competition:";
            ws.Cell(row, 2).Value = summary.BestOpportunity.CompetitionLevel.ToString();
            CellFormatter.FormatAsLabel(ws.Cell(row, 1));
            CellFormatter.ApplyCompetitionColor(ws.Cell(row, 2), summary.BestOpportunity.CompetitionLevel);
            row++;

            ws.Cell(row, 1).Value = "Average Price:";
            ws.Cell(row, 2).Value = summary.BestOpportunity.AveragePrice;
            CellFormatter.FormatAsLabel(ws.Cell(row, 1));
            CellFormatter.FormatAsCurrency(ws.Cell(row, 2));
        }

        CellFormatter.AutoFitColumns(ws);
    }

    private void CreateBestOpportunitiesSheet(IXLWorkbook workbook, TrendingKeywordSummary summary)
    {
        var ws = workbook.Worksheets.Add("Best Opportunities");
        int row = 1;

        // Header
        var headers = new[] { "Keyword", "Score", "Competition", "Listings", "Avg Price", "Why Interesting" };
        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(row, i + 1).Value = headers[i];
        }
        CellFormatter.FormatAsTableHeader(ws.Range(row, 1, row, headers.Length));
        row++;

        // Data
        var recommended = summary.Results
            .Where(r => r.Recommendation == "✅ Recommended")
            .OrderByDescending(r => r.NicheScore)
            .ToList();

        foreach (var result in recommended)
        {
            ws.Cell(row, 1).Value = result.Keyword;
            ws.Cell(row, 2).Value = result.NicheScore;
            ws.Cell(row, 3).Value = result.CompetitionLevel.ToString();
            ws.Cell(row, 4).Value = result.TotalListings;
            ws.Cell(row, 5).Value = result.AveragePrice;
            ws.Cell(row, 6).Value = result.WhyInteresting;

            CellFormatter.FormatAsDecimal(ws.Cell(row, 2), 1);
            CellFormatter.ApplyScoreColor(ws.Cell(row, 2), (double)result.NicheScore);
            CellFormatter.ApplyCompetitionColor(ws.Cell(row, 3), result.CompetitionLevel);
            CellFormatter.FormatAsInteger(ws.Cell(row, 4));
            CellFormatter.FormatAsCurrency(ws.Cell(row, 5));

            row++;
        }

        CellFormatter.ApplyBorder(ws.Range(1, 1, row - 1, headers.Length));
        CellFormatter.ApplyZebraStriping(ws.Range(2, 1, row - 1, headers.Length));
        CellFormatter.AutoFitColumns(ws);
        CellFormatter.FreezeHeader(ws);
    }

    private void CreateRiskyNichesSheet(IXLWorkbook workbook, TrendingKeywordSummary summary)
    {
        var ws = workbook.Worksheets.Add("Risky Niches");
        int row = 1;

        // Header
        var headers = new[] { "Keyword", "Score", "Competition", "Listings", "Avg Price", "Risks" };
        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(row, i + 1).Value = headers[i];
        }
        CellFormatter.FormatAsTableHeader(ws.Range(row, 1, row, headers.Length));
        row++;

        // Data
        var risky = summary.Results
            .Where(r => r.Recommendation == "⚠️ Risky")
            .OrderBy(r => r.NicheScore)
            .ToList();

        foreach (var result in risky)
        {
            ws.Cell(row, 1).Value = result.Keyword;
            ws.Cell(row, 2).Value = result.NicheScore;
            ws.Cell(row, 3).Value = result.CompetitionLevel.ToString();
            ws.Cell(row, 4).Value = result.TotalListings;
            ws.Cell(row, 5).Value = result.AveragePrice;
            ws.Cell(row, 6).Value = result.Risks.Any() ? string.Join("; ", result.Risks) : "N/A";

            CellFormatter.FormatAsDecimal(ws.Cell(row, 2), 1);
            CellFormatter.ApplyScoreColor(ws.Cell(row, 2), (double)result.NicheScore);
            CellFormatter.ApplyCompetitionColor(ws.Cell(row, 3), result.CompetitionLevel);
            CellFormatter.FormatAsInteger(ws.Cell(row, 4));
            CellFormatter.FormatAsCurrency(ws.Cell(row, 5));

            row++;
        }

        CellFormatter.ApplyBorder(ws.Range(1, 1, row - 1, headers.Length));
        CellFormatter.ApplyZebraStriping(ws.Range(2, 1, row - 1, headers.Length));
        CellFormatter.AutoFitColumns(ws);
        CellFormatter.FreezeHeader(ws);
    }

    private void CreateDetailedResultsSheet(IXLWorkbook workbook, TrendingKeywordSummary summary)
    {
        var ws = workbook.Worksheets.Add("Detailed Results");
        int row = 1;

        // Header
        var headers = new[] { "Keyword", "Recommendation", "Score", "Competition", "Listings", "Avg Price", "Median Price", "Reason", "Top Tags" };
        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(row, i + 1).Value = headers[i];
        }
        CellFormatter.FormatAsTableHeader(ws.Range(row, 1, row, headers.Length));
        row++;

        // Data
        foreach (var result in summary.Results.OrderByDescending(r => r.NicheScore))
        {
            ws.Cell(row, 1).Value = result.Keyword;
            ws.Cell(row, 2).Value = result.Recommendation;
            ws.Cell(row, 3).Value = result.NicheScore;
            ws.Cell(row, 4).Value = result.CompetitionLevel.ToString();
            ws.Cell(row, 5).Value = result.TotalListings;
            ws.Cell(row, 6).Value = result.AveragePrice;
            ws.Cell(row, 7).Value = result.MedianPrice;
            ws.Cell(row, 8).Value = result.Reason;
            ws.Cell(row, 9).Value = string.Join(", ", result.TopTags.Take(3));

            CellFormatter.FormatAsDecimal(ws.Cell(row, 3), 1);
            CellFormatter.ApplyScoreColor(ws.Cell(row, 3), (double)result.NicheScore);
            CellFormatter.ApplyCompetitionColor(ws.Cell(row, 4), result.CompetitionLevel);
            CellFormatter.FormatAsInteger(ws.Cell(row, 5));
            CellFormatter.FormatAsCurrency(ws.Cell(row, 6));
            CellFormatter.FormatAsCurrency(ws.Cell(row, 7));

            row++;
        }

        CellFormatter.ApplyBorder(ws.Range(1, 1, row - 1, headers.Length));
        CellFormatter.ApplyZebraStriping(ws.Range(2, 1, row - 1, headers.Length));
        CellFormatter.AutoFitColumns(ws);
        CellFormatter.FreezeHeader(ws);
    }
}
