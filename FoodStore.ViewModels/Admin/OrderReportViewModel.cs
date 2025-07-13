using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.ViewModels.Admin
{
    public class OrderReportViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string ProductName { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string Supplier { get; set; } = null!;
        public int Quantity { get; set; }

        public string UserEmail { get; set; } = null!;
    }
}
