namespace EtsyAnalyzer.Core.Entities;

public class Listing
{
    public long Id { get; set; }
    public long ListingId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CategoryPath { get; set; }
    public string Tags { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public long ShopId { get; set; }
    public string ShopName { get; set; } = string.Empty;
    public decimal? Rating { get; set; }
    public int ReviewCount { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? SearchQueryId { get; set; }

    public Shop? Shop { get; set; }
    public SearchQuery? SearchQuery { get; set; }
}
