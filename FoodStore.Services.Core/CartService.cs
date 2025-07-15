using FoodStore.Data;
using FoodStore.Data.Models;
using FoodStore.Data.Models.Enums;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FoodStore.Services.Core
{
    public class CartService : ICartService
    {
        private readonly FoodStoreDbContext dbContext;


        public CartService(FoodStoreDbContext dbContext)
        {
            this.dbContext = dbContext;       
        }

        public async Task<Order> GetOrCreateCartAsync(string userId)
        {
            var cart = await this.dbContext
                .Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.UserId == userId && o.OrderStatus == OrderStatus.Pending);
                
            if (cart == null)
            {
                cart = new Order()
                {
                    UserId = userId,
                    OrderStatus = OrderStatus.Pending,
                    PaymentStatus = PaymentStatus.Pending,
                    OrderDate = DateTime.UtcNow,
                };

                dbContext.Orders.Add(cart);
                await dbContext.SaveChangesAsync();
            }

            return cart;

        }
        public async Task AddToCartAsync(string userId, int productId, int quantity)
        {
            var cart = await GetOrCreateCartAsync(userId);

            var existingItem = cart.Items.FirstOrDefault(i =>  i.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var product = await dbContext.Products.FindAsync(productId);
                if (product == null) throw new Exception("Product not found");

                cart.Items.Add(new OrderItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.Price
                });
            }

            await dbContext.SaveChangesAsync();
        }


        public async Task<List<CartItemViewModel>> GetCartItemsAsync(string userId)
        {
            var cart = await GetOrCreateCartAsync(userId);

            return cart.Items.Select(i => new CartItemViewModel
            {
                ProductId = i.ProductId ?? 0,
                ProductName = i.Product!.Name,
                Price = i.Price,
                Quantity = i.Quantity
            })
            .ToList();
        }

        public async Task RemoveFromCartAsync(string userId, int productId)
        {
            var cart = await dbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.UserId == userId && o.OrderStatus == OrderStatus.Pending);

            if (cart == null) return;

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                dbContext.OrderItems.Remove(item); 
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await GetOrCreateCartAsync(userId);

            var itemsToRemove = cart.Items.ToList();

            dbContext.OrderItems.RemoveRange(itemsToRemove);

            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> CheckoutAsync(string userId)
        {
            var cart = await GetOrCreateCartAsync(userId);

            if (!cart.Items.Any())
            {
                return false;
            }

            foreach (var item in cart.Items)
            {
                var product = await dbContext.Products.FindAsync(item.ProductId);

                if (product == null)
                {
                    throw new Exception($"Product with ID {item.ProductId} not found.");
                }

                if (product.Quantity < item.Quantity)
                {
                    throw new Exception($"Insufficient stock for product: {product.Name}");
                }

                product.Quantity -= item.Quantity;
            }


            cart.OrderStatus = OrderStatus.Processed;
            cart.OrderDate = DateTime.UtcNow;
            cart.TotalAmount = cart.Items.Sum(i => i.Price * i.Quantity);
            cart.PaymentStatus = PaymentStatus.Paid;

            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<OrderHistoryViewModel>> GetOrderHistoryAsync(string userId)
        {
            var orders = await this.dbContext
                .Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderHistoryViewModel()
                {
                    OrderId = o.Id,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    OrderStatus = o.OrderStatus.ToString(),
                    PaymentStatus = o.PaymentStatus.ToString()

                })
                .ToListAsync();

            return orders;
        }
    }
}
