using System.ComponentModel.DataAnnotations.Schema;
using YourNamespace.Models;

namespace Ecommerce.Models
{
    [Table("Stock")]
    public class Stock
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public Product? Product { get; set; }

        internal bool Any()
        {
            throw new NotImplementedException();
        }

        internal int Sum(Func<object, object> value)
        {
            throw new NotImplementedException();
        }
    }
}