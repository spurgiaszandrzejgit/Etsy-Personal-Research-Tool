using EtsyAnalyzer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtsyAnalyzer.Infrastructure.Persistence.Configurations;

public class ListingConfiguration : IEntityTypeConfiguration<Listing>
{
    public void Configure(EntityTypeBuilder<Listing> builder)
    {
        builder.ToTable("Listings");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.ListingId)
            .IsRequired();

        builder.HasIndex(l => l.ListingId)
            .IsUnique();

        builder.Property(l => l.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(l => l.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(l => l.CurrencyCode)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(l => l.Description)
            .HasMaxLength(5000);

        builder.Property(l => l.CategoryPath)
            .HasMaxLength(500);

        builder.Property(l => l.Tags)
            .HasMaxLength(2000);

        builder.Property(l => l.Url)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(l => l.ShopId)
            .IsRequired();

        builder.Property(l => l.ShopName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.Rating)
            .HasPrecision(3, 2);

        builder.Property(l => l.ImageUrl)
            .HasMaxLength(1000);

        builder.Property(l => l.CreatedAt)
            .IsRequired();

        builder.HasOne(l => l.Shop)
            .WithMany(s => s.Listings)
            .HasForeignKey(l => l.ShopId)
            .HasPrincipalKey(s => s.ShopId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.SearchQuery)
            .WithMany(sq => sq.Listings)
            .HasForeignKey(l => l.SearchQueryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
