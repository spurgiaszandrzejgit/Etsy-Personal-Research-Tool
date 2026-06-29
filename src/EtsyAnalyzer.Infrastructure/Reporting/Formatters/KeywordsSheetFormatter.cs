using ClosedXML.Excel;
using EtsyAnalyzer.Core.DTOs;

namespace EtsyAnalyzer.Infrastructure.Reporting.Formatters;

public class KeywordsSheetFormatter
{
    public void FormatSheet(IXLWorksheet worksheet, AnalyticsSummaryDto summary)
    {
        worksheet.Name = "Keywords";

        // Секция: TOP KEYWORDS
        int row = 1;
        worksheet.Cell($"A{row}").Value = "TOP KEYWORDS FROM TITLES";
        worksheet.Cell($"A{row}").Style.Font.FontSize = 14;
        worksheet.Cell($"A{row}").Style.Font.Bold = true;
        worksheet.Cell($"A{row}").Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
        worksheet.Cell($"A{row}").Style.Font.FontColor = XLColor.White;
        worksheet.Range($"A{row}:D{row}").Merge();
        worksheet.Range($"A{row}:D{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        row++;

        // Заголовки для ключевых слов
        worksheet.Cell($"A{row}").Value = "Rank";
        worksheet.Cell($"B{row}").Value = "Keyword";
        worksheet.Cell($"C{row}").Value = "Frequency";
        worksheet.Cell($"D{row}").Value = "Percentage";
        var headerRange = worksheet.Range($"A{row}:D{row}");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        row++;

        // Данные ключевых слов
        int keywordRank = 1;
        foreach (var keyword in summary.TopKeywords)
        {
            worksheet.Cell($"A{row}").Value = keywordRank;
            worksheet.Cell($"B{row}").Value = keyword.Keyword;
            worksheet.Cell($"C{row}").Value = keyword.Frequency;
            worksheet.Cell($"D{row}").Value = $"{keyword.Percentage}%";

            // Условное форматирование
            var freqCell = worksheet.Cell($"C{row}");
            if (keyword.Frequency > summary.TotalListings * 0.5m)
                freqCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#C6EFCE");
            else if (keyword.Frequency > summary.TotalListings * 0.3m)
                freqCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFEB9C");

            row++;
            keywordRank++;
        }

        var keywordDataRange = worksheet.Range($"A2:D{row - 1}");
        keywordDataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        keywordDataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        // Пустая строка
        row += 2;

        // Секция: TOP TAGS
        worksheet.Cell($"A{row}").Value = "TOP TAGS";
        worksheet.Cell($"A{row}").Style.Font.FontSize = 14;
        worksheet.Cell($"A{row}").Style.Font.Bold = true;
        worksheet.Cell($"A{row}").Style.Fill.BackgroundColor = XLColor.FromHtml("#70AD47");
        worksheet.Cell($"A{row}").Style.Font.FontColor = XLColor.White;
        worksheet.Range($"A{row}:D{row}").Merge();
        worksheet.Range($"A{row}:D{row}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        row++;

        // Заголовки для тегов
        worksheet.Cell($"A{row}").Value = "Rank";
        worksheet.Cell($"B{row}").Value = "Tag";
        worksheet.Cell($"C{row}").Value = "Frequency";
        worksheet.Cell($"D{row}").Value = "Percentage";
        headerRange = worksheet.Range($"A{row}:D{row}");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGreen;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        row++;

        // Данные тегов
        int tagRank = 1;
        foreach (var tag in summary.TopTags)
        {
            worksheet.Cell($"A{row}").Value = tagRank;
            worksheet.Cell($"B{row}").Value = tag.Keyword;
            worksheet.Cell($"C{row}").Value = tag.Frequency;
            worksheet.Cell($"D{row}").Value = $"{tag.Percentage}%";

            // Условное форматирование
            var freqCell = worksheet.Cell($"C{row}");
            if (tag.Frequency > summary.TotalListings * 0.4m)
                freqCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#C6EFCE");
            else if (tag.Frequency > summary.TotalListings * 0.2m)
                freqCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFEB9C");

            row++;
            tagRank++;
        }

        var tagDataRange = worksheet.Range($"A{row - summary.TopTags.Count}:D{row - 1}");
        tagDataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        tagDataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        // Автоширина столбцов
        worksheet.Columns().AdjustToContents();
        worksheet.Column(2).Width = 30; // Keyword/Tag column
    }
}
