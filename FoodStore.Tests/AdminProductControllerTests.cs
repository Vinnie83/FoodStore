using FoodStore.Areas.Admin.Controllers;
using FoodStore.Data.Models;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels.Admin;
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
    public class AdminProductControllerTests
    {
        private Mock<IAdminService> mockAdminService;
        private Mock<IProductService> mockProductService;
        private Mock<ICategoryService> mockCategoryService;
        private Mock<IBrandService> mockBrandService;
        private Mock<ISupplierService> mockSupplierService;
        private Mock<UserManager<ApplicationUser>> mockUserManager;
        private Mock<RoleManager<IdentityRole>> mockRoleManager;
        private ProductController controller;

        [SetUp]
        public void SetUp()
        {
            mockAdminService = new Mock<IAdminService>();
            mockProductService = new Mock<IProductService>();
            mockCategoryService = new Mock<ICategoryService>();
            mockBrandService = new Mock<IBrandService>();
            mockSupplierService = new Mock<ISupplierService>();

            var userStore = new Mock<IUserStore<ApplicationUser>>();
            mockUserManager = new Mock<UserManager<ApplicationUser>>(userStore.Object, null, null, null, null, null, null, null, null);

            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            mockRoleManager = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);

            controller = new ProductController(
                mockAdminService.Object,
                mockUserManager.Object,
                mockRoleManager.Object,
                mockProductService.Object,
                mockCategoryService.Object,
                mockBrandService.Object,
                mockSupplierService.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "admin-id")
            }));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
                           .Returns("admin-id");
        }

        [TearDown]
        public void TearDown()
        {
            mockAdminService = null!;
            mockProductService = null!;
            mockCategoryService = null!;
            mockBrandService = null!;
            mockSupplierService = null!;
            mockUserManager = null!;
            mockRoleManager = null!;
            controller.Dispose();
        }

        [Test]
        public async Task Create_Get_ReturnsViewWithPopulatedDropdowns()
        {
         
            mockCategoryService.Setup(x => x.GetCategoriesDropDownAsync()).ReturnsAsync(new List<AddCategoryDropDownMenu>());
            mockBrandService.Setup(x => x.GetBrandsDropDownAsync()).ReturnsAsync(new List<AddBrandDropDownMenu>());
            mockSupplierService.Setup(x => x.GetSuppliersDropDownAsync()).ReturnsAsync(new List<AddSupplierDropDownMenu>());

            
            var result = await controller.Create();

           
            Assert.That(result, Is.TypeOf<ViewResult>());
            var model = (result as ViewResult)?.Model as ProductCreateInputModel;
            Assert.IsNotNull(model);
        }

        [Test]
        public async Task Create_Post_ModelStateInvalid_ReturnsViewWithModel()
        {
           
            controller.ModelState.AddModelError("Name", "Required");
            var input = new ProductCreateInputModel();

            mockCategoryService.Setup(x => x.GetCategoriesDropDownAsync()).ReturnsAsync(new List<AddCategoryDropDownMenu>());
            mockBrandService.Setup(x => x.GetBrandsDropDownAsync()).ReturnsAsync(new List<AddBrandDropDownMenu>());
            mockSupplierService.Setup(x => x.GetSuppliersDropDownAsync()).ReturnsAsync(new List<AddSupplierDropDownMenu>());

            var result = await controller.Create(input);

          
            Assert.That(result, Is.TypeOf<ViewResult>());
            Assert.That(((ViewResult)result).Model, Is.EqualTo(input));
        }

        [Test]
        public async Task Create_Post_ValidData_ProductAdded_Redirects()
        {
           
            var input = new ProductCreateInputModel { CategoryId = 1 };

            mockProductService.Setup(x => x.AddProductAsync("admin-id", input)).ReturnsAsync(true);
            mockProductService.Setup(x => x.GetCategoryNameByIdAsync(1)).ReturnsAsync("Fruits");

          
            var result = await controller.Create(input);

            
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult, Is.Not.Null);
            Assert.That(redirectResult!.ActionName, Is.EqualTo("Category"));
            Assert.That(redirectResult.ControllerName, Is.EqualTo("Product"));
            Assert.That(redirectResult.RouteValues!["category"], Is.EqualTo("Fruits"));
        }

        [Test]
        public async Task Edit_Get_ReturnsViewWithModel()
        {
          
            var model = new EditProductInputModel { Id = 5 };
            mockProductService.Setup(x => x.GetProductForEditingAsync("admin-id", 5)).ReturnsAsync(model);
            mockCategoryService.Setup(x => x.GetCategoriesDropDownAsync()).ReturnsAsync(new List<AddCategoryDropDownMenu>());
            mockBrandService.Setup(x => x.GetBrandsDropDownAsync()).ReturnsAsync(new List<AddBrandDropDownMenu>());
            mockSupplierService.Setup(x => x.GetSuppliersDropDownAsync()).ReturnsAsync(new List<AddSupplierDropDownMenu>());

            
            var result = await controller.Edit(5);

           
            Assert.That(result, Is.TypeOf<ViewResult>());
            Assert.That(((ViewResult)result).Model, Is.EqualTo(model));
        }

        [Test]
        public async Task Edit_Post_ValidData_ProductUpdated_Redirects()
        {
            
            var input = new EditProductInputModel { Id = 6 };
            mockProductService.Setup(x => x.PersistUpdatedProductAsync("admin-id", input)).ReturnsAsync(true);

            
            var result = await controller.Edit(input);

            
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo("Details"));
        }

        [Test]
        public async Task Delete_Get_ReturnsViewWithModel()
        {
            
            var model = new ProductDeleteInputModel { Id = 1 };
            mockProductService.Setup(x => x.GetProductForDeletingAsync("admin-id", 1)).ReturnsAsync(model);

            
            var result = await controller.Delete(1);

            
            Assert.That(result, Is.TypeOf<ViewResult>());
            Assert.That(((ViewResult)result).Model, Is.EqualTo(model));
        }

        [Test]
        public async Task Delete_Post_Success_RedirectsToCategory()
        {
            
            var input = new ProductDeleteInputModel { Id = 1, CategoryName = "Drinks" };
            mockProductService.Setup(x => x.SoftDeleteProductAsync("admin-id", input)).ReturnsAsync(true);

            
            var result = await controller.Delete(input);

            
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo("Category"));
            Assert.That(redirect.RouteValues!["category"], Is.EqualTo("Drinks"));
        }
    }
}
