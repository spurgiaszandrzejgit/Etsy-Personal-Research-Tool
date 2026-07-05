using System.Net.Http.Json;
using System.Text.Json;
using EtsyAnalyzer.Core.Exceptions;
using EtsyAnalyzer.EtsyApi.Configuration;
using EtsyAnalyzer.EtsyApi.Models;
using Microsoft.Extensions.Options;

namespace EtsyAnalyzer.EtsyApi.Client;

public class EtsyApiClient
{
    private readonly HttpClient _httpClient;
    private readonly EtsyApiConfiguration _configuration;
    private readonly JsonSerializerOptions _jsonOptions;

    public EtsyApiClient(HttpClient httpClient, IOptions<EtsyApiConfiguration> configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        ConfigureHttpClient();
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_configuration.ApiUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_configuration.TimeoutSeconds);
        _httpClient.DefaultRequestHeaders.Add("x-api-key", _configuration.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "EtsyAnalyzer/1.0");
    }

    public async Task<EtsySearchResponse<EtsyListingResponse>> SearchListingsAsync(
        string query, 
        int limit = 100, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var requestUrl = $"v3/application/listings/active?keywords={Uri.EscapeDataString(query)}&limit={Math.Min(limit, 100)}";

            var response = await _httpClient.GetAsync(requestUrl, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new DataSourceException(
                    $"Etsy API request failed with status {response.StatusCode}: {errorContent}",
                    "EtsyAPI");
            }

            var result = await response.Content.ReadFromJsonAsync<EtsySearchResponse<EtsyListingResponse>>(
                _jsonOptions, 
                cancellationToken);

            return result ?? new EtsySearchResponse<EtsyListingResponse>();
        }
        catch (HttpRequestException ex)
        {
            throw new DataSourceException("Network error while calling Etsy API", "EtsyAPI", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new DataSourceException("Etsy API request timeout", "EtsyAPI", ex);
        }
        catch (JsonException ex)
        {
            throw new DataSourceException("Failed to parse Etsy API response", "EtsyAPI", ex);
        }
    }

    public async Task<EtsyShopResponse?> GetShopByIdAsync(
        long shopId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var requestUrl = $"v3/application/shops/{shopId}";

            var response = await _httpClient.GetAsync(requestUrl, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new DataSourceException(
                    $"Etsy API request failed with status {response.StatusCode}: {errorContent}",
                    "EtsyAPI");
            }

            return await response.Content.ReadFromJsonAsync<EtsyShopResponse>(
                _jsonOptions, 
                cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            throw new DataSourceException("Network error while calling Etsy API", "EtsyAPI", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new DataSourceException("Etsy API request timeout", "EtsyAPI", ex);
        }
        catch (JsonException ex)
        {
            throw new DataSourceException("Failed to parse Etsy API response", "EtsyAPI", ex);
        }
    }

    /// <summary>
    /// Получить трендовые поисковые запросы из Etsy
    /// Note: Etsy API v3 может не иметь прямого endpoint для trending queries.
    /// Как альтернатива, можно получить популярные запросы через taxonomy или использовать автодополнение.
    /// </summary>
    public async Task<List<string>> GetTrendingQueriesAsync(int limit = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            Console.WriteLine($"[EtsyApiClient] Fetching trending queries (limit: {limit})...");

            // Базовые популярные категории Etsy для получения трендовых запросов
            var popularCategories = new List<string>
            {
                "jewelry", "home decor", "art", "clothing", "accessories",
                "wedding", "craft supplies", "vintage", "gifts", "personalized"
            };

            var trendingQueries = new List<string>();

            // Для каждой категории получаем топ листинги и извлекаем популярные теги
            foreach (var category in popularCategories.Take(5))
            {
                try
                {
                    Console.WriteLine($"[EtsyApiClient] Searching category: {category}...");
                    var response = await SearchListingsAsync(category, 20, cancellationToken);

                    if (response?.Results == null || response.Results.Count == 0)
                    {
                        Console.WriteLine($"[EtsyApiClient] No results for category: {category}");
                        continue;
                    }

                    Console.WriteLine($"[EtsyApiClient] Found {response.Results.Count} listings in {category}");

                    // Извлекаем популярные теги из листингов
                    var tags = response.Results
                        .SelectMany(l => l.Tags ?? new List<string>())
                        .GroupBy(t => t.ToLower())
                        .OrderByDescending(g => g.Count())
                        .Take(3)
                        .Select(g => g.Key)
                        .Where(t => t.Length > 3 && !trendingQueries.Contains(t));

                    var newTags = tags.ToList();
                    Console.WriteLine($"[EtsyApiClient] Extracted {newTags.Count} popular tags from {category}");
                    trendingQueries.AddRange(newTags);

                    if (trendingQueries.Count >= limit)
                    {
                        Console.WriteLine($"[EtsyApiClient] Reached limit of {limit} keywords");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    // Пропускаем ошибки для отдельных категорий
                    Console.WriteLine($"[EtsyApiClient] Error searching category {category}: {ex.Message}");
                    continue;
                }
            }

            var result = trendingQueries.Distinct().Take(limit).ToList();
            Console.WriteLine($"[EtsyApiClient] Returning {result.Count} trending keywords");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EtsyApiClient] FATAL ERROR in GetTrendingQueriesAsync: {ex.GetType().Name} - {ex.Message}");
            throw new DataSourceException("Failed to get trending queries from Etsy", "EtsyAPI", ex);
        }
    }
}
