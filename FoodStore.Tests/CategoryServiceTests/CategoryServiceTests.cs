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

namespace FoodStore.Tests.CategoryServiceTests
{
    public class CategoryServiceTests
    {
        private FoodStoreDbContext dbContext;
        private Mock<UserManager<ApplicationUser>> userManagerMock;
        private CategoryService categoryService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FoodStoreDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            dbContext = new FoodStoreDbContext(options);

           categoryService = new CategoryService(dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        [Test]
        public async Task GetCategoriesDropDownAsync_ReturnsCorrectData()
        {

            dbContext.Categories.AddRange
                (
                    new Category { Id = 1, Name = "Meat" },
                    new Category { Id = 2, Name = "Bakery" }
                );

            await dbContext.SaveChangesAsync();

  
            var result = await categoryService.GetCategoriesDropDownAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.IsTrue(result.Any(c => c.CategoryName == "Meat"));
            Assert.IsTrue(result.Any(c => c.CategoryName == "Bakery"));
        }

        [Test]
        public async Task GetCategoriesDropDownAsync_ReturnsEmpty_WhenNoCategoriesExist()
        {
            
            var result = await categoryService.GetCategoriesDropDownAsync();

            
            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(0));
        }


    }
}
