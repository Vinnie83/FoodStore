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
        public async Task<IActionResult> Category(string category, int page = 1)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(category))
                {
                    return RedirectToAction("NotFoundPage", "Error");
                }

                int pageSize = 4;
                var products = await productService.GetByCategoryAsync(category, page, pageSize);

                var viewModel = new CategoryProductsViewModel
                {
                    CategoryName = category,
                    Products = products
                };

                return View(viewModel);
            }
            catch (Exception)
            {
                return RedirectToAction("ServerError", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var product = await productService.GetProductByIdAsync(id);

                if (product == null)
                {
                    return RedirectToAction("NotFoundPage", "Error");
                }

                return View(product);
            }
            catch (Exception)
            {
                return RedirectToAction("ServerError", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            try
            {
                var result = await productService.SearchProductsAsync(query);

                if (result == null)
                {
                    return RedirectToAction("NotFoundPage", "Error");
                }
                return View(result);
            }
            catch (Exception)
            {

                return RedirectToAction("ServerError", "Error");
            }

        }

    }
}
