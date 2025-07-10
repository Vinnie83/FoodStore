using System.ComponentModel.DataAnnotations;
using static FoodStore.GCommon.ValidationConstants.Supplier;

namespace FoodStore.ViewModels.Admin
{
    public class EditSupplierInputModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(SupplierPhoneMinLength)]
        [MaxLength(SupplierPhoneMaxLength)]
        public string Phone { get; set; } = null!;

        [Required]
        [MinLength(SupplierEmailMinLength)]
        [MaxLength(SupplierEmailMaxLength)]
        public string EmailAddress { get; set; } = null!;
    }
}
