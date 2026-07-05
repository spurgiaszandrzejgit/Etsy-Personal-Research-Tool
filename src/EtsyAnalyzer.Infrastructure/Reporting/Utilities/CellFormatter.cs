using ClosedXML.Excel;

namespace EtsyAnalyzer.Infrastructure.Reporting.Utilities;

/// <summary>
/// Палитра цветов для отчетов
/// </summary>
public static class ReportColors
{
    // Основные цвета
    public static readonly XLColor Primary = XLColor.FromArgb(79, 129, 189);
    public static readonly XLColor Success = XLColor.FromArgb(155, 187, 89);
    public static readonly XLColor Warning = XLColor.FromArgb(247, 150, 70);
    public static readonly XLColor Danger = XLColor.FromArgb(192, 80, 77);

    // Цвета для фона
    public static readonly XLColor HeaderBackground = XLColor.FromArgb(79, 129, 189);
    public static readonly XLColor LightGray = XLColor.FromArgb(217, 217, 217);
    public static readonly XLColor VeryLightGray = XLColor.FromArgb(242, 242, 242);
    public static readonly XLColor DarkGray = XLColor.FromArgb(89, 89, 89);

    // Цвета для уровня конкуренции
    public static readonly XLColor LowCompetition = Success;
    public static readonly XLColor MediumCompetition = Warning;
    public static readonly XLColor HighCompetition = Danger;

    // Цвета для цен
    public static readonly XLColor HighPrice = XLColor.FromArgb(198, 224, 180);
    public static readonly XLColor LowPrice = XLColor.FromArgb(255, 230, 153);
}

/// <summary>
/// Утилиты для форматирования ячеек Excel
/// </summary>
public static class CellFormatter
{
    #region Автоширина и заморозка

    /// <summary>
    /// Применить автоширину к столбцам
    /// </summary>
    public static void AutoFitColumns(IXLWorksheet ws)
    {
        ws.Columns().AdjustToContents();
    }

    /// <summary>
    /// Заморозить первую строку
    /// </summary>
    public static void FreezeHeader(IXLWorksheet ws, int rows = 1)
    {
        ws.SheetView.FreezeRows(rows);
    }

    #endregion

    #region Форматирование числовых значений

    /// <summary>
    /// Форматировать как целое число
    /// </summary>
    public static void FormatAsInteger(IXLCell cell)
    {
        cell.Style.NumberFormat.Format = "#,##0";
    }

    /// <summary>
    /// Форматировать как десятичное число
    /// </summary>
    public static void FormatAsDecimal(IXLCell cell, int decimals = 2)
    {
        var format = decimals == 0 ? "#,##0" : $"#,##0.{new string('0', decimals)}";
        cell.Style.NumberFormat.Format = format;
    }

    /// <summary>
    /// Форматировать как валюту
    /// </summary>
    public static void FormatAsCurrency(IXLCell cell, string currencySymbol = "$")
    {
        cell.Style.NumberFormat.Format = $"{currencySymbol}#,##0.00";
    }

    /// <summary>
    /// Форматировать как процент
    /// </summary>
    public static void FormatAsPercent(IXLCell cell, int decimals = 0)
    {
        var format = decimals == 0 ? "0%" : $"0.{new string('0', decimals)}%";
        cell.Style.NumberFormat.Format = format;
        cell.Value = Convert.ToDouble(cell.Value) / 100;
    }

    #endregion

    #region Стили ячеек

    /// <summary>
    /// Применить стиль заголовка таблицы
    /// </summary>
    public static void FormatAsTableHeader(IXLRange range)
    {
        range.Style.Font.Bold = true;
        range.Style.Fill.BackgroundColor = ReportColors.HeaderBackground;
        range.Style.Font.FontColor = XLColor.White;
        range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        ApplyBorder(range);
    }

    /// <summary>
    /// Применить стиль метки (label)
    /// </summary>
    public static void FormatAsLabel(IXLCell cell)
    {
        cell.Style.Font.Bold = true;
        cell.Style.Fill.BackgroundColor = ReportColors.LightGray;
    }

    #endregion

    #region Рамки и зебра

    /// <summary>
    /// Применить рамку к диапазону
    /// </summary>
    public static void ApplyBorder(IXLRange range)
    {
        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        range.Style.Border.InsideBorder = XLBorderStyleValues.Hair;
    }

    /// <summary>
    /// Применить зебра-полосы к диапазону
    /// </summary>
    public static void ApplyZebraStriping(IXLRange range)
    {
        for (int i = 0; i < range.RowCount(); i++)
        {
            if (i % 2 == 1)
            {
                range.Row(i + 1).Style.Fill.BackgroundColor = ReportColors.VeryLightGray;
            }
        }
    }

    #endregion

    #region Цветовое кодирование

    /// <summary>
    /// Применить цвет на основе уровня конкуренции (строка)
    /// </summary>
    public static void ApplyCompetitionColor(IXLCell cell, string competitionLevel)
    {
        var color = competitionLevel.ToLower() switch
        {
            "low" => ReportColors.LowCompetition,
            "medium" => ReportColors.MediumCompetition,
            "high" => ReportColors.HighCompetition,
            _ => XLColor.White
        };
        cell.Style.Fill.BackgroundColor = color;
        cell.Style.Font.Bold = true;
    }

    /// <summary>
    /// Применить цвет на основе уровня конкуренции (enum)
    /// </summary>
    public static void ApplyCompetitionColor(IXLCell cell, EtsyAnalyzer.Core.ValueObjects.CompetitionLevel level)
    {
        var color = level switch
        {
            EtsyAnalyzer.Core.ValueObjects.CompetitionLevel.Low => ReportColors.LowCompetition,
            EtsyAnalyzer.Core.ValueObjects.CompetitionLevel.Medium => ReportColors.MediumCompetition,
            EtsyAnalyzer.Core.ValueObjects.CompetitionLevel.High => ReportColors.HighCompetition,
            _ => XLColor.White
        };
        cell.Style.Fill.BackgroundColor = color;
        cell.Style.Font.Bold = true;
    }

    /// <summary>
    /// Применить цвет на основе оценки (1-10)
    /// </summary>
    public static void ApplyScoreColor(IXLCell cell, double score)
    {
        var color = score switch
        {
            >= 7.0 => ReportColors.Success,
            >= 4.0 => ReportColors.Warning,
            _ => ReportColors.Danger
        };
        cell.Style.Fill.BackgroundColor = color;
        cell.Style.Font.Bold = true;
    }

    #endregion
}
