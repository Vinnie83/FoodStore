using FoodStore.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using static FoodStore.GCommon.ValidationConstants.Product;

namespace FoodStore.Data.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> entity)
        {
            entity
                .HasKey(p => p.Id);

            entity
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(ProductNameMaxLength);

            entity
                .Property(p => p.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            entity
                .Property(p => p.Quantity)
                .IsRequired();


            entity
                .Property(p => p.ImageUrl)
                .IsRequired(false);

            entity
                .HasOne(p => p.Category)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(p => p.Supplier)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .Property(p => p.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            entity
                .HasQueryFilter(p => p.IsDeleted == false);

        }
    }
}
