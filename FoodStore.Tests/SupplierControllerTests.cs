using FoodStore.Areas.Admin.Controllers;
using FoodStore.Data.Models;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels.Admin;
using FoodStore.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Tests
{
    public class SupplierControllerTests
    {
        private Mock<IAdminService> mockAdminService;
        private Mock<ISupplierService> mockSupplierService;
        private Mock<UserManager<ApplicationUser>> mockUserManager;
        private Mock<RoleManager<IdentityRole>> mockRoleManager;
        private SupplierController controller;

        [SetUp]
        public void SetUp()
        {
            mockAdminService = new Mock<IAdminService>();
            mockSupplierService = new Mock<ISupplierService>();
            mockUserManager = GetMockUserManager();
            mockRoleManager = GetMockRoleManager();

            controller = new SupplierController(
                mockAdminService.Object,
                mockUserManager.Object,
                mockRoleManager.Object,
                mockSupplierService.Object
            );

          
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user123")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [TearDown]
        public void TearDown()
        {
            mockAdminService = null;
            mockSupplierService = null;
            mockUserManager = null;
            mockRoleManager = null;
            controller.Dispose();
        }

        [Test]
        public async Task Index_ReturnsViewWithSuppliers()
        {
            var expected = new PaginatedList<SupplierViewModel>(new List<SupplierViewModel>(), 0, 1, 10);
            mockSupplierService.Setup(s => s.GetAllSuppliersAsync(1, 10)).ReturnsAsync(expected);

            var result = await controller.Index();

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.Model, Is.EqualTo(expected));
        }

        [Test]
        public void AddSupplier_Get_ReturnsView()
        {
            var result = controller.AddSupplier();

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.ViewName, Is.EqualTo("AddSupplier"));
        }

        [Test]
        public async Task AddSupplier_Post_ValidModel_RedirectsToIndex()
        {
            var model = new AddSupplierInputModel { Name = "Test" };
            mockSupplierService.Setup(s => s.AddSupplierAsync("user123", model)).ReturnsAsync(true);

            var result = await controller.AddSupplier(model);

            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task AddSupplier_Post_InvalidModel_ReturnsView()
        {
            controller.ModelState.AddModelError("Name", "Required");
            var model = new AddSupplierInputModel();

            var result = await controller.AddSupplier(model);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.Model, Is.EqualTo(model));
        }

        [Test]
        public async Task EditSupplier_Get_ReturnsViewWithModel()
        {
            var editModel = new EditSupplierInputModel { Id = 1, Name = "Supplier A" };
            mockSupplierService.Setup(s => s.GetSupplierForEditingAsync("user123", 1)).ReturnsAsync(editModel);

            var result = await controller.EditSupplier(1);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.Model, Is.EqualTo(editModel));
        }

        [Test]
        public async Task EditSupplier_Post_ValidModel_RedirectsToIndex()
        {
            var input = new EditSupplierInputModel { Id = 1, Name = "Updated" };
            mockSupplierService.Setup(s => s.EditSupplierAsync("user123", input)).ReturnsAsync(true);

            var result = await controller.EditSupplier(input);

            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task Delete_Get_ReturnsViewWithModel()
        {
            var deleteModel = new SupplierDeleteViewModel { Id = 1, Name = "Test Supplier" };
            mockSupplierService.Setup(s => s.GetSupplierForDeletingAsync("user123", 1)).ReturnsAsync(deleteModel);

            var result = await controller.Delete(1);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.Model, Is.EqualTo(deleteModel));
        }

        [Test]
        public async Task ConfirmDelete_Post_ValidModel_RedirectsToIndex()
        {
            var model = new SupplierDeleteViewModel { Id = 1 };
            mockSupplierService.Setup(s => s.SoftDeleteSupplierAsync("user123", model)).ReturnsAsync(true);

            var result = await controller.ConfirmDelete(model);

            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }

       
        private Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("user123");
            return mgr;
        }

        private Mock<RoleManager<IdentityRole>> GetMockRoleManager()
        {
            var store = new Mock<IRoleStore<IdentityRole>>();
            return new Mock<RoleManager<IdentityRole>>(store.Object, null, null, null, null);
        }
    }
}
