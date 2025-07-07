
using System.ComponentModel.DataAnnotations;

using static FoodStore.GCommon.ValidationConstants.Product;

namespace FoodStore.ViewModels.Admin
{
    public class EditProductInputModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(ProductNameMinLength)]
        [MaxLength(ProductNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        public int CategoryId { get; set; }
        public IEnumerable<AddCategoryDropDownMenu> Categories { get; set; }
           = new HashSet<AddCategoryDropDownMenu>();

        [Required]
        public int BrandId { get; set; }
        public IEnumerable<AddBrandDropDownMenu> Brands { get; set; }
           = new HashSet<AddBrandDropDownMenu>();

        [Required]

        public int SupplierId { get; set; }

        public IEnumerable<AddSupplierDropDownMenu> Suppliers { get; set; }
            = new HashSet<AddSupplierDropDownMenu>();

        public string ImageUrl { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}
