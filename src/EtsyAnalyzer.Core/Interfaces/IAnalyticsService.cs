using EtsyAnalyzer.Core.DTOs;

namespace EtsyAnalyzer.Core.Interfaces;

/// <summary>
/// Сервис для выполнения аналитики ниши
/// </summary>
public interface IAnalyticsService
{
    /// <summary>
    /// Выполнить полный анализ ниши по поисковому запросу
    /// </summary>
    Task<AnalyticsSummaryDto> AnalyzeNicheAsync(string query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Выполнить анализ по уже загруженным данным из БД
    /// </summary>
    Task<AnalyticsSummaryDto> AnalyzeExistingDataAsync(long searchQueryId, CancellationToken cancellationToken = default);
}
