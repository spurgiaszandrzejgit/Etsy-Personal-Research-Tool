using EtsyAnalyzer.Core.Enums;

namespace EtsyAnalyzer.Core.DTOs;

/// <summary>
/// Сводка анализа трендовых ключевых слов
/// </summary>
public class TrendingKeywordSummary
{
    public TrendingKeywordSource Source { get; set; }
    public DateTime AnalyzedAt { get; set; }
    public int TotalKeywordsAnalyzed { get; set; }
    public List<TrendingKeywordResult> Results { get; set; } = new();

    // Статистика
    public int RecommendedCount => Results.Count(r => r.Recommendation == "✅ Recommended");
    public int NotRecommendedCount => Results.Count(r => r.Recommendation == "⚠️ Risky");
    public TrendingKeywordResult? BestOpportunity => Results
        .Where(r => r.Recommendation == "✅ Recommended")
        .OrderByDescending(r => r.NicheScore)
        .FirstOrDefault();
}
