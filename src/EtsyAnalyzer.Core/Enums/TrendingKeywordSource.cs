namespace EtsyAnalyzer.Core.Enums;

/// <summary>
/// Источник трендовых ключевых слов
/// </summary>
public enum TrendingKeywordSource
{
    /// <summary>
    /// Моковые данные для тестирования
    /// </summary>
    Mock,

    /// <summary>
    /// Etsy автокомплит
    /// </summary>
    EtsyAutocomplete,

    /// <summary>
    /// eRank trending
    /// </summary>
    ERank,

    /// <summary>
    /// Google Trends
    /// </summary>
    GoogleTrends
}
