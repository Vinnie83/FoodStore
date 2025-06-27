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

            entity
                .HasData(GenerateSeedBrands());
        }

        private List<Brand> GenerateSeedBrands()
        {
            List<Brand> seedBrands = new List<Brand>()
            {
                new Brand { Id = 1, Name = "Dole", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 2, Name = "Hrisa 13", CountryOfOrigin = "Turkey" },
                new Brand { Id = 3, Name = "Moravsko selo", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 4, Name = "Meggle", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 5, Name = "Activia", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 6, Name = "Danone", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 7, Name = "Sonnenweg", CountryOfOrigin = "Germany" },
                new Brand { Id = 8, Name = "Harmonica", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 9, Name = "Gradus", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 10, Name = "Tandem", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 11, Name = "Naroden", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 12, Name = "Ambarica", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 13, Name = "Boni", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 14, Name = "Stella", CountryOfOrigin = "Greece" },
                new Brand { Id = 15, Name = "Melissa", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 16, Name = "Simid", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 17, Name = "Mestemacher", CountryOfOrigin = "Germany" },
                new Brand { Id = 18, Name = "7 days", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 19, Name = "Benjamissimo", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 20, Name = "Nestle", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 21, Name = "Belvita", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 22, Name = "Leibnitz", CountryOfOrigin = "Germany" },
                new Brand { Id = 23, Name = "Loacker", CountryOfOrigin = "Austria" },
                new Brand { Id = 24, Name = "Chio", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 25, Name = "Milka", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 26, Name = "Krina", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 27, Name = "Coca Cola", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 28, Name = "Fanta", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 29, Name = "Hipp", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 30, Name = "Fino", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 31, Name = "Duracell", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 32, Name = "Medix", CountryOfOrigin = "Bulgaria" },
                new Brand { Id = 33, Name = "Pur", CountryOfOrigin = "Bulgaria" },


            };

            return seedBrands;
        }
    }
}
