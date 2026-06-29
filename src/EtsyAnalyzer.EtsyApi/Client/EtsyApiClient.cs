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
}
