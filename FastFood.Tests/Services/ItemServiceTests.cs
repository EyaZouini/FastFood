using FastFood.Data;
using FastFood.Models;
using FastFood.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FastFood.Tests.Services
{
    public class ItemServiceTests
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
            db.Categories.Add(new Category { Id = 1, Title = "Food" });
            db.SubCategories.Add(new SubCategory { Id = 1, Title = "Burgers", CategoryId = 1 });
            db.Items.AddRange(
                new Item { Title = "Burger", Description = "Beef patty", Price = 8.99, SubCategoryId = 1, ImageUrl = "" },
                new Item { Title = "Pizza", Description = "Cheese pizza", Price = 11.99, SubCategoryId = 1, ImageUrl = "" }
            );
            await db.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllItems_WhenNoSearch()
        {
            using var db = CreateDb();
            await SeedAsync(db);

            var service = new ItemService(db);
            var result = await service.GetAllAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAllAsync_FiltersItems_WhenSearchQueryProvided()
        {
            using var db = CreateDb();
            await SeedAsync(db);

            var service = new ItemService(db);
            var result = await service.GetAllAsync("burg");

            Assert.Single(result);
            Assert.Equal("Burger", result[0].Title);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            using var db = CreateDb();
            var service = new ItemService(db);

            var result = await service.GetByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_RemovesItem()
        {
            using var db = CreateDb();
            await SeedAsync(db);

            var service = new ItemService(db);
            var item = await db.Items.FirstAsync();
            await service.DeleteAsync(item.Id);

            Assert.Equal(1, await db.Items.CountAsync());
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenItemExists()
        {
            using var db = CreateDb();
            await SeedAsync(db);
            var item = await db.Items.FirstAsync();

            var service = new ItemService(db);

            Assert.True(await service.ExistsAsync(item.Id));
            Assert.False(await service.ExistsAsync(99999));
        }
    }
}
