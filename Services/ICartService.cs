using FastFood.Models;
using FastFood.Models.ViewModels;

namespace FastFood.Services
{
    public interface ICartService
    {
        Task<CartOrderViewModel> GetSummaryAsync(string userId);
        Task<List<Cart>> GetUserCartsAsync(string userId);
        Task AddOrUpdateAsync(Cart cart);
        Task UpdateQuantityAsync(int cartId, int newQuantity);
        Task RemoveAsync(int cartId);
        Task ClearUserCartAsync(string userId);
    }
}
