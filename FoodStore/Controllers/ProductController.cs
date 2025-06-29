using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodStore.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService productService;

        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Category(string category)
        {
            IEnumerable<ProductViewModel> products = await this.productService
                .GetByCategoryAsync(category);

            var viewModel = new CategoryProductsViewModel
            {
                CategoryName = category,
                Products = products
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
