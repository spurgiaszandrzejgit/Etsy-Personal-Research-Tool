using ClosedXML.Excel;
using EtsyAnalyzer.Core.DTOs;

namespace EtsyAnalyzer.Infrastructure.Reporting.Formatters;

public class ShopsSheetFormatter
{
    public void FormatSheet(IXLWorksheet worksheet, List<ShopCompetitorDto> shops)
    {
        worksheet.Name = "Shops";

        // Заголовки
        var headers = new[]
        {
            "Rank", "Shop Name", "Listings in Search", "Total Listings",
            "Rating", "Reviews", "URL"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            var cell = worksheet.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#70AD47");
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        // Данные
        int row = 2;
        int rank = 1;
        foreach (var shop in shops.OrderByDescending(s => s.ListingsInSearch))
        {
            worksheet.Cell(row, 1).Value = rank;
            worksheet.Cell(row, 2).Value = shop.ShopName;
            worksheet.Cell(row, 3).Value = shop.ListingsInSearch;
            worksheet.Cell(row, 4).Value = shop.ListingCount;
            worksheet.Cell(row, 5).Value = shop.Rating?.ToString("F2") ?? "N/A";
            worksheet.Cell(row, 6).Value = shop.ReviewCount;

            var urlCell = worksheet.Cell(row, 7);
            urlCell.Value = shop.Url;
            urlCell.SetHyperlink(new XLHyperlink(shop.Url));
            urlCell.Style.Font.FontColor = XLColor.Blue;
            urlCell.Style.Font.Underline = XLFontUnderlineValues.Single;

            // Условное форматирование для топ-3
            if (rank <= 3)
            {
                var rankCell = worksheet.Cell(row, 1);
                rankCell.Style.Fill.BackgroundColor = rank switch
                {
                    1 => XLColor.FromHtml("#FFD700"), // Золото
                    2 => XLColor.FromHtml("#C0C0C0"), // Серебро
                    3 => XLColor.FromHtml("#CD7F32"), // Бронза
                    _ => XLColor.White
                };
                rankCell.Style.Font.Bold = true;
            }

            // Выделение магазинов с большим количеством товаров в поиске
            var listingsCell = worksheet.Cell(row, 3);
            if (shop.ListingsInSearch > 10)
            {
                listingsCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFC7CE");
                listingsCell.Style.Font.Bold = true;
            }
            else if (shop.ListingsInSearch > 5)
            {
                listingsCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFEB9C");
            }

            row++;
            rank++;
        }

        // Автоширина столбцов
        worksheet.Columns().AdjustToContents();
        worksheet.Column(2).Width = 30; // Shop Name
        worksheet.Column(7).Width = 15; // URL

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
