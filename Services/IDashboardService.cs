using FastFood.Models.ViewModels;

namespace FastFood.Services
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardAsync();
    }
}
