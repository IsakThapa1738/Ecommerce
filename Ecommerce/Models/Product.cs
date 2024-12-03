using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourNamespace.Models
{
    public class Product
    {
        // Primary Key
        public int Id { get; set; }

        // Product name
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        // Description of the product
        [StringLength(500)]
        public string Description { get; set; }

        // Price of the product
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        // Product's image 
        public string? Image { get; set; }

        // Foreign key to Category
        [Required]
        public int CategoryId { get; set; }

        // Navigation property to Category
        public virtual Category Category { get; set; }

        // Navigation property for Stocks


        // Navigation properties for related entities
        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        // Not mapped property for displaying the category name
        [NotMapped]
        public string? CategoryName { get; set; }
        public int Quantity { get; internal set; }


        public Stock Stock { get; set; }

    }
}

      
