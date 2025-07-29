using FoodStore.Controllers;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels;
using Microsoft.AspNetCore.Http;
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
    public class FavoritesControllerTest
    {
        public class FavoritesControllerTests
        {
            private Mock<IFavoritesService> favoritesServiceMock;
            private FavoritesController controller;

            [SetUp]
            public void SetUp()
            {
                favoritesServiceMock = new Mock<IFavoritesService>();

                controller = new FavoritesController(favoritesServiceMock.Object);

                // Mock user identity
                var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id")
                }, "mock"));

                controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                };
            }

            [TearDown]
            public void TearDown()
            {
                controller.Dispose();
            }

            [Test]
            public async Task Index_ReturnsViewResultWithFavorites()
            {
    
                var expectedFavorites = new List<FavoriteProductViewModel>();
                favoritesServiceMock
                    .Setup(s => s.GetUserFavoritesAsync("test-user-id"))
                    .ReturnsAsync(expectedFavorites);

    
                var result = await controller.Index();

      
                var viewResult = result as ViewResult;
                Assert.IsNotNull(viewResult);
                Assert.That(viewResult.Model, Is.EqualTo(expectedFavorites));
            }

            [Test]
            public async Task Index_WhenExceptionThrown_RedirectsToError()
            {
    
                favoritesServiceMock
                    .Setup(s => s.GetUserFavoritesAsync(It.IsAny<string>()))
                    .ThrowsAsync(new Exception());

  
                var result = await controller.Index();

                var redirectResult = result as RedirectToActionResult;
                Assert.IsNotNull(redirectResult);
                Assert.That(redirectResult.ActionName, Is.EqualTo("ServerError"));
                Assert.That(redirectResult.ControllerName, Is.EqualTo("Error"));
            }

            [Test]
            public async Task Add_ValidProductId_RedirectsToIndex()
            {
      
                int productId = 42;

     
                var result = await controller.Add(productId);


                favoritesServiceMock.Verify(s => s.AddToFavoritesAsync("test-user-id", productId), Times.Once);
                var redirectResult = result as RedirectToActionResult;
                Assert.IsNotNull(redirectResult);
                Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            }

            [Test]
            public async Task Add_WhenExceptionThrown_RedirectsToError()
            {
    
                favoritesServiceMock
                    .Setup(s => s.AddToFavoritesAsync(It.IsAny<string>(), It.IsAny<int>()))
                    .ThrowsAsync(new Exception());

     
                var result = await controller.Add(1);

  
                var redirectResult = result as RedirectToActionResult;
                Assert.IsNotNull(redirectResult);
                Assert.That(redirectResult.ActionName, Is.EqualTo("ServerError"));
                Assert.That(redirectResult.ControllerName, Is.EqualTo("Error"));
            }

            [Test]
            public async Task Remove_ValidProductId_RedirectsToIndex()
            {

                int productId = 101;

       
                var result = await controller.Remove(productId);

      
                favoritesServiceMock.Verify(s => s.RemoveFromFavoritesAsync("test-user-id", productId), Times.Once);
                var redirectResult = result as RedirectToActionResult;
                Assert.IsNotNull(redirectResult);
                Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            }

            [Test]
            public async Task Remove_WhenExceptionThrown_RedirectsToError()
            {
      
                favoritesServiceMock
                    .Setup(s => s.RemoveFromFavoritesAsync(It.IsAny<string>(), It.IsAny<int>()))
                    .ThrowsAsync(new Exception());

    
                var result = await controller.Remove(1);

           
                var redirectResult = result as RedirectToActionResult;
                Assert.IsNotNull(redirectResult);
                Assert.That(redirectResult.ActionName, Is.EqualTo("ServerError"));
                Assert.That(redirectResult.ControllerName, Is.EqualTo("Error"));
            }

            [Test]
            public async Task Index_WithoutUserId_RedirectsToLogin()
            {

                controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(); 

            
                var result = await controller.Index();

                
                var redirect = result as RedirectResult;
                Assert.IsNotNull(redirect);
                Assert.That(redirect.Url, Is.EqualTo("/Identity/Account/Login"));
            }

            [Test]
            public async Task Add_WithoutUserId_RedirectsToLogin()
            {
                controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(); // No identity

                var result = await controller.Add(1);

                var redirect = result as RedirectResult;
                Assert.IsNotNull(redirect);
                Assert.That(redirect.Url, Is.EqualTo("/Identity/Account/Login"));
            }

            [Test]
            public async Task Remove_WithoutUserId_RedirectsToLogin()
            {
                controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(); // No identity

                var result = await controller.Remove(1);

                var redirect = result as RedirectResult;
                Assert.IsNotNull(redirect);
                Assert.That(redirect.Url, Is.EqualTo("/Identity/Account/Login"));
            }
        }
    }
}
