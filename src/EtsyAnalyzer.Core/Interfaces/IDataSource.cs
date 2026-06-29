using EtsyAnalyzer.Core.DTOs;

namespace EtsyAnalyzer.Core.Interfaces;

/// <summary>
/// Абстракция для разных источников данных (Etsy API, Web Scraper, Mock и т.д.)
/// Реализует Strategy Pattern для возможности подключения новых источников
/// </summary>
public interface IDataSource
{
    /// <summary>
    /// Название источника данных
    /// </summary>
    string SourceName { get; }

    /// <summary>
    /// Поиск товаров по запросу
    /// </summary>
    Task<IEnumerable<ListingDto>> SearchListingsAsync(string query, int limit = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение детальной информации о магазине
    /// </summary>
    Task<ShopDto?> GetShopDetailsAsync(long shopId, CancellationToken cancellationToken = default);
}
