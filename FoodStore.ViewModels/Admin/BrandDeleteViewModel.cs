using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.ViewModels.Admin
{
    public class BrandDeleteViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string CountryOfOrigin { get; set; } = null!;
    }
}
