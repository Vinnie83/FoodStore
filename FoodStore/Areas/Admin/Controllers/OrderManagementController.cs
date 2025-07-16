using FoodStore.Data.Models;
using FoodStore.Services.Core.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodStore.Areas.Admin.Controllers
{
    public class OrderManagementController : AdminBaseController
    {
        private readonly IOrderManagementService orderManagementService;

        public OrderManagementController(IAdminService adminService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOrderManagementService orderManagementService)
           : base(adminService, userManager, roleManager)
        {
            this.orderManagementService = orderManagementService;
        }


        public async Task<IActionResult> Index()
        {

            var orders = await orderManagementService.GetAllProcessedOrdersAsync();

            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsDelivered(int orderId)
        {
            try
            {
                var success = await orderManagementService.MarkOrderAsDeliveredAsync(orderId);

                if (!success)
                    return RedirectToAction("NotFoundPage", "Error");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                return RedirectToAction("ServerError", "Error");
            }
            
        }
    }
}
