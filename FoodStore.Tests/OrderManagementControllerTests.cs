using FoodStore.Areas.Admin.Controllers;
using FoodStore.Data.Models;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels;
using FoodStore.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using static FoodStore.Tests.MockHelpers;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Tests
{
    public class OrderManagementControllerTests
    {
        private Mock<IOrderManagementService> mockOrderService;
        private Mock<IAdminService> mockAdminService;
        private Mock<UserManager<ApplicationUser>> mockUserManager;
        private Mock<RoleManager<IdentityRole>> mockRoleManager;
        private OrderManagementController controller;

        [SetUp]
        public void SetUp()
        {
            mockOrderService = new Mock<IOrderManagementService>();
            mockAdminService = new Mock<IAdminService>();
            mockUserManager = MockHelpers.MockUserManager<ApplicationUser>();
            mockRoleManager = MockHelpers.MockRoleManager<IdentityRole>();

            controller = new OrderManagementController(
                mockAdminService.Object,
                mockUserManager.Object,
                mockRoleManager.Object,
                mockOrderService.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            mockOrderService = null;
            mockAdminService = null;
            mockUserManager = null;
            mockRoleManager = null;
            controller.Dispose();
        }

        [Test]
        public async Task Index_ReturnsViewWithViewModel()
        {
            int page = 1;
            int pageSize = 4;

            var mockOrders = new List<OrderAdminViewModel>
            {
                new OrderAdminViewModel { OrderId = 1, OrderStatus = "Processed" },
                new OrderAdminViewModel { OrderId = 2, OrderStatus = "Delivered" }
            };

            var paginatedList = new PaginatedList<OrderAdminViewModel>(
                mockOrders, mockOrders.Count, page, pageSize);

            mockOrderService.Setup(x => x.GetAllProcessedAndDeliveredOrdersAsync(null, 1, 10))
                .ReturnsAsync(paginatedList);

            
            var result = await controller.Index(null, 1) as ViewResult;


            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OrderFilterViewModel>(result.Model);
            var model = result.Model as OrderFilterViewModel;
            Assert.That(model.Orders.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task Deliver_OrderExists_ReturnsViewWithModel()
        {


            var order = new DeliverOrderViewModel { OrderId = 1, PaymentStatus = "Paid" };


            mockOrderService.Setup(x => x.GetOrderViewModelByIdAsync(1))
                .ReturnsAsync(order);

       
            var result = await controller.Deliver(1) as ViewResult;

       
            Assert.IsNotNull(result);
            Assert.That(result.Model, Is.EqualTo(order));
        }

        [Test]
        public async Task Deliver_OrderNotFound_RedirectsToNotFoundPage()
        {
         
            mockOrderService.Setup(x => x.GetOrderViewModelByIdAsync(1))
                .ReturnsAsync((DeliverOrderViewModel)null);

       
            var result = await controller.Deliver(1) as RedirectToActionResult;

         
            Assert.IsNotNull(result);
            Assert.That(result.ActionName, Is.EqualTo("NotFoundPage")   );
            Assert.That(result.ControllerName, Is.EqualTo("Error"));
        }

        [Test]
        public async Task DeliverConfirmed_Success_RedirectsToIndex()
        {
            // Arrange
            mockOrderService.Setup(x => x.MarkOrderAsDeliveredAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await controller.DeliverConfirmed(1) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task DeliverConfirmed_Failure_RedirectsToNotFoundPage()
        {
         
            mockOrderService.Setup(x => x.MarkOrderAsDeliveredAsync(1))
                .ReturnsAsync(false);

            var result = await controller.DeliverConfirmed(1) as RedirectToActionResult;

          
            Assert.IsNotNull(result);
            Assert.That(result.ActionName, Is.EqualTo("NotFoundPage"));
            Assert.That(result.ControllerName, Is.EqualTo("Error"));
        }

        [Test]
        public async Task DeliverConfirmed_ExceptionThrown_RedirectsToServerError()
        {
      
            mockOrderService.Setup(x => x.MarkOrderAsDeliveredAsync(1))
                .ThrowsAsync(new System.Exception());

    
            var result = await controller.DeliverConfirmed(1) as RedirectToActionResult;

      
            Assert.IsNotNull(result);
            Assert.That(result.ActionName, Is.EqualTo("ServerError"));
            Assert.That(result.ControllerName, Is.EqualTo("Error")  );
        }

    }
}
