using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebSite.Domain.Models;

namespace WebSite.Infrastructure.Database.Configurations
{
    public class CategoryImageConfiguration : IEntityTypeConfiguration<CategoryImage>
    {
        public void Configure(EntityTypeBuilder<CategoryImage> builder)
        {
         
            // Primary Key
            builder.HasKey(i => i.Id);

            // Свойства
            builder.Property(i => i.Url)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(i => i.Alt)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(i => i.Order)
                .IsRequired();

            builder.Property(i => i.CategoryId)
                .IsRequired()
                .HasMaxLength(50);

            // Связь с категорией
            builder.HasOne(i => i.Category)
                .WithMany(c => c.Images)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Индексы
            builder.HasIndex(i => i.CategoryId);
            builder.HasIndex(i => new { i.CategoryId, i.Order })
                .IsUnique();
        }
    }
}
