using FastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFood.ViewModels
{
    [Keyless]
    public class CartOrderViewModel
    {
        public List<Cart> ListofCart { get; set; } = new();
        public OrderHeader OrderHeader { get; set; } = null!;
    }
}
