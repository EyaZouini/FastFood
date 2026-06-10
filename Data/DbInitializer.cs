using FastFood.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;

namespace FastFood.Data
{
    public static class DbInitializer
    {
        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Seed roles
            var roles = new[] { "Admin", "Manager", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed default admin user
            var adminEmail = "admin@gmail.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Name = "Admin Name",
                    Address = "Default Admin Address",
                    City = "Default Admin City",
                    PostalCode = "Default Admin Postal Code"
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Seed food items from CSV
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var csvPath = Path.Combine(env.ContentRootPath, "Data", "seed", "food-items.csv");
            if (File.Exists(csvPath))
                await CsvSeeder.SeedFromCsvAsync(db, csvPath);
        }
    }
}
