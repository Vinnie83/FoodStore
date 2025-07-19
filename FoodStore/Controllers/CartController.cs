using FoodStore.Data.Models;
using FoodStore.Data.Models.Enums;
using FoodStore.Services.Core.Contracts;
using FoodStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using static FoodStore.GCommon.ValidationConstants;

namespace FoodStore.Controllers
{
    [Authorize]
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
                return RedirectToAction("ServerError", "Error");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                return RedirectToAction("ServerError", "Error");
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
                return RedirectToAction("ServerError", "Error");
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
                return RedirectToAction("ServerError", "Error");
            }
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
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
                return RedirectToAction("ServerError", "Error");
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

                return RedirectToAction("ThankYou");
            }
            catch (Exception)
            {
                return RedirectToAction("ServerError", "Error");
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
                return RedirectToAction("ServerError", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Cancel(int orderId)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                    return RedirectToAction("Login", "User");

                var model = await cartService.GetCancelOrderViewModelAsync(userId, orderId);

                if (model == null)
                    return RedirectToAction("NotFoundPage", "Error");

                return View(model);
            }
            catch (Exception)
            {

                return RedirectToAction("ServerError", "Error");
            }
           
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmCancel(CancelOrderViewModel model)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null) return RedirectToAction("Login", "User");

                var success = await cartService.CancelOrderAsync(userId, model.OrderId);

                if (!success)
                {
                    TempData["Error"] = "You can only cancel orders that are still pending and belong to you.";
                    return RedirectToAction("History");
                }

                TempData["Message"] = "Order cancelled successfully.";
                return RedirectToAction("History");

            }
            catch (Exception)
            {

                return RedirectToAction("ServerError", "Error");
            }
           
        }
    }
}
