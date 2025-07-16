using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.ViewModels.Admin
{
    public class DeliverOrderViewModel
    {
        public int OrderId { get; set; }
        public string UserEmail { get; set; } = null!;
        public string OrderDate { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public string TotalAmount { get; set; } = null!;
    }
}
