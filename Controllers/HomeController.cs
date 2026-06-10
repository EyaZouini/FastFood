using System.Diagnostics;
using FastFood.Models;
using FastFood.Services;
using FastFood.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IItemService _itemService;
        private readonly ICartService _cartService;

        public HomeController(ILogger<HomeController> logger, IItemService itemService, ICartService cartService)
        {
            _logger = logger;
            _itemService = itemService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index(string searchQuery) =>
            View(await _itemService.GetAllAsync(searchQuery));

        public async Task<IActionResult> Details(int Id)
        {
            var item = await _itemService.GetByIdAsync(Id);
            if (item == null) return NotFound();
            return View(new Cart { Item = item, ItemId = item.Id, Count = 1 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Details(Cart cart)
        {
            cart.ApplicationUserId = ClaimsHelper.GetUserId(User);

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    _logger.LogError(error.ErrorMessage);
                return RedirectToAction("Details", new { id = cart.ItemId });
            }

            await _cartService.AddOrUpdateAsync(cart);
            return RedirectToAction("Index");
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
