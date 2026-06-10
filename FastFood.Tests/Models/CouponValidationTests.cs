using System.ComponentModel.DataAnnotations;
using FastFood.Models;
using Xunit;

namespace FastFood.Tests.Models
{
    public class CouponValidationTests
    {
        [Fact]
        public void Coupon_IsValid_WhenAllFieldsSet()
        {
            var coupon = new Coupon
            {
                Title = "SUMMER10",
                Type = "Percent",
                Discount = 10,
                MinimumAmount = 20,
                CouponPicture = new byte[] { 1, 2, 3 },
                IsActive = true
            };

            var errors = Validate(coupon);

            Assert.Empty(errors);
        }

        [Fact]
        public void Coupon_IsActive_DefaultsToFalse()
        {
            var coupon = new Coupon
            {
                Title = "TEST",
                Type = "Flat",
                Discount = 5,
                MinimumAmount = 10,
                CouponPicture = Array.Empty<byte>()
            };

            Assert.False(coupon.IsActive);
        }

        private static IList<ValidationResult> Validate(object model)
        {
            var results = new List<ValidationResult>();
            var ctx = new ValidationContext(model);
            Validator.TryValidateObject(model, ctx, results, validateAllProperties: true);
            return results;
        }
    }
}
