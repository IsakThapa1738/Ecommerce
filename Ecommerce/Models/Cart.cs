namespace Ecommerce.Models
{
    public class Cart
    {
        public int Id { get; set; } // Primary Key

        // UserId should match the type used in IdentityUser (typically a string)
        public string UserId { get; set; }

        // Navigation properties
        public ICollection<CartItem> CartItems { get; set; }
    }
}
