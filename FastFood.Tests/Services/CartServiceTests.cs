using FastFood.Data;
using FastFood.Models;
using FastFood.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FastFood.Tests.Services
{
    public class CartServiceTests
    {
        private ApplicationDbContext CreateDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task AddOrUpdateAsync_AddsNewCart_WhenNotExists()
        {
            using var db = CreateDb();
            var service = new CartService(db);
            var cart = new Cart { ItemId = 1, ApplicationUserId = "user1", Count = 2 };

            await service.AddOrUpdateAsync(cart);

            Assert.Equal(1, await db.Carts.CountAsync());
            Assert.Equal(2, (await db.Carts.FirstAsync()).Count);
        }

        [Fact]
        public async Task AddOrUpdateAsync_IncrementsCount_WhenCartExists()
        {
            using var db = CreateDb();
            db.Carts.Add(new Cart { Id = 1, ItemId = 1, ApplicationUserId = "user1", Count = 2 });
            await db.SaveChangesAsync();

            var service = new CartService(db);
            await service.AddOrUpdateAsync(new Cart { ItemId = 1, ApplicationUserId = "user1", Count = 3 });

            Assert.Equal(5, (await db.Carts.FirstAsync()).Count);
        }

        [Fact]
        public async Task RemoveAsync_DeletesCart()
        {
            using var db = CreateDb();
            db.Carts.Add(new Cart { Id = 1, ItemId = 1, ApplicationUserId = "user1", Count = 1 });
            await db.SaveChangesAsync();

            var service = new CartService(db);
            await service.RemoveAsync(1);

            Assert.Equal(0, await db.Carts.CountAsync());
        }

        [Fact]
        public async Task ClearUserCartAsync_RemovesAllUserCarts()
        {
            using var db = CreateDb();
            db.Carts.AddRange(
                new Cart { ItemId = 1, ApplicationUserId = "user1", Count = 1 },
                new Cart { ItemId = 2, ApplicationUserId = "user1", Count = 1 },
                new Cart { ItemId = 3, ApplicationUserId = "user2", Count = 1 }
            );
            await db.SaveChangesAsync();

            var service = new CartService(db);
            await service.ClearUserCartAsync("user1");

            Assert.Equal(1, await db.Carts.CountAsync());
            Assert.Equal("user2", (await db.Carts.FirstAsync()).ApplicationUserId);
        }

        [Fact]
        public async Task UpdateQuantityAsync_UpdatesCount()
        {
            using var db = CreateDb();
            db.Carts.Add(new Cart { Id = 1, ItemId = 1, ApplicationUserId = "user1", Count = 1 });
            await db.SaveChangesAsync();

            var service = new CartService(db);
            await service.UpdateQuantityAsync(1, 5);

            Assert.Equal(5, (await db.Carts.FirstAsync()).Count);
        }
    }
}
