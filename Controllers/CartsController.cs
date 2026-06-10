using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FastFood.Data;
using FastFood.Models;
using FastFood.Utility;
using Microsoft.AspNetCore.Authorization;
using FastFood.ViewModels;

namespace FastFood.Controllers
{
    [Authorize]
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Summary()
        {
            var userId = ClaimsHelper.GetUserId(User);

            var details = new CartOrderViewModel()
            {
                ListofCart = _context.Carts.Include(x => x.Item)
                .Where(x => x.ApplicationUserId == userId).ToList(),
                OrderHeader = new OrderHeader()
            };
            details.OrderHeader.ApplicationUser = _context.ApplicationUsers
                .Where(x => x.Id == userId).FirstOrDefault();
            details.OrderHeader.Name = details.OrderHeader.ApplicationUser.Name;
            details.OrderHeader.Phone = details.OrderHeader.ApplicationUser.PhoneNumber;
            details.OrderHeader.TimeOfPick = DateTime.Now.AddHours(1);
            foreach(var cart in details.ListofCart)
            {
                details.OrderHeader.OrderTotal += (cart.Item.Price * cart.Count);
            }

            return View(details);
        }
        // GET: Carts
        public async Task<IActionResult> Index()
        {
            var userId = ClaimsHelper.GetUserId(User);

            var applicationDbContext = _context.Carts
                .Where(c => c.ApplicationUserId == userId)
                .Include(c => c.ApplicationUser)
                .Include(c => c.Item);

            return View(await applicationDbContext.ToListAsync());
        }


        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.ApplicationUser)
                .Include(c => c.Item)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Name");
            ViewData["ItemId"] = new SelectList(_context.Items, "Id", "Title");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ItemId,ApplicationUserId,Count")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Name", cart.ApplicationUserId);
            ViewData["ItemId"] = new SelectList(_context.Items, "Id", "Title", cart.ItemId);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Name", cart.ApplicationUserId);
            ViewData["ItemId"] = new SelectList(_context.Items, "Id", "Title", cart.ItemId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ItemId,ApplicationUserId,Count")] Cart cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Name", cart.ApplicationUserId);
            ViewData["ItemId"] = new SelectList(_context.Items, "Id", "Title", cart.ItemId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.ApplicationUser)
                .Include(c => c.Item)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartId, int newQuantity)
        {
            var cart = await _context.Carts.FindAsync(cartId);
            if (cart == null || newQuantity < 1)
            {
                return NotFound();
            }

            cart.Count = newQuantity;
            _context.Update(cart);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
