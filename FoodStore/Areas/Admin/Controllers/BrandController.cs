using FoodStore.Data.Models;
using FoodStore.Services.Core;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace FoodStore.Areas.Admin.Controllers
{

    public class BrandController : AdminBaseController
    {
        private readonly IBrandService brandService;
        public BrandController(
            IAdminService adminService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IBrandService brandService)
           : base(adminService, userManager, roleManager)
        {
              this.brandService = brandService;  
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var brands = await this.brandService.GetAllBrandsAsync();
            return View(brands);
        }

        [HttpGet]
        public IActionResult AddBrand()
        {
            return View("AddBrand");
        }

        [HttpPost]
        public async Task<IActionResult> AddBrand(AddBrandInputModel inputModel)
        {
            if(!this.ModelState.IsValid)
            {
                Console.WriteLine("ModelState is INVALID!");

                return this.View(inputModel);
                
            }

            string? userId = this.userManager.GetUserId(this.User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            bool addResult = await this.brandService.AddBrandAsync(userId, inputModel);

            if (!addResult)
            {

                ModelState.AddModelError(string.Empty, "Fatal error occurred while adding a brand");
                return this.View(inputModel);
            }

            return this.RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditBrand(int? id)
        {
            string? userId = this.userManager.GetUserId(this.User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var model = await this.brandService.GetBrandForEditingAsync(userId, id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> EditBrand(EditBrandInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return View(inputModel);
            }

            string? userId = this.userManager.GetUserId(this.User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            bool result = await this.brandService.EditBrandAsync(userId, inputModel);
            if (!result)
            {
                return this.View("ServerError");
            }

            return RedirectToAction("Index", "Brand", new { area = "Admin" });
        }
    }
}
