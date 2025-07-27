using FoodStore.Data.Models;
using FoodStore.Data;
using FoodStore.Services.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FoodStore.Tests.FavoritesServiceTests
{
    public class FavoritesServiceTests
    {
        private FoodStoreDbContext dbContext;
        private Mock<UserManager<ApplicationUser>> userManagerMock;
        private FavoritesService favoritesService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FoodStoreDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // isolate tests
                .Options;

            dbContext = new FoodStoreDbContext(options);

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object,
                null, null, null, null, null, null, null, null);

            favoritesService = new FavoritesService(dbContext, userManagerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        [Test]
        public async Task AddToFavoritesAsync_AddsProduct_WhenNotAlreadyFavorite()
        {
            var userId = "user1";
            var productId = 1;
            var categoryId = 4;
            var brandId = 2;
            var supplierId = 6;


            dbContext.Users.Add(new ApplicationUser { Id = userId });
            dbContext.Products.Add(new Product 
            { 
                Id = productId, 
                Name = "Milk",
                Price = 3.50m,
                Quantity = 20,
                CategoryId = categoryId,
                BrandId = brandId,
                SupplierId = supplierId,
       
            });
            await dbContext.SaveChangesAsync();

            userManagerMock.Setup(u => u.FindByIdAsync(userId))
                .ReturnsAsync(dbContext.Users.First());

            var result = await favoritesService.AddToFavoritesAsync(userId, productId);

            Assert.IsTrue(result);
            Assert.That(dbContext.UsersProducts.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task AddToFavoritesAsync_DoesNotAdd_IfAlreadyFavorite()
        {
            var userId = "user2";
            var productId = 2;
            var categoryId = 10;
            var brandId = 5;
            var supplierId = 4;


            dbContext.Users.Add(new ApplicationUser { Id = userId });
            dbContext.Products.Add(new Product 
            { 
                Id = productId,
                Name = "Cheese",
                Price = 2.50m,
                Quantity = 20,
                CategoryId = categoryId,
                BrandId = brandId,
                SupplierId = supplierId,
            });
            dbContext.UsersProducts.Add(new UserProduct 
            {
                UserId = userId, 
                ProductId = productId 
            
            });
            await dbContext.SaveChangesAsync();

            userManagerMock.Setup(u => u.FindByIdAsync(userId))
                .ReturnsAsync(dbContext.Users.First());

            var result = await favoritesService.AddToFavoritesAsync(userId, productId);

            Assert.IsFalse(result);
            Assert.That(dbContext.UsersProducts.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task RemoveFromFavoritesAsync_RemovesFavorite_IfExists()
        {
            var userId = "user3";
            var productId = 3;
            var categoryId = 4;
            var brandId = 3;
            var supplierId = 2;

            dbContext.Users.Add(new ApplicationUser { Id = userId });
            dbContext.Products.Add(new Product 
            { 
                Id = productId,
                Name = "Shampoo",
                Price = 5.50m,
                Quantity = 10,
                CategoryId = categoryId,
                BrandId = brandId,
                SupplierId = supplierId,


            });
            dbContext.UsersProducts.Add(new UserProduct { UserId = userId, ProductId = productId });
            await dbContext.SaveChangesAsync();

            userManagerMock.Setup(u => u.FindByIdAsync(userId))
                .ReturnsAsync(dbContext.Users.First());

            var result = await favoritesService.RemoveFromFavoritesAsync(userId, productId);

            Assert.IsTrue(result);
            Assert.That(dbContext.UsersProducts.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task RemoveFromFavoritesAsync_DoesNothing_IfNotFavorite()
        {
            var userId = "user4";
            var productId = 4;
            var categoryId = 8;
            var brandId = 1;
            var supplierId = 2;

            dbContext.Users.Add(new ApplicationUser { Id = userId });
            dbContext.Products.Add(new Product 
            { 
                Id = productId,
                Name = "Sausage",
                Price = 4.50m,
                Quantity = 15,
                CategoryId = categoryId,
                BrandId = brandId,
                SupplierId = supplierId,

            });
            await dbContext.SaveChangesAsync();

            userManagerMock.Setup(u => u.FindByIdAsync(userId))
                .ReturnsAsync(dbContext.Users.First());

            var result = await favoritesService.RemoveFromFavoritesAsync(userId, productId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetUserFavoritesAsync_ReturnsCorrectFavorites()
        {
            var userId = "user5";
            var categoryId = 5;
            var brandId = 4;
            var supplierId = 3;

            var product = new Product 
            { 
                Id = 5, 
                Name = "Yogurt", 
                ImageUrl = "img.jpg", 
                Price = 3.99m,
                Quantity = 18,
                CategoryId = categoryId,
                BrandId = brandId,
                SupplierId = supplierId,
            };

            dbContext.Users.Add(new ApplicationUser { Id = userId });
            dbContext.Products.Add(product);
            dbContext.UsersProducts.Add(new UserProduct { UserId = userId, ProductId = product.Id });
            await dbContext.SaveChangesAsync();

            userManagerMock.Setup(u => u.FindByIdAsync(userId))
                .ReturnsAsync(dbContext.Users.First());

            var result = await favoritesService.GetUserFavoritesAsync(userId);

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().ProductName, Is.EqualTo("Yogurt"));
        }

        [Test]
        public async Task IsFavoriteAsync_ReturnsTrue_IfFavoriteExists()
        {
            var userId = "user6";
            var categoryId = 3;
            var brandId = 3;
            var supplierId = 1;

            var productId = 6;

            dbContext.Users.Add(new ApplicationUser { Id = userId });
            dbContext.Products.Add(new Product 
            { 
                Id = productId,
                Name = "Salami",
                ImageUrl = "imgsal.jpg",
                Price = 5.59m,
                Quantity = 13,
                CategoryId = categoryId,
                BrandId = brandId,
                SupplierId = supplierId,
            });

            dbContext.UsersProducts.Add(new UserProduct { UserId = userId, ProductId = productId });
            await dbContext.SaveChangesAsync();

            userManagerMock.Setup(u => u.FindByIdAsync(userId))
                .ReturnsAsync(dbContext.Users.First());

            var result = await favoritesService.IsFavoriteAsync(userId, productId);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task IsFavoriteAsync_ReturnsFalse_IfNotFavorite()
        {
            var userId = "user7";
            var categoryId = 4;
            var brandId = 1;
            var supplierId = 5;

            var productId = 7;

            dbContext.Users.Add(new ApplicationUser { Id = userId });
            dbContext.Products.Add(new Product 
            { 
                Id = productId,
                Name = "Potato",
                ImageUrl = "img-pot.jpg",
                Price = 2.59m,
                Quantity = 19,
                CategoryId = categoryId,
                BrandId = brandId,
                SupplierId = supplierId


            });
            await dbContext.SaveChangesAsync();

            userManagerMock.Setup(u => u.FindByIdAsync(userId))
                .ReturnsAsync(dbContext.Users.First());

            var result = await favoritesService.IsFavoriteAsync(userId, productId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task IsFavoriteAsync_ReturnsFalse_IfUserNotFound()
        {
            var result = await favoritesService.IsFavoriteAsync("invalid-user", 1);
            Assert.IsFalse(result);
        }
    }
}
