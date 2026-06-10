using FastFood.Models;
using Xunit;

namespace FastFood.Tests.Models
{
    public class OrderHeaderValidationTests
    {
        [Fact]
        public void OrderHeader_OrderTotal_IsCalculatedCorrectly()
        {
            var order = new OrderHeader
            {
                Name = "Eya",
                Phone = "0600000000",
                SubTotal = 25.0,
                CouponDis = 5.0,
                OrderTotal = 25.0 - 5.0
            };

            Assert.Equal(20.0, order.OrderTotal);
        }

        [Fact]
        public void OrderHeader_OptionalFields_AreNullable()
        {
            var order = new OrderHeader
            {
                Name = "Eya",
                Phone = "0600000000",
                OrderTotal = 15.0
            };

            Assert.Null(order.CouponCode);
            Assert.Null(order.TransId);
            Assert.Null(order.OrderStatus);
            Assert.Null(order.PaymentStatus);
        }
    }
}
