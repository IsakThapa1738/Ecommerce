namespace Ecommerce.Models
{
    public class OrderStatus
    {
        public int Id { get; set; }
        public string StatusName { get; set; } // e.g., "Pending", "Shipped", "Delivered", "Canceled"
        public string Description { get; set; } // Optional detailed description of the status

        public int StatusId { get; set; }

        // Navigation property
        public ICollection<Order> Orders { get; set; }
    }

}
