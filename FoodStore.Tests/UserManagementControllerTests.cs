using FoodStore.Areas.Admin.Controllers;
using FoodStore.Data.Models;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels;
using FoodStore.ViewModels.Admin;
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
    public class UserManagementControllerTests
    {
        private Mock<IAdminService> mockAdminService;
        private Mock<UserManager<ApplicationUser>> mockUserManager;
        private Mock<RoleManager<IdentityRole>> mockRoleManager;

        private UserManagementController controller;

        [SetUp]
        public void SetUp()
        {
            mockAdminService = new Mock<IAdminService>();

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            mockUserManager = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                roleStoreMock.Object, null, null, null, null);

            controller = new UserManagementController(
                mockAdminService.Object,
                mockUserManager.Object,
                mockRoleManager.Object);
        }

        [TearDown]
        public void TearDown()
        {
            mockAdminService = null!;
            mockUserManager = null!;
            mockRoleManager = null!;
            controller.Dispose();
        }

        [Test]
        public async Task Index_ReturnsViewWithUsers()
        {
            var viewModel = new PaginatedList<UserViewModel>(new List<UserViewModel>(), 0, 1, 5);

            mockAdminService
                .Setup(s => s.GetAllUsersAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(viewModel);

            var result = await controller.Index(1) as ViewResult;

            Assert.IsNotNull(result);
            Assert.That(result.Model, Is.EqualTo(viewModel));
        }

        [Test]
        public async Task Delete_ValidUserId_DeletesUserAndRedirects()
        {
            var user = new ApplicationUser { Id = "123" };

            mockUserManager.Setup(um => um.FindByIdAsync("123"))
                .ReturnsAsync(user);
            mockUserManager.Setup(um => um.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            var result = await controller.Delete("123") as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.That(result.ActionName, Is.EqualTo(nameof(UserManagementController.Index)));
        }

        [Test]
        public async Task AddToAdmin_UserNotInRole_AddsToAdminRole()
        {
            var user = new ApplicationUser { Id = "123" };

            mockUserManager.Setup(um => um.FindByIdAsync("123"))
                .ReturnsAsync(user);
            mockUserManager.Setup(um => um.IsInRoleAsync(user, "Admin"))
                .ReturnsAsync(false);
            mockRoleManager.Setup(rm => rm.RoleExistsAsync("Admin"))
                .ReturnsAsync(false);
            mockRoleManager.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(um => um.AddToRoleAsync(user, "Admin"))
                .ReturnsAsync(IdentityResult.Success);

            var result = await controller.AddToAdmin("123") as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.That(result.ActionName, Is.EqualTo(nameof(UserManagementController.Index)));
        }

        [Test]
        public async Task RemoveFromAdmin_UserInRole_RemovesFromAdminRole()
        {
            var user = new ApplicationUser { Id = "123" };

            mockUserManager.Setup(um => um.FindByIdAsync("123"))
                .ReturnsAsync(user);
            mockUserManager.Setup(um => um.IsInRoleAsync(user, "Admin"))
                .ReturnsAsync(true);
            mockUserManager.Setup(um => um.RemoveFromRoleAsync(user, "Admin"))
                .ReturnsAsync(IdentityResult.Success);

            var result = await controller.RemoveFromAdmin("123") as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.That(result.ActionName, Is.EqualTo(nameof(UserManagementController.Index)));
        }

        [Test]
        public void Register_ReturnsView()
        {
            var result = controller.Register() as ViewResult;

            Assert.IsNotNull(result);
        }
    }
}
