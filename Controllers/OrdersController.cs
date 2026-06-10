using FastFood.Services;
using FastFood.Utility;
using FastFood.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index(string status)
        {
            var userId = ClaimsHelper.GetUserId(User);
            var isAdminOrManager = User.IsInRole("Admin") || User.IsInRole("Manager");
            var orders = await _orderService.GetOrdersAsync(userId, isAdminOrManager, status);
            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(int id) =>
            View(await _orderService.GetOrderDetailsAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(CartOrderViewModel order)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Summary", "Carts");

            await _orderService.CreateOrderAsync(order, ClaimsHelper.GetUserId(User));
            return RedirectToAction("Index", "Home");
        }
    }
}
