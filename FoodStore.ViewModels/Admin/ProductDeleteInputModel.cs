using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.ViewModels.Admin
{
    public class ProductDeleteInputModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string CategoryName { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public decimal? Price { get; set; }

    }
}
