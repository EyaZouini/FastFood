using FastFood.Models;

namespace FastFood.Services
{
    public interface IItemService
    {
        Task<List<Item>> GetAllAsync(string? searchQuery = null);
        Task<Item?> GetByIdAsync(int id);
        Task CreateAsync(Item item, IFormFile? imageFile, string webRootPath);
        Task UpdateAsync(Item item, IFormFile? imageFile, string webRootPath);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
