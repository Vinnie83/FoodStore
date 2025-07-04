using FoodStore.Data.Models;
using FoodStore.Services.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodStore.Controllers
{
    [Authorize]
    public class FavoritesController : BaseController
    {
        private readonly IFavoritesService favoritesService;

        public FavoritesController(IFavoritesService favoritesService)
        {
            this.favoritesService = favoritesService;

        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();

            var favorites = await favoritesService.GetUserFavoritesAsync(userId);

            return View(favorites);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId)
        {
            var userId = GetUserId();

            await favoritesService.AddToFavoritesAsync(userId, productId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = GetUserId();

            await favoritesService.RemoveFromFavoritesAsync(userId, productId);

            return RedirectToAction("Index");
        }
    }
}
