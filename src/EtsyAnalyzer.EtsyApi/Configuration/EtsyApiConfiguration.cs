namespace EtsyAnalyzer.EtsyApi.Configuration;

public class EtsyApiConfiguration
{
    public const string SectionName = "Etsy";

    public string ApiKey { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = "https://openapi.etsy.com/v3";
    public int RateLimitPerMinute { get; set; } = 10;
    public int TimeoutSeconds { get; set; } = 30;
}
