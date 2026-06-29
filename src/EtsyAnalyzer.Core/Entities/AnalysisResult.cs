namespace EtsyAnalyzer.Core.Entities;

public class AnalysisResult
{
    public long Id { get; set; }
    public long SearchQueryId { get; set; }
    public string Query { get; set; } = string.Empty;
    public int TotalListings { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal MedianPrice { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public string CompetitionLevel { get; set; } = string.Empty;
    public decimal NicheScore { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public DateTime AnalyzedAt { get; set; }

    public SearchQuery? SearchQuery { get; set; }
}
