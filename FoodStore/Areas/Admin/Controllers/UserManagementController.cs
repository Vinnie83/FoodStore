using FoodStore.Data.Models;
using FoodStore.Services.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodStore.Areas.Admin.Controllers
{

    public class UserManagementController : AdminBaseController
    {


        public UserManagementController(
            IAdminService adminService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
            : base(adminService, userManager, roleManager)
        {

        }

        public async Task<IActionResult> Index()
        {
            var users = await adminService.GetAllUsersAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                await userManager.DeleteAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddToAdmin(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null && !await userManager.IsInRoleAsync(user, "Admin"))
            {
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                await userManager.AddToRoleAsync(user, "Admin");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromAdmin(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null && await userManager.IsInRoleAsync(user, "Admin"))
            {
                await userManager.RemoveFromRoleAsync(user, "Admin");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

    }
}
