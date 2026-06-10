namespace FastFood.ViewModels
{
    public class DashboardViewModel
    {
        public double RevenueToday { get; set; }
        public double RevenueThisMonth { get; set; }
        public double RevenueTotal { get; set; }
        public int TotalOrders { get; set; }
        public int OrdersToday { get; set; }

        // Last 7 days — label + count per day
        public List<string> OrdersPerDayLabels { get; set; } = new();
        public List<int> OrdersPerDayCounts { get; set; } = new();

        // Top 5 most-ordered items — name + total quantity sold
        public List<string> TopItemNames { get; set; } = new();
        public List<int> TopItemCounts { get; set; } = new();
    }
}
