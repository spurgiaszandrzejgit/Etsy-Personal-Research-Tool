using System.Text.Json.Serialization;

namespace EtsyAnalyzer.EtsyApi.Models;

/// <summary>
/// Ответ Etsy API для поисковых подсказок (trending queries)
/// </summary>
public class EtsyTrendingQueriesResponse
{
    [JsonPropertyName("queries")]
    public List<TrendingQuery> Queries { get; set; } = new();
}

public class TrendingQuery
{
    [JsonPropertyName("query")]
    public string Query { get; set; } = string.Empty;

    [JsonPropertyName("score")]
    public double? Score { get; set; }
}
