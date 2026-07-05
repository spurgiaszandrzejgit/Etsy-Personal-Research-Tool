using EtsyAnalyzer.Core.DTOs;

namespace EtsyAnalyzer.Core.Interfaces;

/// <summary>
/// Генератор отчёта по трендовым ключевым словам
/// </summary>
public interface ITrendingKeywordReportGenerator
{
    /// <summary>
    /// Генерировать Excel отчёт
    /// </summary>
    Task<string> GenerateReportAsync(TrendingKeywordSummary summary);
}
