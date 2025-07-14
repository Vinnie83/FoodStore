using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.ViewModels.Admin
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }

        public string OrderDate { get; set; } = null!;

        public string UserEmail { get; set; } = null!;

        public List<OrderItemViewModel> Items { get; set; } = new();

        public decimal TotalPrice { get; set; }
    }
}
