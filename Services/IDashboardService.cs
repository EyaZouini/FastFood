using FastFood.ViewModels;

namespace FastFood.Services
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardAsync();
    }
}
