using FastFood.Models;
using FastFood.ViewModels;

namespace FastFood.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderHeader>> GetOrdersAsync(string? userId, bool isAdminOrManager, string? statusFilter);
        Task<OrderDetailsViewModel> GetOrderDetailsAsync(int id);
        Task CreateOrderAsync(CartOrderViewModel order, string userId);
    }
}
