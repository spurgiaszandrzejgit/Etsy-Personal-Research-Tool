using System.Text.Json.Serialization;

namespace EtsyAnalyzer.EtsyApi.Models;

public class EtsyListingResponse
{
    [JsonPropertyName("listing_id")]
    public long ListingId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public EtsyPriceResponse? Price { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("taxonomy_path")]
    public List<string>? TaxonomyPath { get; set; }

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("shop_id")]
    public long ShopId { get; set; }

    [JsonPropertyName("user_id")]
    public long UserId { get; set; }

    [JsonPropertyName("num_favorers")]
    public int NumFavorers { get; set; }

    [JsonPropertyName("views")]
    public int? Views { get; set; }

    [JsonPropertyName("images")]
    public List<EtsyImageResponse>? Images { get; set; }
}

public class EtsyPriceResponse
{
    [JsonPropertyName("amount")]
    public int Amount { get; set; } // В центах/копейках

    [JsonPropertyName("divisor")]
    public int Divisor { get; set; } // Обычно 100

    [JsonPropertyName("currency_code")]
    public string CurrencyCode { get; set; } = "USD";

    public decimal GetDecimalPrice() => (decimal)Amount / Divisor;
}

public class EtsyImageResponse
{
    [JsonPropertyName("url_75x75")]
    public string? Url75x75 { get; set; }

    [JsonPropertyName("url_170x135")]
    public string? Url170x135 { get; set; }

    [JsonPropertyName("url_570xN")]
    public string? Url570xN { get; set; }

    [JsonPropertyName("url_fullxfull")]
    public string? UrlFullxFull { get; set; }
}
