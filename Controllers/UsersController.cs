using FastFood.Data;
using FastFood.Models;
using FastFood.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Controllers
{
    [Authorize(Roles = "Admin")]

    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userId = ClaimsHelper.GetUserId(User);
            var users = _context.ApplicationUsers.Where(x => x.Id != userId).ToList();
            return View(users);
        }

        // GET: Users/Edit/{id}
        public IActionResult Edit(string id)
        {
            if (id == null)
                return NotFound();

            var user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: Users/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser user, string newRole)
        {
            if (id != user.Id)
            {
                return BadRequest("User ID mismatch.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing user
                    var existingUser = await _context.ApplicationUsers.FindAsync(id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    // Update only the editable fields
                    existingUser.Name = user.Name;
                    existingUser.Email = user.Email;
                    existingUser.City = user.City;
                    existingUser.Address = user.Address;
                    existingUser.PostalCode = user.PostalCode;

                    // Role management
                    var userRoles = await _userManager.GetRolesAsync(existingUser);
                    if (!string.IsNullOrEmpty(newRole) && !userRoles.Contains(newRole))
                    {
                        // Remove all existing roles
                        await _userManager.RemoveFromRolesAsync(existingUser, userRoles);

                        // Add the new role
                        await _userManager.AddToRoleAsync(existingUser, newRole);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(user);
        }

        // GET: Users/Delete/{id}
        public IActionResult Delete(string id)
        {
            if (id == null)
                return NotFound();

            var user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: Users/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();

            _context.ApplicationUsers.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _context.ApplicationUsers.Any(e => e.Id == id);
        }
    }
}
