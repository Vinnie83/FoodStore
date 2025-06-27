using FoodStore.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Data.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> entity)
        {
            entity
                .HasKey(o => o.Id);

            entity
                .Property(o => o.OrderDate)
                .IsRequired();

            entity
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            entity
                .Property(o => o.PaymentStatus)
                .IsRequired();

            entity
                .Property(o => o.OrderStatus)
                .IsRequired();

            entity
               .HasMany(o => o.Items)
               .WithOne(i => i.Order)
               .HasForeignKey(o => o.OrderId)
               .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
