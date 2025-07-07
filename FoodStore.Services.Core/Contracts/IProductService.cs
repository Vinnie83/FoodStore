using FoodStore.ViewModels;
using FoodStore.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Services.Core.Contracts
{
    public interface IProductService
    {
        Task<IEnumerable<ProductViewModel>> GetByCategoryAsync(string categoryName);

        Task<ProductDetailsViewModel> GetProductByIdAsync(int productId);

        Task<bool> AddProductAsync(string userId, ProductCreateInputModel model);

        Task<string?> GetCategoryNameByIdAsync(int categoryId);

        Task<EditProductInputModel?> GetProductForEditingAsync(string userId, int? id);

        Task<bool> PersistUpdatedProductAsync(string userId, EditProductInputModel inputModel);
    }
}
