using FoodStore.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using static FoodStore.GCommon.ValidationConstants.Category;

namespace FoodStore.Data.Configuration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> entity)
        {
            entity
                .HasKey(c => c.Id);

            entity
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(CategoryNameMaxLength);

            entity
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasData(GenerateSeedCategories());
        }

        private List<Category> GenerateSeedCategories()
        {
            List<Category> seedCategories = new List<Category>()
            {
                new Category { Id = 1, Name = "Fruits and vegetables" },
                new Category { Id = 2, Name = "Milk and eggs" },
                new Category { Id = 3, Name = "Meat and fish" },
                new Category { Id = 4, Name = "Sausages and deli" },
                new Category { Id = 5, Name = "Bread and pastry" },
                new Category { Id = 6, Name = "Sweets and snacks" },
                new Category { Id = 7, Name = "Package foods" },
                new Category { Id = 8, Name = "Beverages" },
                new Category { Id = 9, Name = "Baby products" },
                new Category { Id = 10, Name = "Household products" },
            };

            return seedCategories;
        }
    }


}
