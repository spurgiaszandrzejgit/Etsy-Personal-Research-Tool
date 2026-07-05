using EtsyAnalyzer.Core.Enums;
using EtsyAnalyzer.Core.Interfaces;
using EtsyAnalyzer.EtsyApi.Client;

namespace EtsyAnalyzer.Infrastructure.Providers;

/// <summary>
/// Провайдер трендовых ключевых слов из реального Etsy API
/// Извлекает популярные теги и поисковые запросы
/// </summary>
public class EtsyTrendingKeywordProvider : ITrendingKeywordProvider
{
    private readonly EtsyApiClient _apiClient;

    public string SourceName => "Etsy API (Real Trending Data)";
    public TrendingKeywordSource Source => TrendingKeywordSource.EtsyAutocomplete;

    public EtsyTrendingKeywordProvider(EtsyApiClient apiClient)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
    }

    public async Task<List<string>> GetTopKeywordsAsync(int count = 10)
    {
        try
        {
            Console.WriteLine($"[EtsyTrendingKeywordProvider] Fetching top {count} trending keywords from Etsy API...");
            var queries = await _apiClient.GetTrendingQueriesAsync(count);

            if (queries == null || queries.Count == 0)
            {
                Console.WriteLine("[EtsyTrendingKeywordProvider] WARNING: API returned empty list, using fallback keywords");
                return GetDefaultKeywords().Take(count).ToList();
            }

            Console.WriteLine($"[EtsyTrendingKeywordProvider] Successfully fetched {queries.Count} keywords from Etsy API");
            return queries;
        }
        catch (Exception ex)
        {
            // Fallback to default keywords if API fails
            Console.WriteLine($"[EtsyTrendingKeywordProvider] ERROR: Failed to get trending keywords from Etsy API");
            Console.WriteLine($"[EtsyTrendingKeywordProvider] Error: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine($"[EtsyTrendingKeywordProvider] Using fallback default keywords");
            return GetDefaultKeywords().Take(count).ToList();
        }
    }

    public async Task<List<string>> GetKeywordsByCategoryAsync(string category, int count = 10)
    {
        try
        {
            // Поиск по категории и извлечение популярных тегов
            var response = await _apiClient.SearchListingsAsync(category, Math.Min(count * 5, 100));

            var keywords = response.Results
                .SelectMany(l => l.Tags ?? new List<string>())
                .GroupBy(t => t.ToLower())
                .OrderByDescending(g => g.Count())
                .Take(count)
                .Select(g => g.Key)
                .ToList();

            return keywords;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Failed to get keywords for category '{category}': {ex.Message}");
            return GetDefaultKeywords().Take(count).ToList();
        }
    }

    private List<string> GetDefaultKeywords()
    {
        return new List<string>
        {
            "handmade jewelry",
            "custom gifts",
            "vintage decor",
            "personalized items",
            "wedding accessories",
            "home wall art",
            "printable digital",
            "boho style",
            "minimalist design",
            "custom portrait"
        };
    }
}
