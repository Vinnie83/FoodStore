using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static FoodStore.GCommon.ValidationConstants.Product;

namespace FoodStore.Data.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        [Range((double)ProductMinPrice, (double)ProductMaxPrice)]
        public decimal Price { get; set; }

        [Range(ProductMinQuantity, ProductMaxQuantity)]
        public int Quantity { get; set; }

        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int BrandId { get; set; }
        public Brand Brand { get; set; } = null!;

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;

        public bool IsDeleted { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
