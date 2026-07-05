using ClosedXML.Excel;
using EtsyAnalyzer.Core.DTOs;

namespace EtsyAnalyzer.Infrastructure.Reporting.Interfaces;

/// <summary>
/// Интерфейс для построителей листов Excel-отчета
/// </summary>
public interface IWorksheetBuilder
{
    /// <summary>
    /// Название листа
    /// </summary>
    string WorksheetName { get; }

    /// <summary>
    /// Порядок сортировки (меньше = раньше)
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Построить лист в рабочей книге
    /// </summary>
    /// <param name="workbook">Рабочая книга Excel</param>
    /// <param name="summary">Данные анализа</param>
    void Build(IXLWorkbook workbook, AnalyticsSummaryDto summary);
}
