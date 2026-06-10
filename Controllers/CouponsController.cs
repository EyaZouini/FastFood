using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FastFood.Data;
using FastFood.Models;
using FastFood.Models.ViewModels;

namespace FastFood.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CouponsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CouponsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Coupons
        public async Task<IActionResult> Index()
        {
            return View(await _context.Coupons.ToListAsync());
        }

        // GET: Coupons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

        // GET: Coupons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Coupons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponViewModel model)
        {
            if (ModelState.IsValid)
            {
                var coupon = new Coupon
                {
                    Title = model.Title,
                    Type = model.Type,
                    Discount = model.Discount,
                    MinimumAmount = model.MinimumAmount,
                    IsActive = model.IsActive
                };

                var files = Request.Form.Files;
                if (files.Count > 0)
                {
                    using var fileStream = files[0].OpenReadStream();
                    using var memoryStream = new MemoryStream();
                    fileStream.CopyTo(memoryStream);
                    coupon.CouponPicture = memoryStream.ToArray();
                }
                _context.Coupons.Add(coupon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Coupons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }
            CouponViewModel Model = new CouponViewModel
            {
                Id = coupon.Id,
                Title = coupon.Title,
                Type = coupon.Type,
                Discount = coupon.Discount,
                MinimumAmount = coupon.MinimumAmount,
                IsActive = coupon.IsActive
            };
            return View(Model);
        }

        // POST: Coupons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CouponViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var coupon = new Coupon
                {   Id = id,
                    Title = model.Title,
                    Type = model.Type,
                    Discount = model.Discount,
                    MinimumAmount = model.MinimumAmount,
                    IsActive = model.IsActive
                };
                var files = Request.Form.Files;
                if (files.Count > 0)
                {
                    using var fileStream = files[0].OpenReadStream();
                    using var memoryStream = new MemoryStream();
                    fileStream.CopyTo(memoryStream);
                    coupon.CouponPicture = memoryStream.ToArray();
                }
                else
                {
                    var existing = await _context.Coupons.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                    coupon.CouponPicture = existing?.CouponPicture ?? Array.Empty<byte>();
                }

                try
                {
                    _context.Update(coupon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CouponExists(coupon.Id))
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
            return View(model);
        }

        // GET: Coupons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

        // POST: Coupons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon != null)
            {
                _context.Coupons.Remove(coupon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CouponExists(int id)
        {
            return _context.Coupons.Any(e => e.Id == id);
        }
    }
}
