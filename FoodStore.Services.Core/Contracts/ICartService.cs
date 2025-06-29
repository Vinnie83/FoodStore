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
        Task<Order> GetOrCreateCartAsync(string userId);
        Task AddToCartAsync(string userId, int productId, int quantity);
        Task<List<CartItemViewModel>> GetCartItemsAsync(string userId);
        Task RemoveFromCartAsync(string userId, int productId);
        Task ClearCartAsync(string userId);
    }
}
