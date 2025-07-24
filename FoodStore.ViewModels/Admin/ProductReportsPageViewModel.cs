using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.ViewModels.Admin
{
    public class ProductReportsPageViewModel
    {
        public PaginatedList<ProductReportViewModel> Reports { get; set; } = null!;
        public List<string> Categories { get; set; } = null!;
        public List<string> Brands { get; set; } = null!;
        public List<string> Suppliers { get; set; } = null!;
    }
}
