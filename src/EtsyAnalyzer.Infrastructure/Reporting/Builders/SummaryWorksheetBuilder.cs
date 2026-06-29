using ClosedXML.Excel;
using EtsyAnalyzer.Core.DTOs;
using EtsyAnalyzer.Infrastructure.Reporting.Interfaces;
using EtsyAnalyzer.Infrastructure.Reporting.Utilities;

namespace EtsyAnalyzer.Infrastructure.Reporting.Builders;

/// <summary>
/// Построитель листа Summary с общей информацией и AI резюме
/// </summary>
public class SummaryWorksheetBuilder : IWorksheetBuilder
{
    public string WorksheetName => "Summary";
    public int Order => 1;

    public void Build(IXLWorkbook workbook, AnalyticsSummaryDto summary)
    {
        var ws = workbook.Worksheets.Add(WorksheetName);

        int currentRow = 1;

        // Заголовок отчета
        BuildHeader(ws, ref currentRow, summary);

        // Основные метрики
        BuildKeyMetrics(ws, ref currentRow, summary);

        // Детальная оценка
        BuildScoreBreakdown(ws, ref currentRow, summary);

        // AI Summary
        BuildAiSummary(ws, ref currentRow, summary);

        // Форматирование
        CellFormatter.AutoFitColumns(ws);
        CellFormatter.FreezeHeader(ws, 1);
    }

    private void BuildHeader(IXLWorksheet ws, ref int row, AnalyticsSummaryDto summary)
    {
        // Заголовок
        var titleCell = ws.Cell(row, 1);
        titleCell.Value = "ETSY NICHE ANALYSIS REPORT";
        titleCell.Style.Font.FontSize = 18;
        titleCell.Style.Font.Bold = true;
        titleCell.Style.Font.FontColor = ReportColors.HeaderBackground;
        ws.Range(row, 1, row, 6).Merge();
        row++;

        // Подзаголовок
        var subtitleCell = ws.Cell(row, 1);
        subtitleCell.Value = $"Query: \"{summary.SearchQuery}\"";
        subtitleCell.Style.Font.FontSize = 14;
        subtitleCell.Style.Font.Italic = true;
        ws.Range(row, 1, row, 6).Merge();
        row++;

        // Дата анализа
        var dateCell = ws.Cell(row, 1);
        dateCell.Value = $"Analysis Date: {DateTime.Now:yyyy-MM-dd HH:mm}";
        dateCell.Style.Font.FontSize = 10;
        dateCell.Style.Font.FontColor = ReportColors.DarkGray;
        ws.Range(row, 1, row, 6).Merge();
        row += 2;
    }

    private void BuildKeyMetrics(IXLWorksheet ws, ref int row, AnalyticsSummaryDto summary)
    {
        // Заголовок секции
        var sectionHeader = ws.Cell(row, 1);
        sectionHeader.Value = "KEY METRICS";
        sectionHeader.Style.Font.FontSize = 12;
        sectionHeader.Style.Font.Bold = true;
        sectionHeader.Style.Fill.BackgroundColor = ReportColors.LightGray;
        ws.Range(row, 1, row, 6).Merge();
        CellFormatter.ApplyBorder(ws.Range(row, 1, row, 6));
        row++;

        // Метрики
        var metrics = new (string Label, object Value, Action<IXLCell>? Formatter)[]
        {
            ("Total Listings", summary.TotalListings, CellFormatter.FormatAsInteger),
            ("Unique Shops", summary.UniqueShops, CellFormatter.FormatAsInteger),
            ("Competition Level", summary.CompetitionLevel.ToString(), cell => CellFormatter.ApplyCompetitionColor(cell, summary.CompetitionLevel.ToString())),
            ("Average Price", summary.AveragePrice, cell => CellFormatter.FormatAsCurrency(cell, summary.CurrencySymbol)),
            ("Median Price", summary.MedianPrice, cell => CellFormatter.FormatAsCurrency(cell, summary.CurrencySymbol)),
            ("Min Price", summary.MinPrice, cell => CellFormatter.FormatAsCurrency(cell, summary.CurrencySymbol)),
            ("Max Price", summary.MaxPrice, cell => CellFormatter.FormatAsCurrency(cell, summary.CurrencySymbol)),
            ("Std Deviation", summary.StandardDeviation ?? 0, cell => CellFormatter.FormatAsCurrency(cell, summary.CurrencySymbol)),
            ("Niche Score", (double)summary.NicheScore.Value, cell => 
            {
                CellFormatter.FormatAsDecimal(cell, 1);
                CellFormatter.ApplyScoreColor(cell, (double)summary.NicheScore.Value);
            })
        };

        foreach (var (label, value, formatter) in metrics)
        {
            var labelCell = ws.Cell(row, 1);
            labelCell.Value = label;
            CellFormatter.FormatAsLabel(labelCell);

            var valueCell = ws.Cell(row, 2);
            valueCell.Value = (XLCellValue)Convert.ChangeType(value, value.GetType());
            formatter?.Invoke(valueCell);

            CellFormatter.ApplyBorder(ws.Range(row, 1, row, 2));
            row++;
        }

        row++;
    }

