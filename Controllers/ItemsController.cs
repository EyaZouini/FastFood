using FastFood.Models;
using FastFood.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FastFood.Data;

namespace FastFood.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ItemsController : Controller
    {
        private readonly IItemService _itemService;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ItemsController(IItemService itemService, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _itemService = itemService;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index() =>
            View(await _itemService.GetAllAsync());

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var item = await _itemService.GetByIdAsync(id.Value);
            return item == null ? NotFound() : View(item);
        }

        public IActionResult Create()
        {
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Price,SubCategoryId")] Item item, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Id", "Title", item.SubCategoryId);
                return View(item);
            }
            await _itemService.CreateAsync(item, imageFile, _webHostEnvironment.WebRootPath);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _itemService.GetByIdAsync(id.Value);
            if (item == null) return NotFound();
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Id", "Title", item.SubCategoryId);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Price,SubCategoryId")] Item item, IFormFile imageFile)
        {
            if (id != item.Id) return NotFound();
            if (!ModelState.IsValid)
            {
                ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Id", "Title", item.SubCategoryId);
                return View(item);
            }
            try
            {
                await _itemService.UpdateAsync(item, imageFile, _webHostEnvironment.WebRootPath);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _itemService.ExistsAsync(item.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var item = await _itemService.GetByIdAsync(id.Value);
            return item == null ? NotFound() : View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _itemService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
