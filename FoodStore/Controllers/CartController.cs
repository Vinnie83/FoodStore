using FoodStore.Data.Models;
using FoodStore.Data.Models.Enums;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                    return Redirect("/Identity/Account/Login");
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

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var cartItems = await cartService.GetCartItemsAsync(userId);

                return View(cartItems);
            }
            catch (Exception)
            {
                return View("ServerError");
            }

        }

        [HttpPost]
        public async Task<IActionResult> CheckoutConfirm()
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var success = await cartService.CheckoutAsync(userId);

                if (!success)
                {
                    TempData["Error"] = "Your cart is empty!";
                    return RedirectToAction("Index");
                }

                await cartService.ClearCartAsync(userId);

                return RedirectToAction("ThankYou");
            }
            catch (Exception)
            {
                return View("ServerError");
            }
        }

        [HttpGet]
        public IActionResult ThankYou()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return RedirectToAction("Login", "User");

                var orders = await cartService.GetOrderHistoryAsync(userId);
                return View(orders);
            }
            catch
            {
                return View("ServerError");
            }
        }
    }
}
