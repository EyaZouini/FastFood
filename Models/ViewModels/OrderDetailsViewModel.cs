using FastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Models.ViewModels
{
    [Keyless]
    public class OrderDetailsViewModel
    {
        public OrderHeader OrderHeader { get; set; } = null!;
        public IEnumerable<OrderDetails> OrderDetails { get; set; } = null!;
    }
}
