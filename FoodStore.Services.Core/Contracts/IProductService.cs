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
        Task<PaginatedList<ProductViewModel>> GetByCategoryAsync(string categoryName, int pageIndex, int pageSize);

        Task<ProductDetailsViewModel> GetProductByIdAsync(int productId);

        Task<bool> AddProductAsync(string userId, ProductCreateInputModel model);

        Task<string?> GetCategoryNameByIdAsync(int categoryId);

        Task<EditProductInputModel?> GetProductForEditingAsync(string userId, int? id);

        Task<bool> PersistUpdatedProductAsync(string userId, EditProductInputModel inputModel);

        Task<ProductDeleteInputModel?> GetProductForDeletingAsync(string userId, int? productId);

        Task<bool> SoftDeleteProductAsync(string userId, ProductDeleteInputModel inputModel);

        Task<IEnumerable<ProductSearchResultViewModel>> SearchProductsAsync(string query);
    }
}
