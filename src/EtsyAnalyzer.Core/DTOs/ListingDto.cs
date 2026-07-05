namespace EtsyAnalyzer.Core.DTOs;

public class ListingDto
{
    public long ListingId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public string Description { get; set; } = string.Empty;
    public string? CategoryPath { get; set; }
    public List<string> Tags { get; set; } = new();
    public string Url { get; set; } = string.Empty;
    public long ShopId { get; set; }
    public string ShopName { get; set; } = string.Empty;
    public decimal? Rating { get; set; }
    public int ReviewCount { get; set; }
    public string? ImageUrl { get; set; }

    // Helper property
    public string CurrencySymbol => CurrencyCode switch
    {
        "USD" => "$",
        "EUR" => "€",
        "GBP" => "£",
        "CAD" => "C$",
        "AUD" => "A$",
        _ => CurrencyCode
    };
}