    private void BuildScoreBreakdown(IXLWorksheet ws, ref int row, AnalyticsSummaryDto summary)
    {
        // Заголовок секции
        var sectionHeader = ws.Cell(row, 1);
        sectionHeader.Value = "SCORE BREAKDOWN";
        sectionHeader.Style.Font.FontSize = 12;
        sectionHeader.Style.Font.Bold = true;
        sectionHeader.Style.Fill.BackgroundColor = ReportColors.LightGray;
        ws.Range(row, 1, row, 6).Merge();
        CellFormatter.ApplyBorder(ws.Range(row, 1, row, 6));
        row++;

        // Заголовки таблицы
        ws.Cell(row, 1).Value = "Metric";
        ws.Cell(row, 2).Value = "Value";
        ws.Cell(row, 3).Value = "Score";
        ws.Cell(row, 4).Value = "Weight";
        ws.Cell(row, 5).Value = "Contribution";
        CellFormatter.FormatAsTableHeader(ws.Range(row, 1, row, 5));
        row++;

        // Рассчитываем компоненты оценки
        var priceScore = CalculatePriceScore(summary);
        var competitionScore = CalculateCompetitionScore(summary);
        var marketSizeScore = CalculateMarketSizeScore(summary);

        const double priceWeight = 0.30;
        const double competitionWeight = 0.40;
        const double marketSizeWeight = 0.30;

        var components = new[]
        {
            ("Price Opportunity", $"{summary.CurrencySymbol}{summary.AveragePrice:F2}", priceScore, priceWeight),
            ("Competition Level", summary.CompetitionLevel.ToString(), competitionScore, competitionWeight),
            ("Market Size", $"{summary.TotalListings} listings", marketSizeScore, marketSizeWeight)
        };

        int startRow = row;
        foreach (var (metric, value, score, weight) in components)
        {
            ws.Cell(row, 1).Value = metric;
            ws.Cell(row, 2).Value = value;

            var scoreCell = ws.Cell(row, 3);
            scoreCell.Value = score;
            CellFormatter.FormatAsDecimal(scoreCell, 1);
            CellFormatter.ApplyScoreColor(scoreCell, score);

            var weightCell = ws.Cell(row, 4);
            weightCell.Value = weight;
            CellFormatter.FormatAsPercent(weightCell, 0);

            var contribCell = ws.Cell(row, 5);
            contribCell.Value = score * weight;
            CellFormatter.FormatAsDecimal(contribCell, 2);

            row++;
        }

        // Итоговый счет
        ws.Cell(row, 1).Value = "FINAL NICHE SCORE";
        ws.Cell(row, 1).Style.Font.Bold = true;

        var finalScoreCell = ws.Cell(row, 5);
        finalScoreCell.Value = (double)summary.NicheScore.Value;
        CellFormatter.FormatAsDecimal(finalScoreCell, 1);
        CellFormatter.ApplyScoreColor(finalScoreCell, (double)summary.NicheScore.Value);
        finalScoreCell.Style.Font.FontSize = 12;

        CellFormatter.ApplyBorder(ws.Range(startRow, 1, row, 5));
        CellFormatter.ApplyZebraStriping(ws.Range(startRow, 1, row - 1, 5));
        row += 2;
    }

    private void BuildAiSummary(IXLWorksheet ws, ref int row, AnalyticsSummaryDto summary)
    {
        // Заголовок секции
        var sectionHeader = ws.Cell(row, 1);
        sectionHeader.Value = "AI ANALYSIS SUMMARY";
        sectionHeader.Style.Font.FontSize = 12;
        sectionHeader.Style.Font.Bold = true;
        sectionHeader.Style.Fill.BackgroundColor = ReportColors.LightGray;
        ws.Range(row, 1, row, 6).Merge();
        CellFormatter.ApplyBorder(ws.Range(row, 1, row, 6));
        row++;

        // Генерируем AI резюме
        var aiSummary = AiSummaryGenerator.GenerateSummary(summary);

        var summaryCell = ws.Cell(row, 1);
        summaryCell.Value = aiSummary;
        summaryCell.Style.Alignment.WrapText = true;
        summaryCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
        summaryCell.Style.Font.FontSize = 9;
        summaryCell.Style.Font.FontName = "Consolas";

        ws.Range(row, 1, row + 20, 6).Merge();
        CellFormatter.ApplyBorder(ws.Range(row, 1, row + 20, 6));
        ws.Row(row).Height = 400;
    }

    private double CalculatePriceScore(AnalyticsSummaryDto summary)
    {
        // Оптимальная цена: $20-$80
        var avgPrice = summary.AveragePrice;

        if (avgPrice >= 20m && avgPrice <= 80m)
            return 10.0;
        else if (avgPrice >= 15m && avgPrice <= 100m)
            return 8.0;
        else if (avgPrice >= 10m && avgPrice <= 150m)
            return 6.0;
        else if (avgPrice < 10m)
            return 4.0; // Too low
        else
            return 5.0; // Too high
    }

    private double CalculateCompetitionScore(AnalyticsSummaryDto summary)
    {
        return summary.CompetitionLevel.ToString().ToLower() switch
        {
            "low" => 10.0,
            "medium" => 6.0,
            "high" => 3.0,
            _ => 5.0
        };
    }

    private double CalculateMarketSizeScore(AnalyticsSummaryDto summary)
    {
        // Оптимальный размер рынка: 50-500 товаров
        var total = summary.TotalListings;

        if (total >= 50 && total <= 500)
            return 10.0;
        else if (total >= 30 && total <= 1000)
            return 8.0;
        else if (total >= 20 || total <= 2000)
            return 6.0;
        else if (total < 20)
            return 4.0; // Too small
        else
            return 5.0; // Too saturated
    }
}
