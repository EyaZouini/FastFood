using System.ComponentModel.DataAnnotations;

namespace FastFood.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Type { get; set; }
        public double Discount { get; set; }
        public double MinimumAmount { get; set; }
        public byte[] CouponPicture { get; set; } = Array.Empty<byte>();
        public bool IsActive { get; set; }
    }
}
