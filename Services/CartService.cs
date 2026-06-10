using FastFood.Data;
using FastFood.Models;
using FastFood.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CartOrderViewModel> GetSummaryAsync(string userId)
        {
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == userId);
            var carts = await _context.Carts.Include(x => x.Item)
                .Where(x => x.ApplicationUserId == userId).ToListAsync();

            return new CartOrderViewModel
            {
                ListofCart = carts,
                OrderHeader = new OrderHeader
                {
                    ApplicationUser = user,
                    Name = user?.Name ?? string.Empty,
                    Phone = user?.PhoneNumber ?? string.Empty,
                    TimeOfPick = DateTime.Now.AddHours(1),
                    OrderTotal = carts.Sum(c => c.Item.Price * c.Count)
                }
            };
        }

        public async Task<List<Cart>> GetUserCartsAsync(string userId) =>
            await _context.Carts
                .Where(c => c.ApplicationUserId == userId)
                .Include(c => c.ApplicationUser)
                .Include(c => c.Item)
                .ToListAsync();

        public async Task AddOrUpdateAsync(Cart cart)
        {
            var existing = await _context.Carts.FirstOrDefaultAsync(
                x => x.ApplicationUserId == cart.ApplicationUserId && x.ItemId == cart.ItemId);

            if (existing == null)
            {
                cart.Id = 0;
                _context.Carts.Add(cart);
            }
            else
            {
                existing.Count += cart.Count;
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(int cartId, int newQuantity)
        {
            var cart = await _context.Carts.FindAsync(cartId);
            if (cart != null && newQuantity >= 1)
            {
                cart.Count = newQuantity;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveAsync(int cartId)
        {
            var cart = await _context.Carts.FindAsync(cartId);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearUserCartAsync(string userId)
        {
            var carts = _context.Carts.Where(c => c.ApplicationUserId == userId);
            _context.Carts.RemoveRange(carts);
            await _context.SaveChangesAsync();
        }
    }
}
