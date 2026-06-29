namespace EtsyAnalyzer.Core.DTOs;

public class ShopCompetitorDto
{
    public long ShopId { get; set; }
    public string ShopName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int ListingCount { get; set; }
    public decimal? Rating { get; set; }
    public int ReviewCount { get; set; }
    public int ListingsInSearch { get; set; }
}
