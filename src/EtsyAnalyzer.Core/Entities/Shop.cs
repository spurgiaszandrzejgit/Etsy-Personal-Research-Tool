namespace EtsyAnalyzer.Core.Entities;

public class Shop
{
    public long Id { get; set; }
    public long ShopId { get; set; }
    public string ShopName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int ListingCount { get; set; }
    public decimal? Rating { get; set; }
    public int ReviewCount { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
}
