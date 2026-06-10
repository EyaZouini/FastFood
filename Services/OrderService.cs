using FastFood.Data;
using FastFood.Models;
using FastFood.Utility;
using FastFood.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderHeader>> GetOrdersAsync(string? userId, bool isAdminOrManager, string? statusFilter)
        {
            IEnumerable<OrderHeader> orders;

            if (isAdminOrManager)
                orders = await _context.OrderHeaders.Include(x => x.ApplicationUser).ToListAsync();
            else
                orders = await _context.OrderHeaders
                    .Where(x => x.ApplicationUserId == userId).ToListAsync();

            return statusFilter switch
            {
                "pending"      => orders.Where(x => x.PaymentStatus == PaymentStatus.StatusPending),
                "approved"     => orders.Where(x => x.PaymentStatus == PaymentStatus.StatusApproved),
                "underprocess" => orders.Where(x => x.OrderStatus == OrderStatus.StatusInProcess),
                "shipped"      => orders.Where(x => x.OrderStatus == OrderStatus.StatusShipped),
                _              => orders
            };
        }

        public async Task<OrderDetailsViewModel> GetOrderDetailsAsync(int id) =>
            new OrderDetailsViewModel
            {
                OrderHeader = await _context.OrderHeaders
                    .Include(x => x.ApplicationUser)
                    .FirstOrDefaultAsync(x => x.Id == id) ?? new OrderHeader { Name = string.Empty, Phone = string.Empty },
                OrderDetails = await _context.OrderDetails
                    .Include(x => x.Item)
                    .Where(y => y.OrderHeaderId == id).ToListAsync()
            };

        public async Task CreateOrderAsync(CartOrderViewModel order, string userId)
        {
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == userId);

            var header = new OrderHeader
            {
                ApplicationUser = user,
                Name = order.OrderHeader.Name,
                Phone = order.OrderHeader.Phone,
                TimeOfPick = DateTime.Now.AddHours(1),
                OrderTotal = order.OrderHeader.OrderTotal,
                OrderDate = DateTime.Now,
                OrderStatus = OrderStatus.StatusInProcess,
                PaymentStatus = PaymentStatus.StatusApproved,
                TransId = string.Empty,
                CouponCode = string.Empty,
                CouponDis = 0.0
            };

            _context.OrderHeaders.Add(header);
            await _context.SaveChangesAsync();

            var orderDetails = order.ListofCart.Select(detail => new OrderDetails
            {
                OrderHeaderId = header.Id,
                ItemId = detail.Item.Id,
                Count = detail.Count,
                Name = detail.Item.Title,
                Description = detail.Item.Description,
                Price = detail.Item.Price
            }).ToList();

            _context.OrderDetails.AddRange(orderDetails);
            await _context.SaveChangesAsync();

            var userCart = _context.Carts.Where(c => c.ApplicationUserId == userId);
            _context.Carts.RemoveRange(userCart);
            await _context.SaveChangesAsync();
        }
    }
}
