using EtsyAnalyzer.Core.DTOs;
using EtsyAnalyzer.Core.Interfaces;

namespace EtsyAnalyzer.Analytics.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IDataSource _dataSource;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PriceAnalyzer _priceAnalyzer;
    private readonly KeywordAnalyzer _keywordAnalyzer;
    private readonly CompetitionAnalyzer _competitionAnalyzer;
    private readonly NicheScoreCalculator _nicheScoreCalculator;

    public AnalyticsService(
        IDataSource dataSource,
        IUnitOfWork unitOfWork,
        PriceAnalyzer priceAnalyzer,
        KeywordAnalyzer keywordAnalyzer,
        CompetitionAnalyzer competitionAnalyzer,
        NicheScoreCalculator nicheScoreCalculator)
    {
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _priceAnalyzer = priceAnalyzer ?? throw new ArgumentNullException(nameof(priceAnalyzer));
        _keywordAnalyzer = keywordAnalyzer ?? throw new ArgumentNullException(nameof(keywordAnalyzer));
        _competitionAnalyzer = competitionAnalyzer ?? throw new ArgumentNullException(nameof(competitionAnalyzer));
        _nicheScoreCalculator = nicheScoreCalculator ?? throw new ArgumentNullException(nameof(nicheScoreCalculator));
    }

    public async Task<AnalyticsSummaryDto> AnalyzeNicheAsync(string query, CancellationToken cancellationToken = default)
    {
        // 1. Получаем данные из источника
        var listings = (await _dataSource.SearchListingsAsync(query, 200, cancellationToken)).ToList();

        if (!listings.Any())
        {
            throw new InvalidOperationException($"No listings found for query: {query}");
        }

        // 2. Сохраняем в БД
        var searchQuery = new Core.Entities.SearchQuery
        {
            Query = query,
            ExecutedAt = DateTime.UtcNow,
            ResultCount = listings.Count,
            DataSource = _dataSource.SourceName
        };

        await _unitOfWork.SearchQueries.AddAsync(searchQuery, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Track shops already processed in this session to avoid duplicate tracking
        var processedShopIds = new HashSet<long>();

        // Преобразуем DTOs в entities и сохраняем
        foreach (var listingDto in listings)
        {
            // Проверяем/создаём магазин (только если ещё не обработан в этой сессии)
            if (!processedShopIds.Contains(listingDto.ShopId))
            {
                var existingShop = await _unitOfWork.Shops.GetByShopIdAsync(listingDto.ShopId, cancellationToken);
                if (existingShop == null)
                {
                    var newShop = new Core.Entities.Shop
                    {
                        ShopId = listingDto.ShopId,
                        ShopName = listingDto.ShopName,
                        Url = $"https://www.etsy.com/shop/{listingDto.ShopName}",
                        ListingCount = 0,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.Shops.AddAsync(newShop, cancellationToken);
                }
                processedShopIds.Add(listingDto.ShopId);
            }

            // Создаём listing (только если не существует)
            var existingListing = await _unitOfWork.Listings.GetByListingIdAsync(listingDto.ListingId, cancellationToken);
            if (existingListing == null)
            {
                var listing = new Core.Entities.Listing
                {
                    ListingId = listingDto.ListingId,
                    Title = listingDto.Title,
                    Price = listingDto.Price,
                    CurrencyCode = listingDto.CurrencyCode,
                    Description = listingDto.Description,
                    CategoryPath = listingDto.CategoryPath,
                    Tags = string.Join(", ", listingDto.Tags),
                    Url = listingDto.Url,
                    ShopId = listingDto.ShopId,
                    ShopName = listingDto.ShopName,
                    Rating = listingDto.Rating,
                    ReviewCount = listingDto.ReviewCount,
                    ImageUrl = listingDto.ImageUrl,
                    CreatedAt = DateTime.UtcNow,
                    SearchQueryId = searchQuery.Id
                };

                await _unitOfWork.Listings.AddAsync(listing, cancellationToken);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 3. Выполняем анализ
        var summary = PerformAnalysis(query, listings);

        return summary;
    }

    public async Task<AnalyticsSummaryDto> AnalyzeExistingDataAsync(long searchQueryId, CancellationToken cancellationToken = default)
    {
        var searchQuery = await _unitOfWork.SearchQueries.GetByIdAsync(searchQueryId, cancellationToken);

        if (searchQuery == null)
            throw new InvalidOperationException($"Search query with ID {searchQueryId} not found");

        var listingsEntities = await _unitOfWork.Listings.GetBySearchQueryIdAsync(searchQueryId, cancellationToken);

        // Преобразуем entities в DTOs
        var listings = listingsEntities.Select(l => new ListingDto
        {
            ListingId = l.ListingId,
            Title = l.Title,
            Price = l.Price,
            CurrencyCode = l.CurrencyCode,
            Description = l.Description,
            CategoryPath = l.CategoryPath,
            Tags = l.Tags.Split(", ", StringSplitOptions.RemoveEmptyEntries).ToList(),
            Url = l.Url,
            ShopId = l.ShopId,
            ShopName = l.ShopName,
            Rating = l.Rating,
            ReviewCount = l.ReviewCount,
            ImageUrl = l.ImageUrl
        }).ToList();

        return PerformAnalysis(searchQuery.Query, listings);
    }

    private AnalyticsSummaryDto PerformAnalysis(string query, List<ListingDto> listings)
    {
        var priceStats = _priceAnalyzer.AnalyzePrices(listings);
        var topKeywords = _keywordAnalyzer.AnalyzeKeywords(listings, 20);
        var topTags = _keywordAnalyzer.AnalyzeTags(listings, 20);
        var topShops = _competitionAnalyzer.GetTopCompetitors(listings, 10);
        var competitionLevel = _competitionAnalyzer.AnalyzeCompetition(listings.Count);

        var summary = new AnalyticsSummaryDto
        {
            Query = query,
            TotalListings = listings.Count,
            PriceStatistics = priceStats,
            TopKeywords = topKeywords,
            TopTags = topTags,
            TopShops = topShops,
            CompetitionLevel = competitionLevel,
            AnalyzedAt = DateTime.UtcNow
        };

        // Рассчитываем итоговую оценку ниши
        summary.NicheScore = _nicheScoreCalculator.CalculateScore(summary);

        return summary;
    }
}
