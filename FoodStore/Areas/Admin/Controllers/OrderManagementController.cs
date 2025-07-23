using FoodStore.Data.Models;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels.Admin;
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


        public async Task<IActionResult> Index(string? selectedStatus, int page = 1)
        {
            int pageSize = 10;
            var orders = await orderManagementService.GetAllProcessedAndDeliveredOrdersAsync(selectedStatus, page, pageSize);

            var viewModel = new OrderFilterViewModel
            {
                SelectedStatus = selectedStatus,
                Orders = orders
            };


            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Deliver(int id)
        {
            var model = await orderManagementService.GetOrderViewModelByIdAsync(id);

            if (model == null)
                return RedirectToAction("NotFoundPage", "Error");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeliverConfirmed(int id)
        {
            try
            {
                var success = await orderManagementService.MarkOrderAsDeliveredAsync(id);

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
