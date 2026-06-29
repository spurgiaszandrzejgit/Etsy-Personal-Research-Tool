using EtsyAnalyzer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtsyAnalyzer.Infrastructure.Persistence.Configurations;

public class AnalysisResultConfiguration : IEntityTypeConfiguration<AnalysisResult>
{
    public void Configure(EntityTypeBuilder<AnalysisResult> builder)
    {
        builder.ToTable("AnalysisResults");

        builder.HasKey(ar => ar.Id);

        builder.Property(ar => ar.Query)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(ar => ar.AveragePrice)
            .HasPrecision(18, 2);

        builder.Property(ar => ar.MedianPrice)
            .HasPrecision(18, 2);

        builder.Property(ar => ar.MinPrice)
            .HasPrecision(18, 2);

        builder.Property(ar => ar.MaxPrice)
            .HasPrecision(18, 2);

        builder.Property(ar => ar.CompetitionLevel)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(ar => ar.NicheScore)
            .HasPrecision(3, 1);

        builder.Property(ar => ar.CurrencyCode)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(ar => ar.AnalyzedAt)
            .IsRequired();

        builder.HasOne(ar => ar.SearchQuery)
            .WithMany()
            .HasForeignKey(ar => ar.SearchQueryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
