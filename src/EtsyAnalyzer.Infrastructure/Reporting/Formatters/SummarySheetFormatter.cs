using ClosedXML.Excel;
using EtsyAnalyzer.Core.DTOs;
using EtsyAnalyzer.Core.ValueObjects;

namespace EtsyAnalyzer.Infrastructure.Reporting.Formatters;

public class SummarySheetFormatter
{
    public void FormatSheet(IXLWorksheet worksheet, AnalyticsSummaryDto summary)
    {
        worksheet.Name = "Summary";

        // Заголовок
        worksheet.Cell("A1").Value = "Etsy Niche Analysis Report";
        worksheet.Cell("A1").Style.Font.FontSize = 18;
        worksheet.Cell("A1").Style.Font.Bold = true;
        worksheet.Cell("A1").Style.Font.FontColor = XLColor.White;
        worksheet.Cell("A1").Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
        worksheet.Range("A1:B1").Merge();
        worksheet.Range("A1:B1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Дата отчета
        worksheet.Cell("A2").Value = "Report Date:";
        worksheet.Cell("B2").Value = summary.AnalyzedAt.ToString("yyyy-MM-dd HH:mm:ss UTC");
        worksheet.Cell("A2").Style.Font.Bold = true;

        // Основная информация
        var row = 4;
        AddSummaryRow(worksheet, ref row, "Search Query", summary.Query);
        AddSummaryRow(worksheet, ref row, "Total Listings Found", summary.TotalListings.ToString());

        row++;
        worksheet.Cell($"A{row}").Value = "PRICE STATISTICS";
        worksheet.Cell($"A{row}").Style.Font.Bold = true;
        worksheet.Cell($"A{row}").Style.Fill.BackgroundColor = XLColor.LightGray;
        worksheet.Range($"A{row}:B{row}").Merge();
        row++;

        AddSummaryRow(worksheet, ref row, "Average Price", $"{summary.PriceStatistics.Average:F2} {summary.PriceStatistics.CurrencyCode}");
        AddSummaryRow(worksheet, ref row, "Median Price", $"{summary.PriceStatistics.Median:F2} {summary.PriceStatistics.CurrencyCode}");
        AddSummaryRow(worksheet, ref row, "Min Price", $"{summary.PriceStatistics.Min:F2} {summary.PriceStatistics.CurrencyCode}");
        AddSummaryRow(worksheet, ref row, "Max Price", $"{summary.PriceStatistics.Max:F2} {summary.PriceStatistics.CurrencyCode}");

        row++;
        worksheet.Cell($"A{row}").Value = "COMPETITION ANALYSIS";
        worksheet.Cell($"A{row}").Style.Font.Bold = true;
        worksheet.Cell($"A{row}").Style.Fill.BackgroundColor = XLColor.LightGray;
        worksheet.Range($"A{row}:B{row}").Merge();
        row++;

        var competitionCell = AddSummaryRow(worksheet, ref row, "Competition Level", summary.CompetitionLevel.ToDisplayString());

        // Условное форматирование для уровня конкуренции
        var competitionColor = summary.CompetitionLevel switch
        {
            Core.ValueObjects.CompetitionLevel.Low => XLColor.FromHtml("#C6EFCE"),
            Core.ValueObjects.CompetitionLevel.Medium => XLColor.FromHtml("#FFEB9C"),
            Core.ValueObjects.CompetitionLevel.High => XLColor.FromHtml("#FFC7CE"),
            _ => XLColor.White
        };
        competitionCell.Style.Fill.BackgroundColor = competitionColor;

        row++;
        worksheet.Cell($"A{row}").Value = "NICHE SCORE";
        worksheet.Cell($"A{row}").Style.Font.Bold = true;
        worksheet.Cell($"A{row}").Style.Fill.BackgroundColor = XLColor.LightGray;
        worksheet.Range($"A{row}:B{row}").Merge();
        row++;

        var scoreCell = AddSummaryRow(worksheet, ref row, "Overall Niche Score", summary.NicheScore.ToString());
        scoreCell.Style.Font.FontSize = 14;
        scoreCell.Style.Font.Bold = true;

        // Условное форматирование для оценки
        var scoreColor = summary.NicheScore.Value switch
        {
            >= 8.0m => XLColor.FromHtml("#C6EFCE"), // Зеленый
            >= 6.0m => XLColor.FromHtml("#FFEB9C"),  // Желтый
            >= 4.0m => XLColor.FromHtml("#FFC7CE"),  // Красный
            _ => XLColor.FromHtml("#FF6B6B")         // Темно-красный
        };
        scoreCell.Style.Fill.BackgroundColor = scoreColor;

        // Price Ranges
        row += 2;
        worksheet.Cell($"A{row}").Value = "PRICE DISTRIBUTION";
        worksheet.Cell($"A{row}").Style.Font.Bold = true;
        worksheet.Cell($"A{row}").Style.Fill.BackgroundColor = XLColor.LightGray;
        worksheet.Range($"A{row}:D{row}").Merge();
        row++;

        worksheet.Cell($"A{row}").Value = "Price Range";
        worksheet.Cell($"B{row}").Value = "Min";
        worksheet.Cell($"C{row}").Value = "Max";
        worksheet.Cell($"D{row}").Value = "Count";
        worksheet.Cell($"E{row}").Value = "Percentage";
        worksheet.Range($"A{row}:E{row}").Style.Font.Bold = true;
        worksheet.Range($"A{row}:E{row}").Style.Fill.BackgroundColor = XLColor.LightBlue;
        row++;

        foreach (var range in summary.PriceStatistics.PriceRanges)
        {
            worksheet.Cell($"A{row}").Value = range.Label;
            worksheet.Cell($"B{row}").Value = range.MinPrice;
            worksheet.Cell($"C{row}").Value = range.MaxPrice;
            worksheet.Cell($"D{row}").Value = range.Count;
            worksheet.Cell($"E{row}").Value = $"{range.Percentage}%";
            row++;
        }

        // Автоширина столбцов
        worksheet.Columns().AdjustToContents();

        // Границы
        worksheet.Range($"A1:B{row - 1}").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
    }

    private IXLCell AddSummaryRow(IXLWorksheet worksheet, ref int row, string label, string value)
    {
        worksheet.Cell($"A{row}").Value = label;
        worksheet.Cell($"A{row}").Style.Font.Bold = true;
        var valueCell = worksheet.Cell($"B{row}");
        valueCell.Value = value;
        row++;
        return valueCell;
    }
}
