using FoodStore.Data.Models;
using FoodStore.Services.Core.Contracts;
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
    }
}
