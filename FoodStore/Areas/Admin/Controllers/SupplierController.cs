using FoodStore.Data.Models;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodStore.Areas.Admin.Controllers
{
    public class SupplierController : AdminBaseController
    {
        private readonly ISupplierService supplierService;
        public SupplierController(IAdminService adminService, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            ISupplierService supplierService) 
            : base(adminService, userManager, roleManager)
        {
            this.supplierService = supplierService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var services = await this.supplierService.GetAllSuppliersAsync();
            return View(services);
        }

        [HttpGet]
        public IActionResult AddSupplier()
        {
            return View("AddSupplier");
        }

        [HttpPost]

        public async Task<IActionResult> AddSupplier(AddSupplierInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                Console.WriteLine("ModelState is INVALID!");

                return this.View(inputModel);

            }

            string? userId = this.userManager.GetUserId(this.User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            bool addResult = await this.supplierService.AddSupplierAsync(userId, inputModel);

            if (!addResult)
            {

                ModelState.AddModelError(string.Empty, "Fatal error occurred while adding a supplier");
                return this.View(inputModel);
            }

            return this.RedirectToAction("Index");
        }
    }
}
