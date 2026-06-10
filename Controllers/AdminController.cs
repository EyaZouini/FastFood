using FastFood.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public AdminController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var vm = await _dashboardService.GetDashboardAsync();
            return View(vm);
        }
    }
}
