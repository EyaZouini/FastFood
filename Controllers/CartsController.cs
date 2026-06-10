using FastFood.Models;
using FastFood.Services;
using FastFood.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Controllers
{
    [Authorize]
    public class CartsController : Controller
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> Summary() =>
            View(await _cartService.GetSummaryAsync(ClaimsHelper.GetUserId(User)));

        public async Task<IActionResult> Index() =>
            View(await _cartService.GetUserCartsAsync(ClaimsHelper.GetUserId(User)));

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var carts = await _cartService.GetUserCartsAsync(ClaimsHelper.GetUserId(User));
            var cart = carts.FirstOrDefault(c => c.Id == id);
            if (cart == null) return NotFound();
            return View(cart);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _cartService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartId, int newQuantity)
        {
            await _cartService.UpdateQuantityAsync(cartId, newQuantity);
            return RedirectToAction(nameof(Index));
        }
    }
}
