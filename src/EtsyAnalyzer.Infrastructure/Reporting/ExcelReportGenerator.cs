using ClosedXML.Excel;
using EtsyAnalyzer.Core.DTOs;
using EtsyAnalyzer.Core.Interfaces;
using EtsyAnalyzer.Infrastructure.Reporting.Formatters;
using EtsyAnalyzer.Infrastructure.Reporting.Builders;

namespace EtsyAnalyzer.Infrastructure.Reporting;

public class ExcelReportGenerator : IReportGenerator
{
    private readonly SummarySheetFormatter _summaryFormatter;
    private readonly ListingsSheetFormatter _listingsFormatter;
    private readonly ShopsSheetFormatter _shopsFormatter;
    private readonly KeywordsSheetFormatter _keywordsFormatter;
    private readonly ActionPlanWorksheetBuilder _actionPlanBuilder;

    public ExcelReportGenerator()
    {
        _summaryFormatter = new SummarySheetFormatter();
        _listingsFormatter = new ListingsSheetFormatter();
        _shopsFormatter = new ShopsSheetFormatter();
        _keywordsFormatter = new KeywordsSheetFormatter();
        _actionPlanBuilder = new ActionPlanWorksheetBuilder();
    }

    public string ReportType => "Excel";

    public async Task<string> GenerateReportAsync(
        AnalyticsSummaryDto summary,
        IEnumerable<ListingDto> listings,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            using var workbook = new XLWorkbook();

            // 1. Summary Sheet
            var summarySheet = workbook.Worksheets.Add("Summary");
            _summaryFormatter.FormatSheet(summarySheet, summary);

            // 2. Listings Sheet
            var listingsSheet = workbook.Worksheets.Add("Listings");
            _listingsFormatter.FormatSheet(listingsSheet, listings);

            // 3. Shops Sheet
            var shopsSheet = workbook.Worksheets.Add("Shops");
            _shopsFormatter.FormatSheet(shopsSheet, summary.TopShops);

            // 4. Keywords Sheet
            var keywordsSheet = workbook.Worksheets.Add("Keywords");
            _keywordsFormatter.FormatSheet(keywordsSheet, summary);

            // 5. Action Plan Sheet (NEW!)
            _actionPlanBuilder.Build(workbook, summary);

            // Ensure directory exists
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Save workbook
            workbook.SaveAs(outputPath);

            return outputPath;
        }, cancellationToken);
    }
}
