using EtsyAnalyzer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtsyAnalyzer.Infrastructure.Persistence.Configurations;

public class SearchQueryConfiguration : IEntityTypeConfiguration<SearchQuery>
{
    public void Configure(EntityTypeBuilder<SearchQuery> builder)
    {
        builder.ToTable("SearchQueries");

        builder.HasKey(sq => sq.Id);

        builder.Property(sq => sq.Query)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(sq => sq.Query);

        builder.Property(sq => sq.ExecutedAt)
            .IsRequired();

        builder.Property(sq => sq.DataSource)
            .HasMaxLength(100);

        builder.HasMany(sq => sq.Listings)
            .WithOne(l => l.SearchQuery)
            .HasForeignKey(l => l.SearchQueryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
