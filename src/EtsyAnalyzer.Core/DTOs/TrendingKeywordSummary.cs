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

    // Lists for categorized results (filled by analysis service)
    public List<TrendingKeywordResult> BestOpportunities { get; set; } = new();
    public List<TrendingKeywordResult> HighRiskNiches { get; set; } = new();

    // Статистика
    public int RecommendedCount => Results.Count(r => r.Recommendation == "✅ Recommended");
    public int NotRecommendedCount => Results.Count(r => r.Recommendation == "⚠️ Risky" || r.Recommendation == "⚠️ High Risk");
    public TrendingKeywordResult? BestOpportunity => Results
        .Where(r => r.Recommendation == "✅ Recommended")
        .OrderByDescending(r => r.NicheScore)
        .FirstOrDefault();
}
