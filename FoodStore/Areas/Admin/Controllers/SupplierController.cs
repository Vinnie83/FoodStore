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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSupplier(AddSupplierInputModel inputModel)
        {
            try
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
            catch (Exception)
            {

                return RedirectToAction("ServerError", "Error");
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> EditSupplier(int? id)
        {
            try
            {
                string? userId = this.userManager.GetUserId(this.User);
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account", new { area = "" });
                }

                var model = await this.supplierService.GetSupplierForEditingAsync(userId, id);
                if (model == null)
                {
                    return RedirectToAction("NotFoundPage", "Error"); ;
                }

                return View(model);
            }
            catch (Exception)
            {

                return RedirectToAction("ServerError", "Error");
            }
            

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSupplier(EditSupplierInputModel inputModel)
        {
            try
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

                bool result = await this.supplierService.EditSupplierAsync(userId, inputModel);
                if (!result)
                {
                    return RedirectToAction("ServerError", "Error");
                }

                return RedirectToAction("Index", "Supplier", new { area = "Admin" });
            }
            catch (Exception)
            {

                return RedirectToAction("ServerError", "Error");
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                string? userId = this.userManager.GetUserId(this.User);
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account", new { area = "" });
                }

                var model = await this.supplierService.GetSupplierForDeletingAsync(userId, id);
                if (model == null)
                {
                    return RedirectToAction("NotFoundPage", "Error");
                }

                return View(model);
            }
            catch (Exception)
            {

                return RedirectToAction("ServerError", "Error");
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(SupplierDeleteViewModel inputModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Delete", inputModel);
                }

                string? userId = this.userManager.GetUserId(this.User);
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account", new { area = "" });
                }

                bool isDeleted = await this.supplierService.SoftDeleteSupplierAsync(userId, inputModel);
                if (!isDeleted)
                {
                    return RedirectToAction("NotFoundPage", "Error");
                }

                return RedirectToAction("Index", "Supplier", new { area = "Admin" });
            }
            catch (Exception)
            {

                return RedirectToAction("ServerError", "Error");
            }
            

        }
    }
}
