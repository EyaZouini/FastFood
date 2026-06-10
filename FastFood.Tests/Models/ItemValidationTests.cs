using System.ComponentModel.DataAnnotations;
using FastFood.Models;
using Xunit;

namespace FastFood.Tests.Models
{
    public class ItemValidationTests
    {
        [Fact]
        public void Item_IsValid_WhenAllRequiredFieldsSet()
        {
            var item = new Item
            {
                Title = "Classic Burger",
                Description = "Beef patty with cheese",
                Price = 8.99,
                SubCategoryId = 1,
                ImageUrl = "/images/burger.jpg"
            };

            var errors = Validate(item);

            Assert.Empty(errors);
        }

        [Fact]
        public void Item_Price_CanBeZero()
        {
            var item = new Item
            {
                Title = "Free Sample",
                Description = "Complimentary item",
                Price = 0,
                SubCategoryId = 1,
                ImageUrl = "/images/sample.jpg"
            };

            var errors = Validate(item);

            Assert.Empty(errors);
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
