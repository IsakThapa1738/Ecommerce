using System.ComponentModel.DataAnnotations;
using YourNamespace.Models;

namespace Ecommerce.Models.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string Name { get; set; }

    }
}