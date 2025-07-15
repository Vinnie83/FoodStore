using FoodStore.Data;
using FoodStore.Data.Models;
using FoodStore.Data.Models.Enums;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels;
using Microsoft.EntityFrameworkCore;
using static FoodStore.GCommon.ValidationConstants;

namespace FoodStore.Services.Core
{
    public class CartService : ICartService
    {
        private readonly FoodStoreDbContext dbContext;


        public CartService(FoodStoreDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Order?> GetActiveCartAsync(string userId)
        {
            return await this.dbContext
            .Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.UserId == userId && o.OrderStatus == OrderStatus.Pending);

        }

        public async Task<Order> CreateCartAsync(string userId)
        {
            var cart = new Order
            {
                UserId = userId,
                OrderStatus = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Pending,
                OrderDate = DateTime.UtcNow,
            };

            dbContext.Orders.Add(cart);
            await dbContext.SaveChangesAsync();

            return cart;
        }

        public async Task AddToCartAsync(string userId, int productId, int quantity)
        {
            var cart = await GetActiveCartAsync(userId) ?? await CreateCartAsync(userId);

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

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
            var cart = await GetActiveCartAsync(userId);

            if (cart == null)
            {
                return new List<CartItemViewModel>();
            }

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
            var cart = await GetActiveCartAsync(userId);

            if (cart == null) return;

            dbContext.OrderItems.RemoveRange(cart.Items);

            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> CheckoutAsync(string userId)
        {
            var cart = await GetActiveCartAsync(userId);
            if (cart == null || !cart.Items.Any())
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
                    OrderDate = o.OrderDate.ToString(CreatedOnFormat),
                    TotalAmount = o.Items.Sum(i => i.Product.Price * i.Quantity),
                    OrderStatus = o.OrderStatus.ToString(),
                    PaymentStatus = o.PaymentStatus.ToString()

                })
                .ToListAsync();

            return orders;
        }

        public async Task<bool> CancelOrderAsync(string userId, int orderId)
        {
            bool result = false;

            var order = await dbContext.Orders
               .FirstOrDefaultAsync(o => o.UserId == userId && o.OrderStatus == OrderStatus.Pending
               && o.Id == orderId);


            if (order != null)
            {
                order.OrderStatus = OrderStatus.Cancelled;
                await dbContext.SaveChangesAsync();

                result = true;
            }

            return result;
        }

        public async Task<Order?> GetByIdAsync(int orderId)
        {
            return await dbContext.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<CancelOrderViewModel?> GetCancelOrderViewModelAsync(string userId, int orderId)
        {
            var order = await dbContext.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null || order.OrderStatus != OrderStatus.Pending)
            {
                return null;
            }

            return new CancelOrderViewModel
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate.ToString(CreatedOnFormat),
                OrderStatus = order.OrderStatus.ToString(),
                PaymentStatus = order.PaymentStatus.ToString(),
                TotalAmount = order.Items.Sum(i => i.Product.Price * i.Quantity)
            };
        }

    } 
}
