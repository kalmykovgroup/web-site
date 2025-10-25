using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSite.Domain.Models;

namespace WebSite.Infrastructure.Database.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            // Таблица 

            // Primary Key
            builder.HasKey(c => c.Id);

            // Свойства
            builder.Property(c => c.Id)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(c => c.Color)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.UpdatedAt)
                .IsRequired(false);

            builder.Property(i => i.IconUrl)
                     .IsRequired()
                     .HasMaxLength(1000);

            builder.Property(i => i.IconAlt)
                .IsRequired()
                .HasMaxLength(200); 

            // Связь с изображениями
            builder.HasMany(c => c.Images)
                .WithOne(i => i.Category)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Индексы
            builder.HasIndex(c => c.Name);
            builder.HasIndex(c => c.Color);
        }
    }
}
