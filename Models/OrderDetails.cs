using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace FastFood.Models
{
    public class OrderDetails
    {
        [Key]
        public int Id { get; set; }
        public int OrderHeaderId { get; set; }
        [ValidateNever]
        public OrderHeader OrderHeader { get; set; } = null!;
        public int ItemId { get; set; }
        [ValidateNever]
        public Item Item { get; set; } = null!;
        public int Count { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public double Price { get; set; }
    }
}
