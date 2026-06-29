using EtsyAnalyzer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtsyAnalyzer.Infrastructure.Persistence.Configurations;

public class ShopConfiguration : IEntityTypeConfiguration<Shop>
{
    public void Configure(EntityTypeBuilder<Shop> builder)
    {
        builder.ToTable("Shops");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ShopId)
            .IsRequired();

        builder.HasIndex(s => s.ShopId)
            .IsUnique();

        builder.Property(s => s.ShopName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Url)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(s => s.Rating)
            .HasPrecision(3, 2);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.HasMany(s => s.Listings)
            .WithOne(l => l.Shop)
            .HasForeignKey(l => l.ShopId)
            .HasPrincipalKey(s => s.ShopId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
