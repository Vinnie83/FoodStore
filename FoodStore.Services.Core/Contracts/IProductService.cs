using FoodStore.ViewModels;
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
    }
}
