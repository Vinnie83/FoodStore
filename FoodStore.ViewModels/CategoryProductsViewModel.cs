using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.ViewModels
{
    public class CategoryProductsViewModel
    {
        public string CategoryName { get; set; } = null!;
        public IEnumerable<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
    }
}
