using ClosedXML.Excel;
using EtsyAnalyzer.Core.DTOs;

namespace EtsyAnalyzer.Infrastructure.Reporting.Formatters;

public class ListingsSheetFormatter
{
    public void FormatSheet(IXLWorksheet worksheet, IEnumerable<ListingDto> listings)
    {
        worksheet.Name = "Listings";

        // Заголовки
        var headers = new[]
        {
            "Listing ID", "Title", "Price", "Currency", "Shop Name",
            "Rating", "Reviews", "Category", "Tags", "URL"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            var cell = worksheet.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        // Данные
        int row = 2;
        foreach (var listing in listings)
        {
            worksheet.Cell(row, 1).Value = listing.ListingId;
            worksheet.Cell(row, 2).Value = listing.Title;
            worksheet.Cell(row, 3).Value = listing.Price;
            worksheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(row, 4).Value = listing.CurrencyCode;
            worksheet.Cell(row, 5).Value = listing.ShopName;
            worksheet.Cell(row, 6).Value = listing.Rating?.ToString("F2") ?? "N/A";
            worksheet.Cell(row, 7).Value = listing.ReviewCount;
            worksheet.Cell(row, 8).Value = listing.CategoryPath ?? "";
            worksheet.Cell(row, 9).Value = string.Join(", ", listing.Tags);

            var urlCell = worksheet.Cell(row, 10);
            urlCell.Value = listing.Url;
            urlCell.SetHyperlink(new XLHyperlink(listing.Url));
            urlCell.Style.Font.FontColor = XLColor.Blue;
            urlCell.Style.Font.Underline = XLFontUnderlineValues.Single;

            // Условное форматирование для цены
            var priceCell = worksheet.Cell(row, 3);
            if (listing.Price < 20)
                priceCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#C6EFCE");
            else if (listing.Price > 100)
                priceCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFC7CE");

            row++;
        }

        // Автоширина столбцов
        worksheet.Columns().AdjustToContents();

        // Ограничим ширину для длинных столбцов
        worksheet.Column(2).Width = 50; // Title
        worksheet.Column(8).Width = 30; // Category
        worksheet.Column(9).Width = 40; // Tags
        worksheet.Column(10).Width = 15; // URL

        // Закрепление первой строки
        worksheet.SheetView.FreezeRows(1);

        // Автофильтр
        var dataRange = worksheet.Range(1, 1, row - 1, headers.Length);
        dataRange.SetAutoFilter();

        // Границы
        dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
    }
}
