using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string? Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; } // Use Range instead of MaxLength

        public string? Image { get; set; }

        [StringLength(500)]
        public string Description { get; set; }


        [Required]
        public int CategoryId { get; set; }

        public IFormFile? ImageFile { get; set; }

        public IEnumerable<SelectListItem>? CategoryList { get; set; }
    }
}
