using System.Text.Json.Serialization;

namespace EtsyAnalyzer.EtsyApi.Models;

public class EtsySearchResponse<T>
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("results")]
    public List<T> Results { get; set; } = new();
}
