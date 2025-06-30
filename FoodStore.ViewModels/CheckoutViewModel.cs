using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.ViewModels
{
    public class CheckoutViewModel
    {

        public IEnumerable<CartItemViewModel> Items { get; set; } = new HashSet<CartItemViewModel>();

        public decimal TotalAmount { get; set; }
    }
}
