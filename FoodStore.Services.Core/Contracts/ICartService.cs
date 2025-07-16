using FoodStore.Data.Models;
using FoodStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Services.Core.Contracts
{
    public interface ICartService
    {
        Task<Order?> GetActiveCartAsync(string userId);
        Task<Order> CreateCartAsync(string userId);
        Task AddToCartAsync(string userId, int productId, int quantity);
        Task<List<CartItemViewModel>> GetCartItemsAsync(string userId);
        Task RemoveFromCartAsync(string userId, int productId);
        Task ClearCartAsync(string userId);
        Task<bool> CheckoutAsync(string userId);
        Task<List<OrderHistoryViewModel>> GetOrderHistoryAsync(string userId);
        Task<CancelOrderViewModel?> GetCancelOrderViewModelAsync(string userId, int orderId);
        Task<bool> CancelOrderAsync(string userId, int orderId);
        Task<Order?> GetByIdAsync(int orderId);

        
    }
}
