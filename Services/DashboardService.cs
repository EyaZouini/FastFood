using FastFood.Data;
using FastFood.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardAsync()
        {
            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var headers = await _context.OrderHeaders.ToListAsync();
            var details = await _context.OrderDetails
                .Include(d => d.Item)
                .ToListAsync();

            // Revenue
            var revenueToday = headers
                .Where(o => o.OrderDate.Date == today)
                .Sum(o => o.OrderTotal);

            var revenueThisMonth = headers
                .Where(o => o.OrderDate.Date >= monthStart)
                .Sum(o => o.OrderTotal);

            var revenueTotal = headers.Sum(o => o.OrderTotal);

            // Orders per day — last 7 days
            var labels = new List<string>();
            var counts = new List<int>();
            for (int i = 6; i >= 0; i--)
            {
                var day = today.AddDays(-i);
                labels.Add(day.ToString("MMM dd"));
                counts.Add(headers.Count(o => o.OrderDate.Date == day));
            }

            // Top 5 items by total quantity sold
            var topItems = details
                .GroupBy(d => d.Name)
                .Select(g => new { Name = g.Key, Total = g.Sum(d => d.Count) })
                .OrderByDescending(x => x.Total)
                .Take(5)
                .ToList();

            return new DashboardViewModel
            {
                RevenueToday = revenueToday,
                RevenueThisMonth = revenueThisMonth,
                RevenueTotal = revenueTotal,
                TotalOrders = headers.Count,
                OrdersToday = headers.Count(o => o.OrderDate.Date == today),
                OrdersPerDayLabels = labels,
                OrdersPerDayCounts = counts,
                TopItemNames = topItems.Select(x => x.Name).ToList(),
                TopItemCounts = topItems.Select(x => x.Total).ToList()
            };
        }
    }
}
