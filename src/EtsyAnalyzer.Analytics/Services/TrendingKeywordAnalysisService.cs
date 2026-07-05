using EtsyAnalyzer.Core.DTOs;
using EtsyAnalyzer.Core.Interfaces;
using EtsyAnalyzer.Core.ValueObjects;

namespace EtsyAnalyzer.Analytics.Services;

/// <summary>
/// Сервис анализа трендовых ключевых слов
/// </summary>
public class TrendingKeywordAnalysisService : ITrendingKeywordAnalysisService
{
    private readonly ITrendingKeywordProvider _keywordProvider;
    private readonly IAnalyticsService _analyticsService;

    public TrendingKeywordAnalysisService(
        ITrendingKeywordProvider keywordProvider,
        IAnalyticsService analyticsService)
    {
        _keywordProvider = keywordProvider ?? throw new ArgumentNullException(nameof(keywordProvider));
        _analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));
    }

    public async Task<TrendingKeywordSummary> AnalyzeTopKeywordsAsync(int count = 10)
    {
        var keywords = await _keywordProvider.GetTopKeywordsAsync(count);
        return await AnalyzeKeywordsInternalAsync(keywords);
    }

    public async Task<TrendingKeywordSummary> AnalyzeCustomKeywordsAsync(List<string> keywords)
    {
        return await AnalyzeKeywordsInternalAsync(keywords);
    }

    private async Task<TrendingKeywordSummary> AnalyzeKeywordsInternalAsync(List<string> keywords)
    {
        var summary = new TrendingKeywordSummary
        {
            Source = _keywordProvider.Source,
            AnalyzedAt = DateTime.UtcNow,
            TotalKeywordsAnalyzed = keywords.Count
        };

        foreach (var keyword in keywords)
        {
            try
            {
                Console.WriteLine($"[TrendingAnalysis] Analyzing keyword: '{keyword}'...");

                // Analyze the keyword
                var analysis = await _analyticsService.AnalyzeNicheAsync(keyword);

                Console.WriteLine($"[TrendingAnalysis] Keyword '{keyword}' analyzed successfully. Listings: {analysis.TotalListings}");

                // Convert to TrendingKeywordResult
                var result = ConvertToTrendingResult(keyword, analysis);
                summary.Results.Add(result);
            }
            catch (Exception ex)
            {
                // Log error but continue with other keywords
                Console.WriteLine($"[TrendingAnalysis] ERROR analyzing '{keyword}': {ex.GetType().Name} - {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[TrendingAnalysis] Inner exception: {ex.InnerException.Message}");
                }
            }
        }

        // Categorize results into best opportunities and risky niches
        summary.BestOpportunities = summary.Results
            .Where(r => r.Recommendation == "✅ Recommended")
            .OrderByDescending(r => r.NicheScore)
            .ToList();

        summary.HighRiskNiches = summary.Results
            .Where(r => r.Recommendation == "⚠️ High Risk")
            .OrderByDescending(r => r.NicheScore)
            .ToList();

        return summary;
    }

    private TrendingKeywordResult ConvertToTrendingResult(string keyword, AnalyticsSummaryDto analysis)
    {
        var result = new TrendingKeywordResult
        {
            Keyword = keyword,
            TotalListings = analysis.TotalListings,
            AveragePrice = analysis.AveragePrice,
            MedianPrice = analysis.MedianPrice,
            CompetitionLevel = analysis.CompetitionLevel,
            NicheScore = analysis.NicheScore.Value,
            CurrencyCode = analysis.PriceStatistics.CurrencyCode,
            TopTags = analysis.TopTags.Take(5).Select(t => t.Keyword).ToList(),
            TopCategories = new List<string>() // Could extract from listings if needed
        };

        // Generate recommendation
        GenerateRecommendation(result, analysis);

        return result;
    }

    private void GenerateRecommendation(TrendingKeywordResult result, AnalyticsSummaryDto analysis)
    {
        // Determine recommendation based on score and competition
        if (result.NicheScore >= 7.0m && result.CompetitionLevel == CompetitionLevel.Low)
        {
            result.Recommendation = "✅ Recommended";
            result.Reason = "High score with low competition - excellent opportunity";
        }
        else if (result.NicheScore >= 5.0m && result.CompetitionLevel != CompetitionLevel.High)
        {
            result.Recommendation = "⚠️ Consider";
            result.Reason = "Moderate opportunity - requires careful positioning";
        }
        else
        {
            result.Recommendation = "⚠️ Risky";
            result.Reason = "High competition or low score - challenging market";
        }

        // Generate "Why Interesting"
        var reasons = new List<string>();

        if (result.AveragePrice > 30)
            reasons.Add($"Good price point (avg ${result.AveragePrice:F2})");

        if (result.TotalListings > 100 && result.TotalListings < 10000)
            reasons.Add("Healthy market size");

        if (result.CompetitionLevel == CompetitionLevel.Low)
            reasons.Add("Low competition");

        result.WhyInteresting = reasons.Any() 
            ? string.Join(", ", reasons) 
            : "Standard market conditions";

        // Generate risks
        var risks = new List<string>();

        if (result.CompetitionLevel == CompetitionLevel.High)
            risks.Add("High competition - difficult to rank");

        if (result.TotalListings < 50)
            risks.Add("Very small market - low demand");

        if (result.AveragePrice < 10)
            risks.Add("Low price point - thin margins");

        result.Risks = risks;
    }
}
