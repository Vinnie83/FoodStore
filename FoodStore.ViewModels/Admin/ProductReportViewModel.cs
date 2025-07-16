using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.ViewModels.Admin
{
    public class ProductReportViewModel
    {
        public string ProductName { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string Supplier { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int TotalOrderedQuantity { get; set; }
        public decimal TotalRevenue => TotalOrderedQuantity * Price;
    }
}
