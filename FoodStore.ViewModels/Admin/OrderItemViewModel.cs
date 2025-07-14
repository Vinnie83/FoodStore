using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.ViewModels.Admin
{
    public class OrderItemViewModel
    {
        public string ProductName { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string Supplier { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }  // price per unit
        public decimal TotalItemPrice => Price * Quantity;
    }
}
