using EtsyAnalyzer.Core.ValueObjects;

namespace EtsyAnalyzer.Core.DTOs;

/// <summary>
/// Результат анализа одного трендового ключевого слова
/// </summary>
public class TrendingKeywordResult
{
    public string Keyword { get; set; } = string.Empty;
    public int TotalListings { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal MedianPrice { get; set; }
    public CompetitionLevel CompetitionLevel { get; set; }
    public decimal NicheScore { get; set; }
    public string CurrencyCode { get; set; } = "USD";

    // Рекомендации
    public string Recommendation { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string WhyInteresting { get; set; } = string.Empty;
    public List<string> Risks { get; set; } = new();

    // Топ теги и категории
    public List<string> TopTags { get; set; } = new();
    public List<string> TopCategories { get; set; } = new();
}
