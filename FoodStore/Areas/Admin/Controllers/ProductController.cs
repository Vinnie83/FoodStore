using FoodStore.Data.Models;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodStore.Areas.Admin.Controllers
{
    public class ProductController : AdminBaseController
    {

        private readonly IProductService productService;
        private readonly ICategoryService categoryService;
        private readonly IBrandService brandService;
        private readonly ISupplierService supplierService;

        public ProductController(
            IAdminService adminService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IProductService productService, 
            ICategoryService categoryService,
            IBrandService brandService,
            ISupplierService supplierService)
            : base(adminService, userManager, roleManager)
        {
  
            this.productService = productService;
            this.categoryService = categoryService;
            this.brandService = brandService;
            this.supplierService = supplierService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ProductCreateInputModel model = new ProductCreateInputModel()
            {
                Categories = await this.categoryService.GetCategoriesDropDownAsync(),
                Brands = await this.brandService.GetBrandsDropDownAsync(),
                Suppliers = await this.supplierService.GetSuppliersDropDownAsync()
            };

            return this.View(model);
        }

        [HttpPost]

        public async Task<IActionResult> Create(ProductCreateInputModel inputModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    Console.WriteLine("ModelState is INVALID!");

                    inputModel.Categories = await this.categoryService.GetCategoriesDropDownAsync();
                    inputModel.Brands = await this.brandService.GetBrandsDropDownAsync();
                    inputModel.Suppliers = await this.supplierService.GetSuppliersDropDownAsync();

                    return this.View(inputModel);
                }

                string? userId = this.userManager.GetUserId(this.User);
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account", new { area = "" });
                }
                bool addResult = await this.productService.AddProductAsync(userId, inputModel);

                if (!addResult)
                {
                    inputModel.Categories = await this.categoryService.GetCategoriesDropDownAsync();
                    inputModel.Brands = await this.brandService.GetBrandsDropDownAsync();
                    inputModel.Suppliers = await this.supplierService.GetSuppliersDropDownAsync();

                    ModelState.AddModelError(string.Empty, "Fatal error occurred while adding a product");
                    return this.View(inputModel);
                }

                string? categoryName = await this.productService.GetCategoryNameByIdAsync(inputModel.CategoryId);

                return this.RedirectToAction("Category", "Product", new { area = "", category = categoryName });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                return this.View("ServerError");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            string? userId = this.userManager.GetUserId(this.User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var model = await this.productService.GetProductForEditingAsync(userId, id);
            if (model == null)
            {
                return NotFound();
            }

            model.Categories = await this.categoryService.GetCategoriesDropDownAsync();
            model.Brands = await this.brandService.GetBrandsDropDownAsync();
            model.Suppliers = await this.supplierService.GetSuppliersDropDownAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditProductInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                inputModel.Categories = await this.categoryService.GetCategoriesDropDownAsync();
                inputModel.Brands = await this.brandService.GetBrandsDropDownAsync();
                inputModel.Suppliers = await this.supplierService.GetSuppliersDropDownAsync();
                return View(inputModel);
            }

            string? userId = this.userManager.GetUserId(this.User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            bool result = await this.productService.PersistUpdatedProductAsync(userId, inputModel);
            if (!result)
            {
                return this.View("ServerError");
            }

            return RedirectToAction("Details", "Product", new { area = "", id = inputModel.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            string? userId = this.userManager.GetUserId(this.User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var model = await this.productService.GetProductForDeletingAsync(userId, id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ProductDeleteInputModel inputModel)
        {
            string? userId = this.userManager.GetUserId(this.User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            bool success = await this.productService.SoftDeleteProductAsync(userId, inputModel);

            if (!success)
            {
                return View("Error");
            }

            string categoryName = inputModel.CategoryName;

            return this.RedirectToAction("Category", "Product", new { area = "", category = categoryName });
        }
    }
}
