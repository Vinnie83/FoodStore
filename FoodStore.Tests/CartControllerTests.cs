using FoodStore.Controllers;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Tests
{
    internal class CartControllerTests
    {
        private Mock<ICartService> mockCartService;
        private CartController controller;

        private const string TestUserId = "test-user-id";

        [SetUp]
        public void SetUp()
        {
            mockCartService = new Mock<ICartService>();

            controller = new CartController(mockCartService.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, TestUserId)
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            controller.TempData = new TempDataDictionary(
            new DefaultHttpContext(),
            Mock.Of<ITempDataProvider>()
);
        }

        [TearDown]

        public void TearDown()
        {
            controller.Dispose();
        }

        [Test]
        public async Task Index_ReturnsViewWithCartItems()
        {
            mockCartService.Setup(s => s.GetCartItemsAsync(TestUserId))
                .ReturnsAsync(new List<CartItemViewModel>());

            var result = await controller.Index();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Add_ValidData_RedirectsToIndex()
        {
            var result = await controller.Add(1, 2);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(((RedirectToActionResult)result).ActionName, Is.EqualTo("Index"));
            mockCartService.Verify(s => s.AddToCartAsync(TestUserId, 1, 2), Times.Once);
        }

        [Test]
        public async Task Remove_ValidProductId_RedirectsToIndex()
        {
            var result = await controller.Remove(1);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(((RedirectToActionResult)result).ActionName, Is.EqualTo("Index"));
            mockCartService.Verify(s => s.RemoveFromCartAsync(TestUserId, 1), Times.Once);
        }

        [Test]
        public async Task Clear_ClearsCartAndRedirectsToIndex()
        {
            var result = await controller.Clear();

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(((RedirectToActionResult)result).ActionName, Is.EqualTo("Index") );
            mockCartService.Verify(s => s.ClearCartAsync(TestUserId), Times.Once);
        }

        [Test]
        public async Task Checkout_ReturnsViewWithCartItems()
        {
            mockCartService.Setup(s => s.GetCartItemsAsync(TestUserId))
                .ReturnsAsync(new List<CartItemViewModel>());

            var result = await controller.Checkout();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task CheckoutConfirm_WhenSuccessIsFalse_ShowsError()
        {
            mockCartService.Setup(s => s.CheckoutAsync(TestUserId)).ReturnsAsync(false);

            var result = await controller.CheckoutConfirm();

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(((RedirectToActionResult)result).ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task CheckoutConfirm_WhenSuccessIsTrue_RedirectsToThankYou()
        {
            mockCartService.Setup(s => s.CheckoutAsync(TestUserId)).ReturnsAsync(true);

            var result = await controller.CheckoutConfirm();

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(((RedirectToActionResult)result).ActionName, Is.EqualTo("ThankYou"));
        }

        [Test]
        public void ThankYou_ReturnsView()
        {
            var result = controller.ThankYou();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task History_ReturnsViewWithOrderHistory()
        {
            mockCartService.Setup(s => s.GetOrderHistoryAsync(TestUserId))
                .ReturnsAsync(new List<OrderHistoryViewModel>());

            var result = await controller.History();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Cancel_ValidOrderId_ReturnsViewWithModel()
        {
            var model = new CancelOrderViewModel { OrderId = 1 };

            mockCartService.Setup(s => s.GetCancelOrderViewModelAsync(TestUserId, 1))
                .ReturnsAsync(model);

            var result = await controller.Cancel(1);

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.That(((ViewResult)result).Model, Is.EqualTo(model));
        }

        [Test]
        public async Task Cancel_NullModel_ReturnsNotFoundPage()
        {
            mockCartService.Setup(s => s.GetCancelOrderViewModelAsync(TestUserId, 1))
                .ReturnsAsync((CancelOrderViewModel)null);

            var result = await controller.Cancel(1);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(((RedirectToActionResult)result).ActionName, Is.EqualTo("NotFoundPage"));
        }

        [Test]
        public async Task ConfirmCancel_InvalidCancel_ReturnsWithError()
        {
            var model = new CancelOrderViewModel { OrderId = 1 };

            mockCartService.Setup(s => s.CancelOrderAsync(TestUserId, model.OrderId))
                .ReturnsAsync(false);

            var result = await controller.ConfirmCancel(model);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(((RedirectToActionResult)result).ActionName, Is.EqualTo("History"));
        }

        [Test]
        public async Task ConfirmCancel_ValidCancel_SetsTempDataAndRedirects()
        {
            var model = new CancelOrderViewModel { OrderId = 1 };
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            mockCartService.Setup(s => s.CancelOrderAsync(TestUserId, model.OrderId))
                .ReturnsAsync(true);

            var result = await controller.ConfirmCancel(model);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(((RedirectToActionResult)result).ActionName, Is.EqualTo("History"));
        }
    }
}
