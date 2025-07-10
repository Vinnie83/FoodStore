using FoodStore.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using static FoodStore.GCommon.ValidationConstants.Supplier;

namespace FoodStore.Data.Configuration
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> entity)
        {
            entity
                .HasKey(s => s.Id);

            entity
                .Property(s => s.Name)
                .IsRequired();

            entity
                .Property(s => s.Phone)
                .IsRequired()
                .HasMaxLength(SupplierPhoneMaxLength);

            entity
                .Property(s => s.EmailAddress)
                .IsRequired()
                .HasMaxLength(SupplierEmailMaxLength);

            entity
                .Property(s => s.IsDeleted)
                .IsRequired();

            entity
                .HasData(GenerateSeedSuppliers());
        }

        private List<Supplier> GenerateSeedSuppliers()
        {
            List<Supplier> seedSuppliers = new List<Supplier>()
            {
                new Supplier { Id = 1, Name = "Maxtrade", EmailAddress = "maxtrade@info.bg", Phone = "0898 753 546", IsDeleted = false },
                new Supplier { Id = 2, Name = "Shopmaster", EmailAddress = "shopmaster@info.com", Phone = "0878 783 456", IsDeleted = false },
                new Supplier { Id = 3, Name = "Unitrade", EmailAddress = "unitrade@info.bg", Phone = "0888 784 569", IsDeleted = false },
                new Supplier {Id = 4, Name = "Brother99", EmailAddress = "brother99@gmail.com", Phone = "0898 345 908", IsDeleted = false},
                new Supplier {Id = 5, Name = "Moni", EmailAddress = "moni@mail.bg", Phone = "0877 848 098", IsDeleted = false},

            };

            return seedSuppliers;
        }
    }
}
