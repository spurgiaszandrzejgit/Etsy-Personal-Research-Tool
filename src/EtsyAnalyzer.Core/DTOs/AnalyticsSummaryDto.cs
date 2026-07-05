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

    // Вспомогательные свойства для удобства доступа в отчетах
    public List<ListingDto> Listings { get; set; } = new();
    public int UniqueShops => Listings.Select(l => l.ShopName).Distinct().Count();
    public string SearchQuery => Query;
    public decimal AveragePrice => PriceStatistics.Average;
    public decimal MedianPrice => PriceStatistics.Median;
    public decimal MinPrice => PriceStatistics.Min;
    public decimal MaxPrice => PriceStatistics.Max;
    public string CurrencySymbol => GetCurrencySymbol(PriceStatistics.CurrencyCode);
    public decimal? StandardDeviation => PriceStatistics.StandardDeviation;

    private static string GetCurrencySymbol(string currencyCode)
    {
        return currencyCode switch
        {
            "USD" => "$",
            "EUR" => "€",
            "GBP" => "£",
            "CAD" => "C$",
            "AUD" => "A$",
            _ => currencyCode
        };
    }
}
