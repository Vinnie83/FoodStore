using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.ViewModels.Admin
{
    public class OrdersReportPageViewModel
    {

        public PaginatedList<OrderReportViewModel> Reports { get; set; }

        public List<string> UserEmails { get; set; } = new();
        public List<string> Suppliers { get; set; } = new();
        public List<string> Brands { get; set; } = new();
        public List<string> Categories { get; set; } = new();
    }
}
