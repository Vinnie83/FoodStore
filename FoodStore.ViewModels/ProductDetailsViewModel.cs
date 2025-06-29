using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.ViewModels
{
    public class ProductDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public decimal Price { get; set; }
        public string UnitQuantity { get; set; } = "1";
        public int StockQuantity { get; set; }
        public string Brand { get; set; } = null!;
        public string Category { get; set; } = null!;
    }
}
