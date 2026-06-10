namespace FastFood.Models.ViewModels
{
    public class DashboardViewModel
    {
        public double RevenueToday { get; set; }
        public double RevenueThisMonth { get; set; }
        public double RevenueTotal { get; set; }
        public int TotalOrders { get; set; }
        public int OrdersToday { get; set; }

        public List<string> OrdersPerDayLabels { get; set; } = new();
        public List<int> OrdersPerDayCounts { get; set; } = new();

        public List<string> TopItemNames { get; set; } = new();
        public List<int> TopItemCounts { get; set; } = new();
    }
}
