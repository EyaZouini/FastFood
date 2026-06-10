using Microsoft.AspNetCore.Identity;

namespace FastFood.Models
{
    public class ApplicationUser : IdentityUser
    {
        public required string Name { get; set; }
        public required string City { get; set; }
        public required string Address { get; set; }
        public required string PostalCode { get; set; }
    }
}
