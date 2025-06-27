using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Data.Models
{
    public class Supplier
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string EmailAddress { get; set; } = null!;

        public ICollection<Product> Products { get; set; }
                    = new HashSet<Product>();
    }
}
