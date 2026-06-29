using ClosedXML.Excel;
using EtsyAnalyzer.Core.DTOs;
using EtsyAnalyzer.Infrastructure.Reporting.Interfaces;
using EtsyAnalyzer.Infrastructure.Reporting.Utilities;

namespace EtsyAnalyzer.Infrastructure.Reporting.Builders;

/// <summary>
/// Построитель листа Top Listings с самыми дорогими и дешевыми товарами
/// </summary>
public class TopListingsWorksheetBuilder : IWorksheetBuilder
{
    public string WorksheetName => "Top Listings";
    public int Order => 7;

    public void Build(IXLWorkbook workbook, AnalyticsSummaryDto summary)
    {
        var ws = workbook.Worksheets.Add(WorksheetName);

        int currentRow = 1;

        // Топ-10 самых дорогих
        BuildTopExpensive(ws, ref currentRow, summary);

        currentRow += 2;

        // Топ-10 самых дешевых
        BuildTopCheapest(ws, ref currentRow, summary);

        CellFormatter.AutoFitColumns(ws);
        CellFormatter.FreezeHeader(ws);
    }

    private void BuildTopExpensive(IXLWorksheet ws, ref int row, AnalyticsSummaryDto summary)
    {
        // Заголовок секции
        var sectionHeader = ws.Cell(row, 1);
        sectionHeader.Value = "TOP 10 MOST EXPENSIVE";
        sectionHeader.Style.Font.FontSize = 12;
        sectionHeader.Style.Font.Bold = true;
        sectionHeader.Style.Fill.BackgroundColor = ReportColors.HighPrice;
        ws.Range(row, 1, row, 5).Merge();
        CellFormatter.ApplyBorder(ws.Range(row, 1, row, 5));
        row++;

        // Заголовки таблицы
        var headers = new[] { "Title", "Price", "Shop", "Rating", "Reviews" };
        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(row, i + 1).Value = headers[i];
        }
        CellFormatter.FormatAsTableHeader(ws.Range(row, 1, row, headers.Length));
        row++;

        // Данные
        var topExpensive = summary.Listings
            .OrderByDescending(l => l.Price)
            .Take(10);

        int startRow = row;
        foreach (var listing in topExpensive)
        {
            ws.Cell(row, 1).Value = listing.Title;

            var priceCell = ws.Cell(row, 2);
            priceCell.Value = listing.Price;
            CellFormatter.FormatAsCurrency(priceCell, listing.CurrencySymbol);
            priceCell.Style.Font.Bold = true;

            ws.Cell(row, 3).Value = listing.ShopName;

            var ratingCell = ws.Cell(row, 4);
            ratingCell.Value = listing.Rating ?? 0;
            CellFormatter.FormatAsDecimal(ratingCell, 1);

            var reviewsCell = ws.Cell(row, 5);
            reviewsCell.Value = listing.ReviewCount;
            CellFormatter.FormatAsInteger(reviewsCell);

            row++;
        }

        CellFormatter.ApplyZebraStriping(ws.Range(startRow, 1, row - 1, headers.Length));
        CellFormatter.ApplyBorder(ws.Range(startRow, 1, row - 1, headers.Length));
    }

    private void BuildTopCheapest(IXLWorksheet ws, ref int row, AnalyticsSummaryDto summary)
    {
        // Заголовок секции
        var sectionHeader = ws.Cell(row, 1);
        sectionHeader.Value = "TOP 10 CHEAPEST";
        sectionHeader.Style.Font.FontSize = 12;
        sectionHeader.Style.Font.Bold = true;
        sectionHeader.Style.Fill.BackgroundColor = ReportColors.LowPrice;
        ws.Range(row, 1, row, 5).Merge();
        CellFormatter.ApplyBorder(ws.Range(row, 1, row, 5));
        row++;

        // Заголовки таблицы
        var headers = new[] { "Title", "Price", "Shop", "Rating", "Reviews" };
        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(row, i + 1).Value = headers[i];
        }
        CellFormatter.FormatAsTableHeader(ws.Range(row, 1, row, headers.Length));
        row++;

        // Данные
        var topCheapest = summary.Listings
            .OrderBy(l => l.Price)
            .Take(10);

        int startRow = row;
        foreach (var listing in topCheapest)
        {
            ws.Cell(row, 1).Value = listing.Title;

            var priceCell = ws.Cell(row, 2);
            priceCell.Value = listing.Price;
            CellFormatter.FormatAsCurrency(priceCell, listing.CurrencySymbol);
            priceCell.Style.Font.Bold = true;

            ws.Cell(row, 3).Value = listing.ShopName;

            var ratingCell = ws.Cell(row, 4);
            ratingCell.Value = listing.Rating ?? 0;
            CellFormatter.FormatAsDecimal(ratingCell, 1);

            var reviewsCell = ws.Cell(row, 5);
            reviewsCell.Value = listing.ReviewCount;
            CellFormatter.FormatAsInteger(reviewsCell);

            row++;
        }

        CellFormatter.ApplyZebraStriping(ws.Range(startRow, 1, row - 1, headers.Length));
        CellFormatter.ApplyBorder(ws.Range(startRow, 1, row - 1, headers.Length));
    }
}
