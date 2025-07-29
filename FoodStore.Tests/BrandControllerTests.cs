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
    public class BrandControllerTests
    {
        private Mock<IAdminService> mockAdminService;
        private Mock<IBrandService> mockBrandService;
        private Mock<UserManager<ApplicationUser>> mockUserManager;
        private Mock<RoleManager<IdentityRole>> mockRoleManager;
        private BrandController controller;

        [SetUp]
        public void SetUp()
        {
            mockAdminService = new Mock<IAdminService>();
            mockBrandService = new Mock<IBrandService>();
            mockUserManager = MockUserManager();
            mockRoleManager = MockRoleManager();

            controller = new BrandController(
                mockAdminService.Object,
                mockUserManager.Object,
                mockRoleManager.Object,
                mockBrandService.Object);

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
            mockBrandService = null;
            mockUserManager = null;
            mockRoleManager = null;
            controller.Dispose();
        }

        [Test]
        public async Task Index_ReturnsViewWithBrands()
        {

            var mockPaginated = new PaginatedList<BrandViewModel>(new List<BrandViewModel>(), 0, 1, 10);
            mockBrandService.Setup(s => s.GetAllBrandsAsync(1, 10)).ReturnsAsync(mockPaginated);

            var result = await controller.Index();


            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOf<PaginatedList<BrandViewModel>>(viewResult.Model);
        }

        [Test]
        public void AddBrand_Get_ReturnsView()
        {
            var result = controller.AddBrand();

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.ViewName, Is.EqualTo("AddBrand"));
        }

        [Test]
        public async Task AddBrand_Post_ValidModel_RedirectsToIndex()
        {
            var input = new AddBrandInputModel { Name = "TestBrand" };
            mockBrandService.Setup(s => s.AddBrandAsync("user123", input)).ReturnsAsync(true);

            var result = await controller.AddBrand(input);

            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task AddBrand_Post_InvalidModel_ReturnsViewWithModel()
        {
            controller.ModelState.AddModelError("Name", "Required");

            var input = new AddBrandInputModel();

            var result = await controller.AddBrand(input);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.Model, Is.EqualTo(input));
        }

        [Test]
        public async Task EditBrand_Get_ReturnsViewWithModel()
        {
            var mockModel = new EditBrandInputModel { Id = 1 };
            mockBrandService.Setup(s => s.GetBrandForEditingAsync("user123", 1)).ReturnsAsync(mockModel);

            var result = await controller.EditBrand(1);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.Model, Is.EqualTo(mockModel));
        }

        [Test]
        public async Task EditBrand_Post_ValidModel_RedirectsToIndex()
        {
            var input = new EditBrandInputModel { Id = 1, Name = "Updated" };
            mockBrandService.Setup(s => s.EditBrandAsync("user123", input)).ReturnsAsync(true);

            var result = await controller.EditBrand(input);

            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task Delete_Get_ReturnsViewWithModel()
        {
            var mockDeleteModel = new BrandDeleteViewModel { Id = 1 };
            mockBrandService.Setup(s => s.GetBrandForDeletingAsync("user123", 1)).ReturnsAsync(mockDeleteModel);

            var result = await controller.Delete(1);

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.Model, Is.EqualTo(mockDeleteModel));
        }

        [Test]
        public async Task ConfirmDelete_Post_ValidModel_RedirectsToIndex()
        {
            var input = new BrandDeleteViewModel { Id = 1 };
            mockBrandService.Setup(s => s.SoftDeleteBrandAsync("user123", input)).ReturnsAsync(true);

            var result = await controller.ConfirmDelete(input);

            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
        }

        private Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            mgr.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("user123");

            return mgr;
        }

        private Mock<RoleManager<IdentityRole>> MockRoleManager()
        {
            var store = new Mock<IRoleStore<IdentityRole>>();
            var mgr = new Mock<RoleManager<IdentityRole>>(store.Object, null, null, null, null);
            return mgr;
        }
    }
}
