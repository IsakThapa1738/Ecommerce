using YourNamespace.Models;

namespace Ecommerce.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        // Navigation properties
        required
        public Cart? Cart { get; set; }
        required
        public Product Product { get; set; }
        public decimal UnitPrice { get; internal set; }
    }

}
