using FoodStore.Data.Models;
using FoodStore.Services.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public abstract class AdminBaseController : Controller
    {
        protected readonly IAdminService adminService;
        protected readonly UserManager<ApplicationUser> userManager;
        protected readonly RoleManager<IdentityRole> roleManager;

        protected AdminBaseController(
            IAdminService adminService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.adminService = adminService;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
    }
}
