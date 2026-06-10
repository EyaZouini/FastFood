using FastFood.Data;
using FastFood.Models;
using FastFood.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FastFood.Tests.Services
{
    public class DashboardServiceTests
    {
        private ApplicationDbContext CreateDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private static async Task SeedAsync(ApplicationDbContext db)
        {
            var user = new ApplicationUser
            {
                Id = "user1",
                UserName = "test@test.com",
                Name = "Test",
                City = "Tunis",
                Address = "1 rue Test",
                PostalCode = "1000"
            };
            db.Users.Add(user);

            var headers = new[]
            {
                new OrderHeader { ApplicationUserId = "user1", Name = "Alice", Phone = "1234",
                    OrderDate = DateTime.Today, OrderTotal = 20.0 },
                new OrderHeader { ApplicationUserId = "user1", Name = "Alice", Phone = "1234",
                    OrderDate = DateTime.Today, OrderTotal = 15.0 },
                new OrderHeader { ApplicationUserId = "user1", Name = "Bob", Phone = "5678",
                    OrderDate = DateTime.Today.AddDays(-1), OrderTotal = 10.0 }
            };
            db.OrderHeaders.AddRange(headers);
            await db.SaveChangesAsync();

            var category = new Category { Id = 1, Title = "Food" };
            var sub = new SubCategory { Id = 1, Title = "Burgers", CategoryId = 1 };
            var item = new Item { Id = 1, Title = "Burger", Description = "desc", Price = 9.99, SubCategoryId = 1, ImageUrl = "" };
            db.Categories.Add(category);
            db.SubCategories.Add(sub);
            db.Items.Add(item);
            await db.SaveChangesAsync();

            // Reload headers to get their IDs
            var savedHeaders = await db.OrderHeaders.ToListAsync();
            db.OrderDetails.AddRange(
                new OrderDetails { OrderHeaderId = savedHeaders[0].Id, ItemId = 1, Count = 3, Name = "Burger", Description = "desc", Price = 9.99 },
                new OrderDetails { OrderHeaderId = savedHeaders[1].Id, ItemId = 1, Count = 2, Name = "Burger", Description = "desc", Price = 9.99 },
                new OrderDetails { OrderHeaderId = savedHeaders[2].Id, ItemId = 1, Count = 1, Name = "Burger", Description = "desc", Price = 9.99 }
            );
            await db.SaveChangesAsync();
        }

        [Fact]
        public async Task GetDashboardAsync_RevenueTodayReflectsOnlyTodayOrders()
        {
            var db = CreateDb();
            await SeedAsync(db);
            var service = new DashboardService(db);

            var vm = await service.GetDashboardAsync();

            Assert.Equal(35.0, vm.RevenueToday);
        }

        [Fact]
        public async Task GetDashboardAsync_TotalOrdersCountsAllOrders()
        {
            var db = CreateDb();
            await SeedAsync(db);
            var service = new DashboardService(db);

            var vm = await service.GetDashboardAsync();

            Assert.Equal(3, vm.TotalOrders);
        }

        [Fact]
        public async Task GetDashboardAsync_OrdersTodayCountsOnlyToday()
        {
            var db = CreateDb();
            await SeedAsync(db);
            var service = new DashboardService(db);

            var vm = await service.GetDashboardAsync();

            Assert.Equal(2, vm.OrdersToday);
        }

        [Fact]
        public async Task GetDashboardAsync_OrdersPerDayHasSeven()
        {
            var db = CreateDb();
            await SeedAsync(db);
            var service = new DashboardService(db);

            var vm = await service.GetDashboardAsync();

            Assert.Equal(7, vm.OrdersPerDayLabels.Count);
            Assert.Equal(7, vm.OrdersPerDayCounts.Count);
        }

        [Fact]
        public async Task GetDashboardAsync_TopItemsReturnsMostOrdered()
        {
            var db = CreateDb();
            await SeedAsync(db);
            var service = new DashboardService(db);

            var vm = await service.GetDashboardAsync();

            Assert.Single(vm.TopItemNames);
            Assert.Equal("Burger", vm.TopItemNames[0]);
            Assert.Equal(6, vm.TopItemCounts[0]); // 3 + 2 + 1
        }
    }
}
