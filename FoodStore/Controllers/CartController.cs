using FoodStore.Services.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace FoodStore.Controllers
{
    public class CartController : BaseController
    {
        private readonly ICartService cartService;

        public CartController(ICartService cartService)
        {
            this.cartService = cartService; 
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                {
                    return View("NotFound"); // Custom 404
                }

                var cartItems = await cartService.GetCartItemsAsync(userId);
                return View(cartItems);
            }
            catch (Exception)
            {
                return View("ServerError"); // Custom 500
            }

        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId, int quantity)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) 
                {
                    return RedirectToAction("Login", "Account");
                }
                   
                await cartService.AddToCartAsync(userId, productId, quantity);

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
                    return RedirectToAction("Login", "Account");
                }

                await cartService.RemoveFromCartAsync(userId, productId);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View("ServerError");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                await cartService.ClearCartAsync(userId);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View("ServerError");
            }
        }
    }
}
