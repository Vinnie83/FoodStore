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


            entity
                .HasData(GenerateSeedProducts());
        }

        private List<Product> GenerateSeedProducts()
        {
            List<Product> seedProducts = new List<Product>()
            {
                new Product { 
                    Id = 1, 
                    Name = "Banana", 
                    Price = 2.30m, 
                    Quantity = 150, 
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-157.jpg", 
                    CategoryId = 1, 
                    BrandId =  1,
                    SupplierId = 5,
                    },
                new Product {
                    Id = 2,
                    Name = "Avocado",
                    Price = 4.70m,
                    Quantity = 20,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-109.jpg",
                    CategoryId = 1,
                    BrandId =  2,
                    SupplierId = 5,
                    },
                new Product {
                    Id = 3,
                    Name = "Potato",
                    Price = 3.20m,
                    Quantity = 200,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-395.jpg",
                    CategoryId = 1,
                    BrandId =  3,
                    SupplierId = 5,
                    },
                new Product {
                    Id = 4,
                    Name = "Onion",
                    Price = 2.10m,
                    Quantity = 100,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-102.jpg",
                    CategoryId = 1,
                    BrandId =  2,
                    SupplierId = 5,
                    },
                new Product {
                    Id = 5,
                    Name = "Yogurt Meggle",
                    Price = 1.80m,
                    Quantity = 50,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-11242.jpg",
                    CategoryId = 2,
                    BrandId =  4,
                    SupplierId = 4,
                    },
                new Product {
                    Id = 6,
                    Name = "Yogurt Activia Naturalna",
                    Price = 2.10m,
                    Quantity = 60,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-10247.jpg",
                    CategoryId = 2,
                    BrandId =  5,
                    SupplierId = 4,
                    },
                new Product {
                    Id = 7,
                    Name = "Yogurt Danone Vanilla",
                    Price = 2.50m,
                    Quantity = 20,
                    ImageUrl = "https://powellsnl.ca/media/uploads/gs1/.thumbnails/05680008218_1.png/05680008218_1-650x0-padded-%23fff.png",
                    CategoryId = 2,
                    BrandId =  6,
                    SupplierId = 4,
                    },
                new Product {
                    Id = 8,
                    Name = "Butter Sonnenweg",
                    Price = 4.10m,
                    Quantity = 35,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-11026.jpg",
                    CategoryId = 2,
                    BrandId =  7,
                    SupplierId = 4,
                    },
                new Product {
                    Id = 9,
                    Name = "Milk Harmonica",
                    Price = 5.10m,
                    Quantity = 20,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-10608.jpg",
                    CategoryId = 2,
                    BrandId =  8,
                    SupplierId = 4,
                    },
                new Product {
                    Id = 10,
                    Name = "Chicken Gradus",
                    Price = 5.10m,
                    Quantity = 30,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-2115.jpg",
                    CategoryId = 3,
                    BrandId =  9,
                    SupplierId = 1,
                    },
                new Product {
                    Id = 11,
                    Name = "Minced meat Tandem",
                    Price = 8.50m,
                    Quantity = 50,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-14157.jpg",
                    CategoryId = 3,
                    BrandId =  10,
                    SupplierId = 1,
                    },
                new Product {
                    Id = 12,
                    Name = "Salami Naroden Hamburgski",
                    Price = 4.30m,
                    Quantity = 30,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-14062.jpg",
                    CategoryId = 4,
                    BrandId =  11,
                    SupplierId = 1,
                    },
                new Product {
                    Id = 13,
                    Name = "Ambarica Chichovtsi",
                    Price = 6.50m,
                    Quantity = 30,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-14289.jpg",
                    CategoryId = 4,
                    BrandId =  12,
                    SupplierId = 1,
                    },
                new Product {
                    Id = 14,
                    Name = "Boni Sausage",
                    Price = 5.50m,
                    Quantity = 30,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-14267.jpg",
                    CategoryId = 4,
                    BrandId =  13,
                    SupplierId = 1,
                    },
                new Product {
                    Id = 15,
                    Name = "Couscous Stella",
                    Price = 3.10m,
                    Quantity = 50,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-30036.jpg",
                    CategoryId = 5,
                    BrandId =  14,
                    SupplierId = 2,
                    },
                new Product {
                    Id = 16,
                    Name = "Couscous Melissa",
                    Price = 3.30m,
                    Quantity = 30,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-30011.jpg",
                    CategoryId = 5,
                    BrandId =  15,
                    SupplierId = 2,
                    },
                new Product {
                    Id = 17,
                    Name = "Bread Simid",
                    Price = 2.40m,
                    Quantity = 60,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-30662.jpg",
                    CategoryId = 5,
                    BrandId =  16,
                    SupplierId = 2,
                    },
                new Product {
                    Id = 18,
                    Name = "Bread Mestemacher",
                    Price = 2.40m,
                    Quantity = 15,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-30478.jpg",
                    CategoryId = 5,
                    BrandId =  17,
                    SupplierId = 2,
                    },
                new Product {
                    Id = 19,
                    Name = "Bake Rolls Spinach",
                    Price = 1.30m,
                    Quantity = 50,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-55101.jpg",
                    CategoryId = 6,
                    BrandId =  18,
                    SupplierId = 2,
                    },
                new Product {
                    Id = 20,
                    Name = "Chocolate Benjamissimo Caramel",
                    Price = 6.40m,
                    Quantity = 22,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-159011.jpg",
                    CategoryId = 6,
                    BrandId =  19,
                    SupplierId = 2,
                    },
                new Product {
                    Id = 21,
                    Name = "Biscuits Nestle Zhiten Dar",
                    Price = 2.40m,
                    Quantity = 30,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-50242.jpg",
                    CategoryId = 6,
                    BrandId =  20,
                    SupplierId = 2,
                    },
                new Product {
                    Id = 22,
                    Name = "Biscuits Belvita wholegrain",
                    Price = 1.30m,
                    Quantity = 32,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-51979.jpg",
                    CategoryId = 6,
                    BrandId =  21,
                    SupplierId = 2,
                    },
                new Product {
                    Id = 23,
                    Name = "Cookies Leibnitz butter",
                    Price = 2.80m,
                    Quantity = 32,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-51690.jpg",
                    CategoryId = 6,
                    BrandId =  22,
                    SupplierId = 2,
                    },
                new Product {
                    Id = 24,
                    Name = "Biscuits Loacker Noisette",
                    Price = 5.60m,
                    Quantity = 21,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-50015.jpg",
                    CategoryId = 6,
                    BrandId =  23,
                    SupplierId = 2,
                    },
                new Product {
                    Id = 25,
                    Name = "Chips Chio Salted",
                    Price = 3.70m,
                    Quantity = 30,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-55311.jpg",
                    CategoryId = 6,
                    BrandId =  24,
                    SupplierId = 2,
                    },
                new Product {
                    Id = 26,
                    Name = "Chocolate Cookies Milka",
                    Price = 3.80m,
                    Quantity = 15,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-50419.jpg",
                    CategoryId = 6,
                    BrandId =  25,
                    SupplierId = 2,
                    },
                new Product {
                    Id = 27,
                    Name = "Beans Krina Extra",
                    Price = 2.80m,
                    Quantity = 25,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-31041.jpg",
                    CategoryId = 7,
                    BrandId =  26,
                    SupplierId = 2,
                    },
                new Product {
                    Id = 28,
                    Name = "Coca Cola Original",
                    Price = 2.40m,
                    Quantity = 80,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-40062.jpg",
                    CategoryId = 8,
                    BrandId =  27,
                    SupplierId = 3,
                    },
                new Product {
                    Id = 29,
                    Name = "Fanta Orange",
                    Price = 3.20m,
                    Quantity = 54,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-40124.jpg",
                    CategoryId = 8,
                    BrandId =  28,
                    SupplierId = 3,
                    },
                new Product {
                    Id = 30,
                    Name = "Baby Puree Hipp Pumpkin",
                    Price = 3.40m,
                    Quantity = 14,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-38038.jpg",
                    CategoryId = 9,
                    BrandId =  29,
                    SupplierId = 3,
                    },
                new Product {
                    Id = 31,
                    Name = "Aluminium Foil Fino",
                    Price = 3.60m,
                    Quantity = 20,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-90050.jpg",
                    CategoryId = 10,
                    BrandId =  30,
                    SupplierId = 3,
                    },
                new Product {
                    Id = 32,
                    Name = "Batteries duracell AAA",
                    Price = 8.90m,
                    Quantity = 25,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-90049.jpg",
                    CategoryId = 10,
                    BrandId =  31,
                    SupplierId = 3,
                    },
                new Product {
                    Id = 33,
                    Name = "Washing-up Liquid Medix",
                    Price = 2.10m,
                    Quantity = 20,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-80275.jpg",
                    CategoryId = 10,
                    BrandId =  32,
                    SupplierId = 3,
                    },
                new Product {
                    Id = 34,
                    Name = "Washing-up Liquid Pur Apple",
                    Price = 3.15m,
                    Quantity = 18,
                    ImageUrl = "https://optima.bg/data/ufiles/images/catalog/main/big/Optima-80055.jpg",
                    CategoryId = 10,
                    BrandId =  33,
                    SupplierId = 3,
                    },

            };

            return seedProducts;
        }
    }
}
