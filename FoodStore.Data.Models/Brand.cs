using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Data.Models
{
    public class Brand
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string CountryOfOrigin { get; set; } = null!;

        public ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}
