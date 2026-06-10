using System.ComponentModel.DataAnnotations;

namespace FastFood.Models.ViewModels
{
    public class CouponViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        public string Type { get; set; } = null!;

        [Range(0, double.MaxValue, ErrorMessage = "Discount must be a positive value")]
        public double Discount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Minimum amount must be a positive value")]
        public double MinimumAmount { get; set; }

        public IFormFile? CouponPicture { get; set; }

        public bool IsActive { get; set; }
    }
}
