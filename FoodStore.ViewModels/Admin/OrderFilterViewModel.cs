using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.ViewModels.Admin
{
    public class OrderFilterViewModel
    {
        public string? SelectedStatus { get; set; }
        public PaginatedList<OrderAdminViewModel> Orders { get; set; } = null!;
    }
}
