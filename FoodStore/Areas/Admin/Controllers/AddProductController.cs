using FoodStore.Data.Models;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AddProductController : Controller
    {

        private readonly IAdminService adminService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IProductService productService;
        private readonly ICategoryService categoryService;
        private readonly IBrandService brandService;
        private readonly ISupplierService supplierService;

        public AddProductController(
            IAdminService adminService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IProductService productService, 
            ICategoryService categoryService,
            IBrandService brandService,
            ISupplierService supplierService)
        {
            this.adminService = adminService;
            this.userManager = userManager;
            this.roleManager = roleManager;
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
    }
}
