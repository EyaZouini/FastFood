using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace FastFood.Models
{
    public class SubCategory
    {
        [Key]
        public int Id { get; set; }
        public required string Title { get; set; }
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; } = null!;
    }
}
