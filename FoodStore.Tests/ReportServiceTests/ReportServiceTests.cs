using FoodStore.Data.Models.Enums;
using FoodStore.Data.Models;
using FoodStore.Data;
using FoodStore.Services.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Tests.ReportServiceTests
{
    public class ReportServiceTests
    {
        private FoodStoreDbContext dbContext;
        private ReportService reportService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FoodStoreDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
                .Options;

            dbContext = new FoodStoreDbContext(options);
            reportService = new ReportService(dbContext);

            SeedData().Wait();
        }

        private async Task SeedData()
        {
            var user = new ApplicationUser { Id = "user1", Email = "niki@foodstore.com" };
            var brand = new Brand { Id = 1, Name = "Coca Cola", CountryOfOrigin = "Bulgaria" };
            var supplier = new Supplier { Id = 1, Name = "Maxtrade", Phone = "0898657834", EmailAddress = "maxtrade@abv.bg" };
            var category = new Category { Id = 1, Name = "Beverages" };
            var product = new Product
            {
                Id = 1,
                Name = "Coca Cola 1 lt",
                Price = 3.50m,
                Quantity = 100,
                Brand = brand,
                Supplier = supplier,
                Category = category
            };

            var order = new Order
            {
                Id = 1,
                User = user,
                OrderDate = DateTime.Now,
                OrderStatus = OrderStatus.Processed,
                Items = new List<OrderItem>
                {
                    new OrderItem { Product = product, Quantity = 2 }
                }
            };

            dbContext.Users.Add(user);
            dbContext.Brands.Add(brand);
            dbContext.Suppliers.Add(supplier);
            dbContext.Categories.Add(category);
            dbContext.Products.Add(product);
            dbContext.Orders.Add(order);

            await dbContext.SaveChangesAsync();
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        [Test]
        public async Task GetOrderReportsAsync_ReturnsCorrectData()
        {
            var result = await reportService.GetOrderReportsAsync(null, 1, 10);

            Assert.That(result.Reports.Count, Is.EqualTo(1));
            Assert.That(result.UserEmails, Contains.Item("niki@foodstore.com"));
            Assert.That(result.Categories, Contains.Item("Beverages"));
        }

        [Test]
        public async Task GetOrderDetailsAsync_ReturnsOrderDetails()
        {
            var result = await reportService.GetOrderDetailsAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.UserEmail, Is.EqualTo("niki@foodstore.com"));
            Assert.That(result.TotalPrice, Is.EqualTo(7)); 
        }

        [Test]
        public async Task ExportOrdersToExcelAsync_ReturnsExcelByteArray()
        {
            var result = await reportService.ExportOrdersToExcelAsync(null);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.GreaterThan(0));
        }

        [Test]
        public async Task GetAllOrderReportsAsync_ReturnsList()
        {
            var result = await reportService.GetAllOrderReportsAsync(null);

            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetProductReportsAsync_ReturnsProductReports()
        {
            var result = await reportService.GetProductReportsAsync(null, 1, 10);

            Assert.That(result.Reports.Count, Is.EqualTo(1));
            Assert.That(result.Reports[0].TotalOrderedQuantity, Is.EqualTo(2));
            Assert.That(result.Reports[0].TotalRevenue, Is.EqualTo(7));
        }

        [Test]
        public async Task ExportProductsToExcelAsync_ReturnsExcelByteArray()
        {
            var result = await reportService.ExportProductsToExcelAsync(null);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.GreaterThan(0));
        }
    }
}
