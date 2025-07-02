using FoodStore.Data;
using FoodStore.Data.Models;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodStore.Services.Core
{
    public class FavoritesService : IFavoritesService
    {
        private readonly FoodStoreDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public FavoritesService(FoodStoreDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;  
            this.userManager = userManager;
        }

        public async Task<bool> AddToFavoritesAsync(string userId, int productId)
        {
            bool operResult = false;

            ApplicationUser? user = await this.userManager.FindByIdAsync(userId);

            Product? product = await this.dbContext
                .Products
                .FindAsync(productId);

            if (user != null && product != null)
            {

                UserProduct? userFav = await this.dbContext
                    .UsersProducts
                    .SingleOrDefaultAsync(up => up.UserId.ToLower() == userId.ToLower()
                                             && up.ProductId == productId);

                if (userFav == null)
                {
                    userFav = new UserProduct()
                    {
                        UserId = userId,
                        ProductId = productId
                    };

                    await this.dbContext.UsersProducts.AddAsync(userFav);
                    await this.dbContext.SaveChangesAsync();

                    operResult = true;
                }
            }

            return operResult;
        }

        public async Task<bool> RemoveFromFavoritesAsync(string userId, int productId)
        {
            bool operResult = false;

            ApplicationUser? user = await this.userManager.FindByIdAsync(userId);

            Product? product = await this.dbContext.Products.FindAsync(productId);

            if (user != null && product != null)
            {
                UserProduct? userFavorite = await this.dbContext.UsersProducts
                    .SingleOrDefaultAsync(fp => fp.UserId.ToLower() == userId.ToLower()
                                             && fp.ProductId == productId);

                if (userFavorite != null)
                {
                    this.dbContext.UsersProducts.Remove(userFavorite);
                    await this.dbContext.SaveChangesAsync();
                    operResult = true;
                }
            }

            return operResult;
        }

        public async Task<IEnumerable<FavoriteProductViewModel>> GetUserFavoritesAsync(string userId)
        {
            var favoriteProducts = Array.Empty<FavoriteProductViewModel>();

            ApplicationUser? user = await this.userManager.FindByIdAsync(userId);

            if (user != null)
            {
                favoriteProducts = await this.dbContext
                    .UsersProducts
                    .Include(up => up.Product)
                    .AsNoTracking()
                    .Where(up => up.UserId.ToLower() == userId.ToLower())
                    .Select(up => new FavoriteProductViewModel
                    {
                        ProductId = up.ProductId,
                        ProductName = up.Product.Name,
                        ImageUrl = up.Product.ImageUrl,
                        Price = up.Product.Price
                    })
                    .ToArrayAsync();
            }

            return favoriteProducts;
        }

        public async Task<bool> IsFavoriteAsync(string userId, int productId)
        {
            ApplicationUser? user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            var productExists = await this.dbContext.Products
                .AnyAsync(p => p.Id == productId);

            if (!productExists)
            {
                return false;
            }

            bool isFavorite = await this.dbContext.UsersProducts
                .AnyAsync(x => x.UserId == userId && x.ProductId == productId);

            return isFavorite;
        }

        
    }
}
