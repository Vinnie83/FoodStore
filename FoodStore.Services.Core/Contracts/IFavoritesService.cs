using FoodStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Services.Core.Contracts
{
    public interface IFavoritesService
    {
        Task<bool> AddToFavoritesAsync(string userId, int productId);
        Task<bool> RemoveFromFavoritesAsync(string userId, int productId);
        Task<bool> IsFavoriteAsync(string userId, int productId);
        Task<IEnumerable<FavoriteProductViewModel>> GetUserFavoritesAsync(string userId);
    }
}
