using EtsyAnalyzer.Core.Enums;
using EtsyAnalyzer.Core.Interfaces;

namespace EtsyAnalyzer.Infrastructure.Providers;

/// <summary>
/// Моковый провайдер трендовых ключевых слов для тестирования
/// </summary>
public class MockTrendingKeywordProvider : ITrendingKeywordProvider
{
    public string SourceName => "Mock Data";
    public TrendingKeywordSource Source => TrendingKeywordSource.Mock;

    private static readonly List<string> MockKeywords = new()
    {
        "personalized gifts",
        "custom wedding invitations",
        "handmade jewelry",
        "vintage clothing",
        "printable wall art",
        "custom pet portrait",
        "boho home decor",
        "minimalist jewelry",
        "digital planner",
        "custom name necklace",
        "watercolor print",
        "macrame wall hanging",
        "leather wallet",
        "custom stickers",
        "wedding guest book",
        "baby shower decorations",
        "custom phone case",
        "hand lettered signs",
        "resin art",
        "crochet blanket"
    };

    private static readonly Dictionary<string, List<string>> KeywordsByCategory = new()
    {
        ["Jewelry"] = new() { "custom name necklace", "minimalist jewelry", "birthstone ring", "personalized bracelet" },
        ["Home Decor"] = new() { "boho home decor", "macrame wall hanging", "printable wall art", "custom signs" },
        ["Wedding"] = new() { "custom wedding invitations", "wedding guest book", "bridal shower gifts" },
        ["Baby"] = new() { "baby shower decorations", "personalized baby blanket", "custom baby onesie" },
        ["Art"] = new() { "watercolor print", "resin art", "digital download", "custom portrait" }
    };

    public Task<List<string>> GetTopKeywordsAsync(int count = 10)
    {
        var keywords = MockKeywords.Take(count).ToList();
        return Task.FromResult(keywords);
    }

    public Task<List<string>> GetKeywordsByCategoryAsync(string category, int count = 10)
    {
        if (KeywordsByCategory.TryGetValue(category, out var keywords))
        {
            return Task.FromResult(keywords.Take(count).ToList());
        }

        return Task.FromResult(new List<string>());
    }
}
