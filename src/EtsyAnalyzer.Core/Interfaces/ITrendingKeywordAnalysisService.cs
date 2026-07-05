using EtsyAnalyzer.Core.DTOs;

namespace EtsyAnalyzer.Core.Interfaces;

/// <summary>
/// Сервис анализа трендовых ключевых слов
/// </summary>
public interface ITrendingKeywordAnalysisService
{
    /// <summary>
    /// Анализировать топ N трендовых ключевых слов
    /// </summary>
    Task<TrendingKeywordSummary> AnalyzeTopKeywordsAsync(int count = 10);

    /// <summary>
    /// Анализировать пользовательский список ключевых слов
    /// </summary>
    Task<TrendingKeywordSummary> AnalyzeCustomKeywordsAsync(List<string> keywords);
}
