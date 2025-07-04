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
            try
            {
                var userId = GetUserId();
                if (userId == null)
                {
                    return Redirect("/Identity/Account/Login");
                }

                var favorites = await favoritesService.GetUserFavoritesAsync(userId);
                return View(favorites);
            }
            catch (Exception)
            {
                return View("ServerError");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                {
                    return Redirect("/Identity/Account/Login");
                }

                await favoritesService.AddToFavoritesAsync(userId, productId);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View("ServerError");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int productId)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                {
                    return Redirect("/Identity/Account/Login");
                }

                await favoritesService.RemoveFromFavoritesAsync(userId, productId);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View("ServerError");
            }
        }
    }
}
