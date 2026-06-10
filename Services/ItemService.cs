using FastFood.Data;
using FastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Services
{
    public class ItemService : IItemService
    {
        private readonly ApplicationDbContext _context;

        public ItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Item>> GetAllAsync(string? searchQuery = null)
        {
            var query = _context.Items.Include(i => i.SubCategory).AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery))
                query = query.Where(i => i.Title.ToLower().Contains(searchQuery.ToLower()));
            return await query.ToListAsync();
        }

        public async Task<Item?> GetByIdAsync(int id) =>
            await _context.Items.Include(i => i.SubCategory).FirstOrDefaultAsync(i => i.Id == id);

        public async Task CreateAsync(Item item, IFormFile? imageFile, string webRootPath)
        {
            item.ImageUrl = imageFile != null
                ? await SaveImageAsync(imageFile, webRootPath)
                : string.Empty;
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Item item, IFormFile? imageFile, string webRootPath)
        {
            if (imageFile != null)
                item.ImageUrl = await SaveImageAsync(imageFile, webRootPath);
            else
            {
                var existing = await _context.Items.AsNoTracking().FirstOrDefaultAsync(i => i.Id == item.Id);
                item.ImageUrl = existing?.ImageUrl ?? string.Empty;
            }
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id) =>
            await _context.Items.AnyAsync(i => i.Id == id);

        private static async Task<string> SaveImageAsync(IFormFile imageFile, string webRootPath)
        {
            var uploadDir = "images/";
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(webRootPath, uploadDir, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);
            return "/" + uploadDir + fileName;
        }
    }
}
