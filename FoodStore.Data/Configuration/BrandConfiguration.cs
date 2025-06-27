using FoodStore.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using static FoodStore.GCommon.ValidationConstants.Brand;

namespace FoodStore.Data.Configuration
{
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> entity)
        {
            entity
               .HasKey(b => b.Id);

            entity
                .Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(BrandNameMaxLength);

            entity
                .Property(b => b.CountryOfOrigin)
                .IsRequired();

            entity.HasMany(b => b.Products)
                .WithOne(p => p.Brand)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
