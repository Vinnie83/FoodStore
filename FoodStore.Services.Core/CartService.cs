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
    }
}
