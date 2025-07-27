using FoodStore.Data;
using FoodStore.Data.Models;
using FoodStore.Data.Models.Enums;
using FoodStore.Services.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Tests.CartServiceTests
{
    public class CartServiceTests
    {
        private FoodStoreDbContext dbContext;
        private CartService cartService;
        private string userId = "someId";

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<FoodStoreDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

            dbContext = new FoodStoreDbContext(options);
            cartService = new CartService(dbContext);

            SeedData().Wait();
        }

        private async Task SeedData()
        {
            var categoryId = 1;
            var brandId = 1;
            var supplierId = 1;

            dbContext.Products.Add(new Product
            {
                Id = 1,
                Name = "Salami",
                ImageUrl = "imgsal.jpg",
                Price = 5.59m,
                Quantity = 13,
                CategoryId = categoryId,
                BrandId = brandId,
                SupplierId = supplierId
            });

            await dbContext.SaveChangesAsync();
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        [Test]
        public async Task GetActiveCartAsync_WithPendingOrder_ReturnsOrder()
        {
            var order = new Order
            {
                UserId = "user-1",
                OrderStatus = OrderStatus.Pending,
                Items = new List<OrderItem>()
            };
            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();

            var result = await cartService.GetActiveCartAsync("user-1");

            Assert.IsNotNull(result);
            Assert.That(result.UserId, Is.EqualTo("user-1"));
            Assert.That(result.OrderStatus, Is.EqualTo(OrderStatus.Pending));
        }

        [Test]
        public async Task AddToCartAsync_AddsNewItem_WhenNotExists()
        {
            await cartService.AddToCartAsync(userId, 1, 2);

            var cart = await cartService.GetActiveCartAsync(userId);

            Assert.IsNotNull(cart);
            Assert.That(cart.Items.Count, Is.EqualTo(1));
            Assert.That(cart.Items.First().Quantity, Is.EqualTo(2));
        }

        [Test]
        public async Task CreateCartAsync_CreatesNewOrder()
        {
            var result = await cartService.CreateCartAsync("user-1");

            Assert.IsNotNull(result);
            Assert.That(result.UserId, Is.EqualTo("user-1"));
            Assert.That(result.OrderStatus, Is.EqualTo(OrderStatus.Pending));
            Assert.That(result.PaymentStatus, Is.EqualTo(PaymentStatus.Pending));

            var dbOrder = await dbContext.Orders.FirstOrDefaultAsync();
            Assert.IsNotNull(dbOrder);
            Assert.That(dbOrder.Id, Is.EqualTo(result.Id));
        }

        [Test]
        public async Task AddToCartAsync_UpdatesQuantity_WhenItemExists()
        {
            await cartService.AddToCartAsync(userId, 1, 2);
            await cartService.AddToCartAsync(userId, 1, 3);

            var cart = await cartService.GetActiveCartAsync(userId);

            Assert.IsNotNull(cart);
            Assert.That(cart.Items.Count, Is.EqualTo(1));
            Assert.That(cart.Items.First().Quantity, Is.EqualTo(5));
        }

        [Test]
        public async Task GetCartItemsAsync_ReturnsCorrectViewModels()
        {
            await cartService.AddToCartAsync(userId, 1, 2);
            var items = await cartService.GetCartItemsAsync(userId);

            Assert.That(items.Count, Is.EqualTo(1));
            Assert.That(items[0].ProductName, Is.EqualTo("Salami"));
            Assert.That(items[0].Price, Is.EqualTo(5.59m));
            Assert.That(items[0].Quantity, Is.EqualTo(2));
        }

        [Test]
        public async Task RemoveFromCartAsync_RemovesItem()
        {
            await cartService.AddToCartAsync(userId, 1, 2);
            await cartService.RemoveFromCartAsync(userId, 1);

            var cart = await cartService.GetActiveCartAsync(userId);

            Assert.IsNotNull(cart);
            Assert.That(cart.Items.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task ClearCartAsync_RemovesAllItems()
        {
            await cartService.AddToCartAsync(userId, 1, 2);
            await cartService.ClearCartAsync(userId);

            var cart = await cartService.GetActiveCartAsync(userId);

            Assert.IsNotNull(cart);
            Assert.That(cart.Items.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task CheckoutAsync_ProcessesOrderAndReducesStock()
        {
            await cartService.AddToCartAsync(userId, 1, 2);
            var success = await cartService.CheckoutAsync(userId);

            var cart = await cartService.GetActiveCartAsync(userId);
            var product = await dbContext.Products.FindAsync(1);

            Assert.IsTrue(success);
            Assert.IsNull(cart);
            Assert.IsNotNull(product);
            Assert.That(product.Quantity, Is.EqualTo(11));
        }

        [Test]
        public async Task CancelOrderAsync_UpdatesOrderStatus()
        {
            await cartService.AddToCartAsync(userId, 1, 2);
            var cart = await cartService.GetActiveCartAsync(userId);

            var result = await cartService.CancelOrderAsync(userId, cart.Id);
            var updatedCart = await cartService.GetByIdAsync(cart.Id);

            Assert.IsTrue(result);
            Assert.IsNotNull(updatedCart);
            Assert.That(updatedCart.OrderStatus, Is.EqualTo(OrderStatus.Cancelled));
        }

        [Test]
        public async Task GetOrderHistoryAsync_ReturnsProcessedOrders()
        {
            await cartService.AddToCartAsync(userId, 1, 2);
            await cartService.CheckoutAsync(userId);

            var history = await cartService.GetOrderHistoryAsync(userId);

            Assert.That(history.Count, Is.EqualTo(1));
            Assert.That(history[0].OrderStatus, Is.EqualTo("Processed"));
            Assert.That(history[0].PaymentStatus, Is.EqualTo("Paid"));
        }

        [Test]
        public async Task GetCancelOrderViewModelAsync_ReturnsCorrectModel()
        {
            await cartService.AddToCartAsync(userId, 1, 2);
            var cart = await cartService.GetActiveCartAsync(userId);

            var result = await cartService.GetCancelOrderViewModelAsync(userId, cart.Id);

            Assert.IsNotNull(result);
            Assert.That(result.OrderId, Is.EqualTo(cart.Id));
            Assert.That(result.OrderStatus, Is.EqualTo("Pending"));
        }
    }
}
