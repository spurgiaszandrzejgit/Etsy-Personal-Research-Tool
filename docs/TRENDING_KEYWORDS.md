# Trending Keywords Feature - Technical Notes

## How It Works

### Data Flow

1. **Keyword Source**: `EtsyTrendingKeywordProvider`
   - Queries Etsy API for popular categories (jewelry, home decor, art, clothing, accessories)
   - Fetches top 20 listings per category
   - Extracts most frequently used tags
   - Returns top N unique trending keywords

2. **Keyword Analysis**: `TrendingKeywordAnalysisService`
   - Takes each trending keyword
   - Performs full niche analysis via Etsy API
   - Calculates competition, pricing, niche score
   - Generates recommendations

3. **Report Generation**: `TrendingKeywordReportGenerator`
   - Creates Excel workbook with 4 sheets:
	 - Overview (summary statistics)
	 - Best Opportunities (recommended niches)
	 - Risky Niches (high competition/low score)
	 - Detailed Results (full analysis table)

### Configuration

In `appsettings.json`:

```json
{
  "TrendingKeywords": {
	"UseRealEtsyProvider": true,  // true = real Etsy API, false = mock data
	"Provider": "EtsyAPI"
  }
}
```

### Logging

Console output shows:
- `[EtsyApiClient]` - API requests and responses per category
- `[EtsyTrendingKeywordProvider]` - Success/failure and fallback status
- Category search progress
- Number of listings and tags extracted
- Final keyword count

### Fallback Behavior

If Etsy API fails:
- Provider falls back to 10 default keywords
- Analysis still uses real Etsy API
- Console shows warning about fallback usage

### Example Console Output

```
[EtsyTrendingKeywordProvider] Fetching top 10 trending keywords from Etsy API...
[EtsyApiClient] Fetching trending queries (limit: 10)...
[EtsyApiClient] Searching category: jewelry...
[EtsyApiClient] Found 20 listings in jewelry
[EtsyApiClient] Extracted 3 popular tags from jewelry
[EtsyApiClient] Searching category: home decor...
[EtsyApiClient] Found 20 listings in home decor
[EtsyApiClient] Extracted 3 popular tags from home decor
[EtsyApiClient] Returning 10 trending keywords
[EtsyTrendingKeywordProvider] Successfully fetched 10 keywords from Etsy API
```

## Troubleshooting

**Problem**: "Total Keywords = 0" in report

**Possible causes**:
1. API rate limit exceeded
2. Invalid API key
3. Network connectivity issues
4. Etsy API temporary unavailability

**Solution**:
- Check console logs for `[EtsyApiClient]` errors
- Verify API key in `appsettings.json`
- Check fallback keywords are being used
- Try again after a few minutes (rate limit reset)
