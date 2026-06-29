using EtsyAnalyzer.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EtsyAnalyzer.Infrastructure.Persistence;

public class EtsyAnalyzerDbContext : DbContext
{
    public EtsyAnalyzerDbContext(DbContextOptions<EtsyAnalyzerDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Listing> Listings => Set<Listing>();
    public DbSet<Shop> Shops => Set<Shop>();
    public DbSet<SearchQuery> SearchQueries => Set<SearchQuery>();
    public DbSet<AnalysisResult> AnalysisResults => Set<AnalysisResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EtsyAnalyzerDbContext).Assembly);
    }
}
