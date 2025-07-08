
using System.ComponentModel.DataAnnotations;

using static FoodStore.GCommon.ValidationConstants.Brand;


namespace FoodStore.ViewModels.Admin
{
    public class EditBrandInputModel
    {
        public int Id { get; set; } 

        [Required]
        [MinLength(BrandNameMinLength)]
        [MaxLength(BrandNameMaxLength)]
        public string Name { get; set; } = null!;

        public string CountryOfOrigin { get; set; } = null!;
    }
}
