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

                return RedirectToAction("ServerError", "Error");
            }
            

            
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            try
            {
                var details = await reportService.GetOrderDetailsAsync(id);

                if (details == null)
                {
                    return RedirectToAction("NotFoundPage", "Error");
                }

                return View(details); 
            }
            catch (Exception)
            {
                return RedirectToAction("ServerError", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel(string? filter)
        {
            try
            {
                var excelBytes = await reportService.ExportOrdersToExcelAsync(filter);
                return File(excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "OrdersReport.xlsx");
            }
            catch
            {
                return RedirectToAction("ServerError", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Products(string? filter)
        {
            try
            {
                var products = await reportService.GetProductReportsAsync(filter);
                return View(products);
            }
            catch
            {
                return RedirectToAction("ServerError", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportProductsToExcel(string? filter)
        {
            try
            {
                var excelBytes = await reportService.ExportProductsToExcelAsync(filter);
                return File(excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "ProductsReport.xlsx");
            }
            catch
            {
                return RedirectToAction("ServerError", "Error");
            }
        }
    }
}
