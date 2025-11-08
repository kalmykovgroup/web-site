using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebSite.Domain.Models;

namespace WebSite.Infrastructure.Database.Configurations
{
    public class SpecialOfferConfiguration : IEntityTypeConfiguration<SpecialOffer>
    {
        public void Configure(EntityTypeBuilder<SpecialOffer> builder)
        {
            builder.ToTable("special_offers");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(o => o.Type)
                .HasColumnName("type")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(o => o.Title)
                .HasColumnName("title")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(o => o.Description)
                .HasColumnName("description")
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(o => o.ImageUrl)
                .HasColumnName("image_url")
                .HasMaxLength(500);

            builder.Property(o => o.ImageAlt)
                .HasColumnName("image_alt")
                .HasMaxLength(200);

            builder.Property(o => o.DisplayConfig)
                .HasColumnName("display_config")
                .HasColumnType("jsonb")
                .IsRequired();

            builder.Property(o => o.Timer)
                .HasColumnName("timer")
                .HasColumnType("jsonb");

            builder.Property(o => o.Order)
                .HasColumnName("order")
                .IsRequired();

            builder.Property(o => o.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(o => o.Metadata)
                .HasColumnName("metadata")
                .HasColumnType("jsonb");

            builder.Property(o => o.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()")
                .IsRequired();

            builder.Property(o => o.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("NOW()")
                .IsRequired();

            // Индексы
            builder.HasIndex(o => o.IsActive)
                .HasDatabaseName("idx_special_offers_is_active");

            builder.HasIndex(o => o.Order)
                .HasDatabaseName("idx_special_offers_order");
        }
    }
}
