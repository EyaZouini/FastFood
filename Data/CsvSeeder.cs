using FastFood.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FastFood.Data
{
    public static class CsvSeeder
    {
        // CSV format (semicolon-separated):
        // Title;Description;Price;Category;SubCategory;ImageUrl
        public static async Task SeedFromCsvAsync(ApplicationDbContext db, string csvPath)
        {
            if (await db.Items.AnyAsync()) return;

            var lines = await File.ReadAllLinesAsync(csvPath);

            var categoryCache = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var subCategoryCache = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(';');
                if (parts.Length < 6) continue;

                var title       = parts[0].Trim();
                var description = parts[1].Trim();
                var price       = double.Parse(parts[2].Trim(), CultureInfo.InvariantCulture);
                var categoryName    = parts[3].Trim();
                var subCategoryName = parts[4].Trim();
                var imageUrl    = parts[5].Trim();

                var categoryId = await GetOrCreateCategoryAsync(db, categoryCache, categoryName);
                var subCategoryId = await GetOrCreateSubCategoryAsync(db, subCategoryCache, subCategoryName, categoryId);

                db.Items.Add(new Item
                {
                    Title = title,
                    Description = description,
                    Price = price,
                    SubCategoryId = subCategoryId,
                    ImageUrl = imageUrl
                });
            }

            await db.SaveChangesAsync();
        }

        private static async Task<int> GetOrCreateCategoryAsync(
            ApplicationDbContext db,
            Dictionary<string, int> cache,
            string name)
        {
            if (cache.TryGetValue(name, out var id)) return id;

            var entity = await db.Categories.FirstOrDefaultAsync(c => c.Title == name);
            if (entity == null)
            {
                entity = new Category { Title = name };
                db.Categories.Add(entity);
                await db.SaveChangesAsync();
            }

            cache[name] = entity.Id;
            return entity.Id;
        }

        private static async Task<int> GetOrCreateSubCategoryAsync(
            ApplicationDbContext db,
            Dictionary<string, int> cache,
            string name,
            int categoryId)
        {
            var key = $"{categoryId}|{name}";
            if (cache.TryGetValue(key, out var id)) return id;

            var entity = await db.SubCategories
                .FirstOrDefaultAsync(s => s.Title == name && s.CategoryId == categoryId);

            if (entity == null)
            {
                entity = new SubCategory { Title = name, CategoryId = categoryId };
                db.SubCategories.Add(entity);
                await db.SaveChangesAsync();
            }

            cache[key] = entity.Id;
            return entity.Id;
        }
    }
}
