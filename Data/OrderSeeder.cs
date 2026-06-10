using FastFood.Models;
using FastFood.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Data
{
    public static class OrderSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            await SeedManagerAsync(userManager);
            await SeedCouponsAsync(db);
            await SeedOrdersAsync(db);
        }

        private static async Task SeedManagerAsync(UserManager<ApplicationUser> userManager)
        {
            const string managerEmail = "manager@fastfood.com";
            if (await userManager.FindByEmailAsync(managerEmail) != null) return;

            var manager = new ApplicationUser
            {
                UserName = managerEmail,
                Email = managerEmail,
                EmailConfirmed = true,
                Name = "FastFood Manager",
                City = "Sousse",
                Address = "12 Avenue Habib Bourguiba",
                PostalCode = "4000"
            };
            var result = await userManager.CreateAsync(manager, "Manager@123");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(manager, "Manager");
        }

        private static async Task SeedCouponsAsync(ApplicationDbContext db)
        {
            if (await db.Coupons.AnyAsync()) return;

            db.Coupons.AddRange(
                new Coupon { Title = "WELCOME10", Type = "Percent", Discount = 10, MinimumAmount = 15, IsActive = true },
                new Coupon { Title = "SUMMER20",  Type = "Percent", Discount = 20, MinimumAmount = 25, IsActive = true },
                new Coupon { Title = "SAVE5",     Type = "Amount",  Discount = 5,  MinimumAmount = 20, IsActive = false }
            );
            await db.SaveChangesAsync();
        }

        private static async Task SeedOrdersAsync(ApplicationDbContext db)
        {
            if (await db.OrderHeaders.AnyAsync()) return;

            var customer = await db.ApplicationUsers
                .FirstOrDefaultAsync(u => u.Email != "admin@gmail.com" && u.Email != "manager@fastfood.com");
            if (customer == null) return;

            var items = await db.Items.ToDictionaryAsync(i => i.Title);
            if (items.Count < 5) return;

            var today = DateTime.Today;

            // 8 orders spread over last 7 days — gives the dashboard chart real data
            var orders = new (DateTime Date, (string Title, int Qty)[] Lines, string OrderSt, string PaySt)[]
            {
                (today.AddDays(-6).AddHours(12), new[] { ("Classic Beef Burger", 2), ("Fresh Orange Juice", 2) },           OrderStatus.StatusShipped,   PaymentStatus.StatusApproved),
                (today.AddDays(-5).AddHours(19), new[] { ("Salmon Sushi Roll", 1), ("Mango Smoothie", 1) },                 OrderStatus.StatusShipped,   PaymentStatus.StatusApproved),
                (today.AddDays(-4).AddHours(13), new[] { ("Couscous Merguez", 1), ("Brik au Thon", 2), ("Lablabi", 1) },   OrderStatus.StatusShipped,   PaymentStatus.StatusApproved),
                (today.AddDays(-3).AddHours(20), new[] { ("Pepperoni Pizza", 1), ("Tiramisu", 2) },                         OrderStatus.StatusApproved,  PaymentStatus.StatusApproved),
                (today.AddDays(-2).AddHours(12), new[] { ("Chicken Tacos", 2), ("Fresh Orange Juice", 2) },                 OrderStatus.StatusApproved,  PaymentStatus.StatusApproved),
                (today.AddDays(-1).AddHours(18), new[] { ("BBQ Bacon Burger", 2), ("Chocolate Lava Cake", 2) },             OrderStatus.StatusInProcess, PaymentStatus.StatusApproved),
                (today.AddHours(10),             new[] { ("Couscous Poulet", 1), ("Mixed Sushi Platter", 1) },              OrderStatus.StatusInProcess, PaymentStatus.StatusApproved),
                (today.AddHours(11),             new[] { ("Margherita Pizza", 1), ("Spaghetti Carbonara", 1), ("Tiramisu", 1) }, OrderStatus.StatusPending, PaymentStatus.StatusPending),
            };

            foreach (var (date, lines, orderSt, paySt) in orders)
            {
                var total = lines.Sum(l => items.TryGetValue(l.Title, out var it) ? it.Price * l.Qty : 0);

                var header = new OrderHeader
                {
                    ApplicationUserId = customer.Id,
                    ApplicationUser   = customer,
                    Name              = customer.Name,
                    Phone             = customer.PhoneNumber ?? "+216 55 338 226",
                    OrderDate         = date,
                    TimeOfPick        = date.AddHours(1),
                    OrderTotal        = Math.Round(total, 2),
                    OrderStatus       = orderSt,
                    PaymentStatus     = paySt,
                    TransId           = paySt == PaymentStatus.StatusApproved ? Guid.NewGuid().ToString("N")[..12] : string.Empty,
                    CouponCode        = string.Empty,
                    CouponDis         = 0
                };

                db.OrderHeaders.Add(header);
                await db.SaveChangesAsync();

                foreach (var (title, qty) in lines)
                {
                    if (!items.TryGetValue(title, out var item)) continue;
                    db.OrderDetails.Add(new OrderDetails
                    {
                        OrderHeaderId = header.Id,
                        ItemId        = item.Id,
                        Count         = qty,
                        Name          = item.Title,
                        Description   = item.Description,
                        Price         = item.Price
                    });
                }
                await db.SaveChangesAsync();
            }
        }
    }
}
