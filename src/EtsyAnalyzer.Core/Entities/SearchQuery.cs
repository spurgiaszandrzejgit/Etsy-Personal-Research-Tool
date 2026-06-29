namespace EtsyAnalyzer.Core.Entities;

public class SearchQuery
{
    public long Id { get; set; }
    public string Query { get; set; } = string.Empty;
    public DateTime ExecutedAt { get; set; }
    public int ResultCount { get; set; }
    public string? DataSource { get; set; }

    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
}
