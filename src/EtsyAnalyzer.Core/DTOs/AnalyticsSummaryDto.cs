using EtsyAnalyzer.Core.ValueObjects;

namespace EtsyAnalyzer.Core.DTOs;

public class AnalyticsSummaryDto
{
    public string Query { get; set; } = string.Empty;
    public int TotalListings { get; set; }
    public PriceStatisticsDto PriceStatistics { get; set; } = new();
    public List<KeywordFrequencyDto> TopKeywords { get; set; } = new();
    public List<KeywordFrequencyDto> TopTags { get; set; } = new();
    public List<ShopCompetitorDto> TopShops { get; set; } = new();
    public CompetitionLevel CompetitionLevel { get; set; }
    public NicheScore NicheScore { get; set; } = new(5.0m);
    public DateTime AnalyzedAt { get; set; }
}
