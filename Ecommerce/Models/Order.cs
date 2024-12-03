public class Order
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public DateTime CreateDate { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string MobileNumber { get; set; }
    public string PaymentMethod { get; set; }
    public string Address { get; set; } // Change 'object' to 'string'
    public bool IsPaid { get; set; }
    public int OrderStatusId { get; set; }

    // Navigation properties
    public OrderStatus OrderStatus { get; set; }
    public ICollection<OrderDetail> OrderDetails { get; set; }
}
