using System.ComponentModel.DataAnnotations;
using static FoodStore.GCommon.ValidationConstants.Brand;

namespace FoodStore.ViewModels.Admin
{
    public class AddBrandInputModel
    {
        [Required]
        [MinLength(BrandNameMinLength)]
        [MaxLength(BrandNameMaxLength)]
        public string Name { get; set; } = null!;

        public string CountryOfOrigin { get; set; } = null!;
    }
}
