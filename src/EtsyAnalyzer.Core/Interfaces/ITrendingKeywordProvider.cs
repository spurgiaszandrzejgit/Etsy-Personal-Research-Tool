using EtsyAnalyzer.Core.Enums;

namespace EtsyAnalyzer.Core.Interfaces;

/// <summary>
/// Провайдер трендовых ключевых слов
/// </summary>
public interface ITrendingKeywordProvider
{
    /// <summary>
    /// Имя источника
    /// </summary>
    string SourceName { get; }

    /// <summary>
    /// Тип источника
    /// </summary>
    TrendingKeywordSource Source { get; }

    /// <summary>
    /// Получить топ N трендовых ключевых слов
    /// </summary>
    Task<List<string>> GetTopKeywordsAsync(int count = 10);

    /// <summary>
    /// Получить трендовые ключевые слова по категории
    /// </summary>
    Task<List<string>> GetKeywordsByCategoryAsync(string category, int count = 10);
}
