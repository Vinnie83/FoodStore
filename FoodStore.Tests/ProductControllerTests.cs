using FoodStore.Controllers;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Tests
{
    public class ProductControllerTests
    {
        private Mock<IProductService> mockProductService;
        private ProductController controller;

        [SetUp]
        public void SetUp()
        {
            mockProductService = new Mock<IProductService>();
            controller = new ProductController(mockProductService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            mockProductService = null;
            controller.Dispose();
        }

        [Test]
        public async Task Category_WithNullOrWhitespaceCategory_RedirectsToNotFound()
        {

            var result1 = await controller.Category(null);
            var result2 = await controller.Category("");
            var result3 = await controller.Category("   ");


            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<RedirectToActionResult>(result1);
                Assert.IsInstanceOf<RedirectToActionResult>(result2);
                Assert.IsInstanceOf<RedirectToActionResult>(result3);

                Assert.That(((RedirectToActionResult)result1).ActionName, Is.EqualTo("NotFoundPage"));
                Assert.That(((RedirectToActionResult)result1).ControllerName, Is.EqualTo("Error"));

                Assert.That(((RedirectToActionResult)result2).ActionName, Is.EqualTo("NotFoundPage"));
                Assert.That(((RedirectToActionResult)result2).ControllerName, Is.EqualTo("Error"));

                Assert.That(((RedirectToActionResult)result3).ActionName, Is.EqualTo("NotFoundPage"));
                Assert.That(((RedirectToActionResult)result3).ControllerName, Is.EqualTo("Error"));
            });
        }

        [Test]
        public async Task Category_ValidCategory_ReturnsViewWithModel()
        {

            string category = "Fruits";
            int page = 1;
            int pageSize = 4;

            var productList = new List<ProductViewModel>
            {
                new ProductViewModel { Id = 1, Name = "Apple" },
                new ProductViewModel { Id = 2, Name = "Banana" }
            };

            var paginatedList = new PaginatedList<ProductViewModel>(
                productList, productList.Count, page, pageSize);


            mockProductService
           .Setup(s => s.GetByCategoryAsync(category, page, pageSize))
           .ReturnsAsync(paginatedList);


            var result = await controller.Category(category, page);
            
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as CategoryProductsViewModel;
            Assert.IsNotNull(model);
            Assert.That(model.CategoryName, Is.EqualTo(category));
            Assert.That(model.Products.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task Category_ServiceThrowsException_RedirectsToServerError()
        {

            var category = "vegetables";
            mockProductService
                .Setup(s => s.GetByCategoryAsync(category, It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception());


            var result = await controller.Category(category);

            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.That(redirectResult.ActionName, Is.EqualTo("ServerError"));
            Assert.That(redirectResult.ControllerName, Is.EqualTo("Error"));
        }

        [Test]
        public async Task Details_ValidId_ReturnsViewWithProduct()
        {
      
            int productId = 5;
            var expectedProduct = new ProductDetailsViewModel
            {
                Id = productId,
                Name = "Orange",
                ImageUrl = "/images/orange.jpg",
                Price = 1.99m,
                StockQuantity = 10,
                UnitQuantity = "1",
                Brand = "FruitBrand",
                Category = "Fruits"
            };

            mockProductService
                .Setup(s => s.GetProductByIdAsync(productId))
                .ReturnsAsync(expectedProduct);

  
            var result = await controller.Details(productId);

      
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as ProductDetailsViewModel;
            Assert.IsNotNull(model);
            Assert.That(model.Id, Is.EqualTo(expectedProduct.Id));
            Assert.That(model.Name, Is.EqualTo(expectedProduct.Name));
        }

        [Test]
        public async Task Details_ProductNotFound_RedirectsToNotFoundPage()
        {
        
            int productId = 10;

            mockProductService
                .Setup(s => s.GetProductByIdAsync(productId))
                .ReturnsAsync((ProductDetailsViewModel)null!);

          
            var result = await controller.Details(productId);

            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.That(redirectResult.ActionName, Is.EqualTo("NotFoundPage"));
            Assert.That(redirectResult.ControllerName, Is.EqualTo("Error"));
        }

        [Test]
        public async Task Details_ServiceThrowsException_RedirectsToServerError()
        {

            int productId = 8;
            mockProductService
                .Setup(s => s.GetProductByIdAsync(productId))
                .ThrowsAsync(new Exception());


            var result = await controller.Details(productId);


            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.That(redirectResult.ActionName, Is.EqualTo("ServerError"));
            Assert.That(redirectResult.ControllerName, Is.EqualTo("Error"));
        }


        [Test]
        public async Task Search_NullOrWhitespaceQuery_ReturnsEmptyViewModel()
        {

            var result1 = await controller.Search(null);
            var result2 = await controller.Search("");
            var result3 = await controller.Search("   ");

            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<ViewResult>(result1);
                Assert.IsInstanceOf<ViewResult>(result2);
                Assert.IsInstanceOf<ViewResult>(result3);

                Assert.IsEmpty(((ViewResult)result1).Model as IEnumerable<ProductSearchResultViewModel>);
                Assert.IsEmpty(((ViewResult)result2).Model as IEnumerable<ProductSearchResultViewModel>);
                Assert.IsEmpty(((ViewResult)result3).Model as IEnumerable<ProductSearchResultViewModel>);
            });
        }

        [Test]
        public async Task Search_ValidQuery_ReturnsViewWithResults()
        {

            var query = "apple";
            var results = new List<ProductSearchResultViewModel>
            {
                new ProductSearchResultViewModel { Id = 1, Name = "Apple" }
            };

            mockProductService
                .Setup(s => s.SearchProductsAsync(query))
                .ReturnsAsync(results);


            var result = await controller.Search(query);


            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.Model, Is.EqualTo(results));
        }

        [Test]
        public async Task Search_ServiceReturnsNull_ReturnsEmptyEnumerable()
        {

            var query = "orange";

            mockProductService
                .Setup(s => s.SearchProductsAsync(query))
                .ReturnsAsync((IEnumerable<ProductSearchResultViewModel>)null);


            var result = await controller.Search(query);


            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as IEnumerable<ProductSearchResultViewModel>;
            Assert.IsNotNull(model);
            Assert.IsEmpty(model);
        }

        [Test]
        public async Task Search_ServiceThrowsException_RedirectsToServerError()
        {

            var query = "banana";

            mockProductService
                .Setup(s => s.SearchProductsAsync(query))
                .ThrowsAsync(new Exception());


            var result = await controller.Search(query);

            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.That(redirectResult.ActionName, Is.EqualTo("ServerError"));
            Assert.That(redirectResult.ControllerName, Is.EqualTo("Error"));
        }
    }

}

