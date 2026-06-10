using FastFood.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FastFood.Tests.Data
{
    public class CsvSeederTests
    {
        private ApplicationDbContext CreateDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private static async Task<string> WriteTempCsvAsync(string content)
        {
            var path = Path.GetTempFileName();
            await File.WriteAllTextAsync(path, content);
            return path;
        }

        [Fact]
        public async Task SeedFromCsvAsync_CreatesExpectedItems()
        {
            var csv = """
                Title;Description;Price;Category;SubCategory;ImageUrl
                Burger;Beef patty;8.99;Fast Food;Burgers;https://img.test/burger.jpg
                Pizza;Cheese pizza;12.99;World Cuisine;Pizza;https://img.test/pizza.jpg
                """;

            var db = CreateDb();
            var csvPath = await WriteTempCsvAsync(csv);

            await CsvSeeder.SeedFromCsvAsync(db, csvPath);

            Assert.Equal(2, await db.Items.CountAsync());
            Assert.Equal(2, await db.Categories.CountAsync());
            Assert.Equal(2, await db.SubCategories.CountAsync());
        }

        [Fact]
        public async Task SeedFromCsvAsync_SharedCategoryCreatedOnce()
        {
            var csv = """
                Title;Description;Price;Category;SubCategory;ImageUrl
                Burger;Beef patty;8.99;Fast Food;Burgers;https://img.test/burger.jpg
                Sandwich;Tuna sandwich;7.50;Fast Food;Sandwiches;https://img.test/sandwich.jpg
                """;

            var db = CreateDb();
            var csvPath = await WriteTempCsvAsync(csv);

            await CsvSeeder.SeedFromCsvAsync(db, csvPath);

            Assert.Equal(1, await db.Categories.CountAsync());
            Assert.Equal(2, await db.SubCategories.CountAsync());
        }

        [Fact]
        public async Task SeedFromCsvAsync_IsIdempotent()
        {
            var csv = """
                Title;Description;Price;Category;SubCategory;ImageUrl
                Burger;Beef patty;8.99;Fast Food;Burgers;https://img.test/burger.jpg
                """;

            var db = CreateDb();
            var csvPath = await WriteTempCsvAsync(csv);

            await CsvSeeder.SeedFromCsvAsync(db, csvPath);
            await CsvSeeder.SeedFromCsvAsync(db, csvPath); // second call should be no-op

            Assert.Equal(1, await db.Items.CountAsync());
        }

        [Fact]
        public async Task SeedFromCsvAsync_ParsesPriceCorrectly()
        {
            var csv = """
                Title;Description;Price;Category;SubCategory;ImageUrl
                Lablabi;Chickpea soup;5.99;Tunisian Cuisine;Soups;https://img.test/lablabi.jpg
                """;

            var db = CreateDb();
            var csvPath = await WriteTempCsvAsync(csv);

            await CsvSeeder.SeedFromCsvAsync(db, csvPath);

            var item = await db.Items.FirstAsync();
            Assert.Equal(5.99, item.Price);
            Assert.Equal("Lablabi", item.Title);
        }

        [Fact]
        public async Task SeedFromCsvAsync_SkipsMalformedLines()
        {
            var csv = """
                Title;Description;Price;Category;SubCategory;ImageUrl
                Burger;Beef patty;8.99;Fast Food;Burgers;https://img.test/burger.jpg
                BAD_LINE_MISSING_FIELDS
                Pizza;Cheese pizza;12.99;World Cuisine;Pizza;https://img.test/pizza.jpg
                """;

            var db = CreateDb();
            var csvPath = await WriteTempCsvAsync(csv);

            await CsvSeeder.SeedFromCsvAsync(db, csvPath);

            Assert.Equal(2, await db.Items.CountAsync());
        }
    }
}
