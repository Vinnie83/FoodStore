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
        }
    }
}
