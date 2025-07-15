using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.ViewModels
{
    public class CancelOrderViewModel
    {
        public int OrderId { get; set; }

        public string OrderDate { get; set; } = null!;

        public string OrderStatus { get; set; } = null!;

        public string PaymentStatus { get; set; } = null!;

        public decimal TotalAmount { get; set; }
    }
}
