using FastFood.Data;
using FastFood.Models;
using FastFood.Utility;
using FastFood.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string status)
        {
            IEnumerable<OrderHeader> order;
            if (User.IsInRole("Admin") || User.IsInRole("Manager"))
            {
                order = _context.OrderHeaders.
                    Include(x => x.ApplicationUser).ToList();
            }
            else
            {
                var userId = ClaimsHelper.GetUserId(User);
                order = _context.OrderHeaders.Where(x => x.ApplicationUserId == userId);
            }
            switch (status)
            {
                case "pending":
                    order = order.Where(x => x.PaymentStatus == PaymentStatus.StatusPending);
                    break;
                case "approved":
                    order = order.Where(x => x.PaymentStatus == PaymentStatus.StatusApproved);
                    break;
                case "underprocess":
                    order = order.Where(x => x.OrderStatus == OrderStatus.StatusInProcess);
                    break;
                case "shipped":
                    order = order.Where(x => x.OrderStatus == OrderStatus.StatusShipped);
                    break;
            }
            return View(order);
        }
        public IActionResult OrderDetails(int id)
        {
            var OrderDetail = new OrderDetailsViewModel()
            {
                OrderHeader = _context.OrderHeaders.Include(x => x.ApplicationUser)
                .FirstOrDefault(x => x.Id == id),
                OrderDetails = _context.OrderDetails.Include(x => x.Item)
                .Where(y => y.OrderHeaderId == id).ToList()

            };
            return View(OrderDetail);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CartOrderViewModel order)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            if (ModelState.IsValid)
            {
                var userId = ClaimsHelper.GetUserId(User);
                var header = new OrderHeader
                {
                    ApplicationUser = _context.ApplicationUsers.Where(x => x.Id == userId).FirstOrDefault(),
                    Name = order.OrderHeader.Name,
                    Phone = order.OrderHeader.Phone,
                    TimeOfPick = DateTime.Now.AddHours(1),
                    OrderTotal = order.OrderHeader.OrderTotal,
                    OrderDate = DateTime.Now,
                    OrderStatus = OrderStatus.StatusInProcess,
                    PaymentStatus = PaymentStatus.StatusApproved,
                    TransId = "",
                    CouponCode = "",
                    CouponDis = 0.0
                };

                // Add the Order Header to the database
                _context.Add(header);
                await _context.SaveChangesAsync();

                // Prepare the Order Details
                var orderDetails = order.ListofCart.Select(detail => new OrderDetails
                {
                    OrderHeader = _context.OrderHeaders.Where(x=>x.Id == header.Id).FirstOrDefault(),
                    ItemId = detail.Item.Id,
                    Count = detail.Count,
                    Name = detail.Item.Title,
                    Description = detail.Item.Description,
                    Price = detail.Item.Price
                }).ToList();

                // Add all Order Details in one call
                _context.AddRange(orderDetails);


                // Save all changes to the database in one operation
                await _context.SaveChangesAsync();

                var userCart = _context.Carts.Where(c => c.ApplicationUserId == userId);
                _context.Carts.RemoveRange(userCart);
                await _context.SaveChangesAsync();

                // Redirect to the Summary action in CartsController
                return RedirectToAction("Index", "Home");
            }

            // If ModelState is invalid, reload the current view with errors
            return RedirectToAction("Summary", "Carts");
        }

    }
}
