using FastFood.Data;
using FastFood.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace FastFood.Tests.Data
{
    public class OrderSeederTests
    {
        private ApplicationDbContext CreateDb()
        {
            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(opts);
        }

        private static Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            mgr.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
            mgr.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
               .ReturnsAsync(IdentityResult.Success);
            mgr.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
               .ReturnsAsync(IdentityResult.Success);
            return mgr;
        }

        private static async Task SeedItemsAsync(ApplicationDbContext db)
        {
            db.Categories.Add(new Category { Id = 1, Title = "Fast Food" });
            db.SubCategories.Add(new SubCategory { Id = 1, Title = "Burgers", CategoryId = 1 });
            db.Items.AddRange(
                new Item { Title = "Classic Beef Burger", Description = "Juicy burger", Price = 8.99,  SubCategoryId = 1, ImageUrl = "" },
                new Item { Title = "Pepperoni Pizza",     Description = "Spicy pizza",  Price = 14.99, SubCategoryId = 1, ImageUrl = "" },
                new Item { Title = "Salmon Sushi Roll",   Description = "Fresh sushi",  Price = 16.99, SubCategoryId = 1, ImageUrl = "" },
                new Item { Title = "Tiramisu",            Description = "Dessert",       Price = 5.99,  SubCategoryId = 1, ImageUrl = "" },
                new Item { Title = "Fresh Orange Juice",  Description = "Fresh juice",   Price = 3.99,  SubCategoryId = 1, ImageUrl = "" }
            );
            await db.SaveChangesAsync();
        }

        private static async Task SeedCustomerAsync(ApplicationDbContext db)
        {
            db.Users.Add(new ApplicationUser
            {
                Id = "customer-1", UserName = "test@test.com", Email = "test@test.com",
                Name = "Test User", City = "Tunis", Address = "1 rue", PostalCode = "1000"
            });
            await db.SaveChangesAsync();
        }

        [Fact]
        public async Task SeedAsync_CreatesCoupons()
        {
            var db = CreateDb();
            var userMgr = MockUserManager();
            await SeedItemsAsync(db);
            await SeedCustomerAsync(db);

            await OrderSeeder.SeedAsync(db, userMgr.Object);

            Assert.Equal(3, await db.Coupons.CountAsync());
        }

        [Fact]
        public async Task SeedAsync_CreatesOrders()
        {
            var db = CreateDb();
            var userMgr = MockUserManager();
            await SeedItemsAsync(db);
            await SeedCustomerAsync(db);

            await OrderSeeder.SeedAsync(db, userMgr.Object);

            Assert.True(await db.OrderHeaders.AnyAsync());
            Assert.True(await db.OrderDetails.AnyAsync());
        }

        [Fact]
        public async Task SeedAsync_IsIdempotent()
        {
            var db = CreateDb();
            var userMgr = MockUserManager();
            await SeedItemsAsync(db);
            await SeedCustomerAsync(db);

            await OrderSeeder.SeedAsync(db, userMgr.Object);
            var countAfterFirst = await db.OrderHeaders.CountAsync();

            await OrderSeeder.SeedAsync(db, userMgr.Object);
            var countAfterSecond = await db.OrderHeaders.CountAsync();

            Assert.Equal(countAfterFirst, countAfterSecond);
        }

        [Fact]
        public async Task SeedAsync_SkipsOrdersWhenNoCustomerExists()
        {
            var db = CreateDb();
            var userMgr = MockUserManager();
            await SeedItemsAsync(db);
            // No customer user added

            await OrderSeeder.SeedAsync(db, userMgr.Object);

            Assert.Equal(0, await db.OrderHeaders.CountAsync());
        }

        [Fact]
        public async Task SeedAsync_FirstOrderTotalMatchesItemPrices()
        {
            // First order: Classic Beef Burger x2 (8.99) + Fresh Orange Juice x2 (3.99)
            // Expected total: 2*8.99 + 2*3.99 = 25.96
            var db = CreateDb();
            var userMgr = MockUserManager();
            await SeedItemsAsync(db);
            await SeedCustomerAsync(db);

            await OrderSeeder.SeedAsync(db, userMgr.Object);

            var firstOrder = await db.OrderHeaders.OrderBy(o => o.OrderDate).FirstAsync();
            Assert.Equal(25.96, firstOrder.OrderTotal, precision: 2);
        }
    }
}
