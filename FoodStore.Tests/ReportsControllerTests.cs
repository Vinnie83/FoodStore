using FoodStore.Areas.Admin.Controllers;
using FoodStore.Data.Models;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels.Admin;
using FoodStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Tests
{
    public class ReportsControllerTests
    {

        private Mock<IReportService> mockReportService;
        private Mock<IAdminService> mockAdminService;
        private Mock<UserManager<ApplicationUser>> mockUserManager;
        private Mock<RoleManager<IdentityRole>> mockRoleManager;
        private ReportsController controller;

        [SetUp]
        public void SetUp()
        {
            mockReportService = new Mock<IReportService>();
            mockAdminService = new Mock<IAdminService>();

            mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();

            controller = new ReportsController(
                mockAdminService.Object,
                mockUserManager.Object,
                mockRoleManager.Object,
                mockReportService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            mockReportService = null;
            mockAdminService = null;
            mockUserManager = null;
            mockRoleManager = null;
            controller.Dispose();
        }

        [Test]
        public void Index_ReturnsView()
        {
            var result = controller.Index();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Orders_ReturnsViewWithOrders()
        {
            var paginated = new PaginatedList<OrderReportViewModel>(
        new List<OrderReportViewModel>(), 0, 1, 10);

            var expected = new OrdersReportPageViewModel
            {
                Reports = paginated,
                UserEmails = new List<string>(),
                Suppliers = new List<string>(),
                Brands = new List<string>(),
                Categories = new List<string>()
            };

            mockReportService
                .Setup(x => x.GetOrderReportsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(expected);

            var result = await controller.Orders(null, 1) as ViewResult;

            Assert.IsNotNull(result);
            Assert.That(result.Model, Is.EqualTo(expected));
        }

        [Test]
        public async Task Orders_OnException_RedirectsToServerError()
        {
            mockReportService.Setup(x => x.GetOrderReportsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            var result = await controller.Orders(null, 1) as RedirectToActionResult;

            Assert.That(result.ActionName, Is.EqualTo("ServerError"));
        }

        [Test]
        public async Task OrderDetails_ValidId_ReturnsView()
        {
            var details = new OrderDetailsViewModel();
            mockReportService.Setup(x => x.GetOrderDetailsAsync(1)).ReturnsAsync(details);

            var result = await controller.OrderDetails(1) as ViewResult;

            Assert.IsNotNull(result);
            Assert.That(result.Model, Is.EqualTo(details));
        }

        [Test]
        public async Task OrderDetails_Null_ReturnsNotFoundPage()
        {
            mockReportService.Setup(x => x.GetOrderDetailsAsync(1)).ReturnsAsync((OrderDetailsViewModel)null);

            var result = await controller.OrderDetails(1) as RedirectToActionResult;

            Assert.That(result.ActionName, Is.EqualTo("NotFoundPage"));
        }

        [Test]
        public async Task OrderDetails_OnException_RedirectsToServerError()
        {
            mockReportService.Setup(x => x.GetOrderDetailsAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            var result = await controller.OrderDetails(1) as RedirectToActionResult;

            Assert.That(result.ActionName, Is.EqualTo("ServerError"));
        }

        [Test]
        public async Task ExportToExcel_ReturnsFileResult()
        {
            var bytes = new byte[] { 1, 2, 3 };
            mockReportService.Setup(x => x.ExportOrdersToExcelAsync(null)).ReturnsAsync(bytes);

            var result = await controller.ExportToExcel(null) as FileContentResult;

            Assert.IsNotNull(result);
            Assert.That(result.ContentType, Is.EqualTo("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
        }

        [Test]
        public async Task ExportToExcel_OnException_RedirectsToServerError()
        {
            mockReportService.Setup(x => x.ExportOrdersToExcelAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var result = await controller.ExportToExcel(null) as RedirectToActionResult;

            Assert.That(result.ActionName, Is.EqualTo("ServerError"));
        }

        [Test]
        public async Task Products_ReturnsViewWithModel()
        {
            var paginated = new PaginatedList<ProductReportViewModel>(
            new List<ProductReportViewModel>(), 0, 1, 10);

            var expected = new ProductReportsPageViewModel
            {
                Reports = paginated,
                Categories = new List<string>(),
                Brands = new List<string>(),
                Suppliers = new List<string>()
            };

            mockReportService
                .Setup(x => x.GetProductReportsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(expected);

            var result = await controller.Products(null, 1) as ViewResult;

            Assert.IsNotNull(result);
            Assert.That(result.Model, Is.EqualTo(expected));
        }

        [Test]
        public async Task Products_OnException_RedirectsToServerError()
        {
            mockReportService.Setup(x => x.GetProductReportsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            var result = await controller.Products(null, 1) as RedirectToActionResult;

            Assert.That(result.ActionName, Is.EqualTo("ServerError"));
        }

        [Test]
        public async Task ExportProductsToExcel_ReturnsFile()
        {
            var bytes = new byte[] { 1, 2, 3 };
            mockReportService.Setup(x => x.ExportProductsToExcelAsync(null))
                .ReturnsAsync(bytes);

            var result = await controller.ExportProductsToExcel(null) as FileContentResult;

            Assert.IsNotNull(result);
            Assert.That(result.FileDownloadName, Is.EqualTo("ProductsReport.xlsx"));
        }

        [Test]
        public async Task ExportProductsToExcel_OnException_RedirectsToServerError()
        {
            mockReportService.Setup(x => x.ExportProductsToExcelAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var result = await controller.ExportProductsToExcel(null) as RedirectToActionResult;

            Assert.That(result.ActionName, Is.EqualTo("ServerError"));
        }
    }
}
