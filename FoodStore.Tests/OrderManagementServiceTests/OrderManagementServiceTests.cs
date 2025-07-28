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

namespace FoodStore.Tests.OrderManagementServiceTests
{
    public class OrderManagementServiceTests
    {
        private FoodStoreDbContext _context;
        private OrderManagementService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FoodStoreDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new FoodStoreDbContext(options);
            _service = new OrderManagementService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task MarkOrderAsDeliveredAsync_ChangesStatus_WhenOrderIsProcessed()
        {
            var adminUser = new ApplicationUser
            {
                Id = "admin1",
                Email = "admin@foodstore.com"
            };
            await _context.Users.AddAsync(adminUser);

            var order = new Order
            {
                Id = 1,
                OrderStatus = OrderStatus.Processed,
                PaymentStatus = PaymentStatus.Paid,
                OrderDate = DateTime.Now,
                TotalAmount = 50,
                UserId = adminUser.Id
            };
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            var result = await _service.MarkOrderAsDeliveredAsync(order.Id);

            Assert.IsTrue(result);
            Assert.That(_context.Orders.First().OrderStatus, Is.EqualTo(OrderStatus.Delivered));
        }

        [Test]
        public async Task MarkOrderAsDeliveredAsync_ReturnsFalse_WhenOrderIsNotProcessed()
        {
            var adminUser = new ApplicationUser
            {
                Id = "admin1",
                Email = "admin@foodstore.com"
            };
            await _context.Users.AddAsync(adminUser);

            var order = new Order
            {
                Id = 2,
                OrderStatus = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Paid,
                UserId = adminUser.Id
            };
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            var result = await _service.MarkOrderAsDeliveredAsync(order.Id);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetOrderViewModelByIdAsync_ReturnsNull_WhenOrderNotFoundOrNotPaid()
        {
            var adminUser = new ApplicationUser
            {
                Id = "admin1",
                Email = "admin@foodstore.com"
            };
            await _context.Users.AddAsync(adminUser);

            var order = new Order
            {
                Id = 3,
                OrderStatus = OrderStatus.Processed,
                PaymentStatus = PaymentStatus.Pending,
                UserId = adminUser.Id
            };
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            var result = await _service.GetOrderViewModelByIdAsync(order.Id);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetOrderViewModelByIdAsync_ReturnsViewModel_WhenValid()
        {
            var adminUser = new ApplicationUser
            {
                Id = "admin1",
                Email = "admin@foodstore.com"
            };
            await _context.Users.AddAsync(adminUser);

            var product = new Product { Id = 1, Name = "Product", Price = 10, Quantity = 100 };
            await _context.Products.AddAsync(product);

            var order = new Order
            {
                Id = 4,
                UserId = adminUser.Id,
                User = adminUser,
                OrderStatus = OrderStatus.Processed,
                PaymentStatus = PaymentStatus.Paid,
                OrderDate = DateTime.Now,
                Items = new List<OrderItem>
                {
                    new OrderItem { Product = product, Price = 10, Quantity = 2 }
                }
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            var result = await _service.GetOrderViewModelByIdAsync(order.Id);

            Assert.IsNotNull(result);
            Assert.That(result.UserEmail, Is.EqualTo("admin@foodstore.com"));
            Assert.That(result.TotalAmount, Is.EqualTo("20.00 lv"));
        }

        [Test]
        public async Task GetAllProcessedAndDeliveredOrdersAsync_ReturnsFilteredOrders()
        {
            await _context.Orders.AddRangeAsync(
                new Order { Id = 5, OrderStatus = OrderStatus.Processed, OrderDate = DateTime.Now, TotalAmount = 100, 
                    User = new ApplicationUser { Email = "kalin@foodstore.com" }, PaymentStatus = PaymentStatus.Paid },
                new Order { Id = 6, OrderStatus = OrderStatus.Delivered, OrderDate = DateTime.Now, TotalAmount = 150,
                    User = new ApplicationUser { Email = "maria@foodstore.com" }, PaymentStatus = PaymentStatus.Paid },
                new Order { Id = 7, OrderStatus = OrderStatus.Pending, OrderDate = DateTime.Now, TotalAmount = 200, 
                    User = new ApplicationUser { Email = "ivan@foodstore.com" }, PaymentStatus = PaymentStatus.Paid }
            );
            await _context.SaveChangesAsync();

            var result = await _service.GetAllProcessedAndDeliveredOrdersAsync(null, 1, 10);

            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetOrdersByStatusAsync_ReturnsOrdersWithMatchingStatus()
        {
            await _context.Orders.AddRangeAsync(
                new Order { Id = 8, OrderStatus = OrderStatus.Processed, OrderDate = DateTime.Now, TotalAmount = 111, 
                    User = new ApplicationUser { Email = "marina@foodstore.com" }, PaymentStatus = PaymentStatus.Paid },
                new Order { Id = 9, OrderStatus = OrderStatus.Delivered, OrderDate = DateTime.Now, TotalAmount = 222, 
                    User = new ApplicationUser { Email = "niki@foodstore.com" }, PaymentStatus = PaymentStatus.Paid }
            );
            await _context.SaveChangesAsync();

            var result = await _service.GetOrdersByStatusAsync("Processed");

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().UserEmail, Is.EqualTo("marina@foodstore.com"));
        }
    }
}
