using FoodStore.Data.Models;
using FoodStore.Services.Core.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodStore.Areas.Admin.Controllers
{
    public class ReportsController : AdminBaseController
    {

        private readonly IReportService reportService;

        public ReportsController(IAdminService adminService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, 
            IReportService reportService)
            :base(adminService, userManager, roleManager)
        {
            this.reportService = reportService;
        }

        public IActionResult Index()
        {
            return View(); 
        }
        public async Task<IActionResult> Orders(string? filter)
        {
            try
            {

                var orders = await reportService.GetOrderReportsAsync(filter);

                return View(orders);

            }
            catch (Exception)
            {

                return View("ServerError");
            }
            

            
        }
    }
}
