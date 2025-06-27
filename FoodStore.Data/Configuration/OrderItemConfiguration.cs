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
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> entity)
        {
            entity
               .HasKey(oi => oi.Id);

            entity
                .Property(oi => oi.Quantity)
                .IsRequired();

            entity
                .Property(oi => oi.Price)
                .HasPrecision(18, 2)
                .IsRequired();

            entity
                .Property(oi => oi.ProductId)
                .IsRequired();

            entity
                 .HasOne(oi => oi.Product)
                 .WithMany(p => p.OrderItems)
                 .HasForeignKey(oi => oi.ProductId)
                 .IsRequired(false)
                 .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
