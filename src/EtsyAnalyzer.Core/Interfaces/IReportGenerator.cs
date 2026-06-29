using EtsyAnalyzer.Core.DTOs;

namespace EtsyAnalyzer.Core.Interfaces;

/// <summary>
/// Генератор отчетов (Excel, PDF и т.д.)
/// </summary>
public interface IReportGenerator
{
    /// <summary>
    /// Тип отчета (Excel, PDF и т.д.)
    /// </summary>
    string ReportType { get; }

    /// <summary>
    /// Сгенерировать отчет на основе результатов анализа
    /// </summary>
    Task<string> GenerateReportAsync(AnalyticsSummaryDto summary, IEnumerable<ListingDto> listings, string outputPath, CancellationToken cancellationToken = default);
}
