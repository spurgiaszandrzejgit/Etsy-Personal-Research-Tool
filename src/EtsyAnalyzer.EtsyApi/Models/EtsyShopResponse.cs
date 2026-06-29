using System.Text.Json.Serialization;

namespace EtsyAnalyzer.EtsyApi.Models;

public class EtsyShopResponse
{
    [JsonPropertyName("shop_id")]
    public long ShopId { get; set; }

    [JsonPropertyName("shop_name")]
    public string ShopName { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("listing_active_count")]
    public int ListingActiveCount { get; set; }

    [JsonPropertyName("digital_listing_count")]
    public int DigitalListingCount { get; set; }

    [JsonPropertyName("num_favorers")]
    public int NumFavorers { get; set; }
}
