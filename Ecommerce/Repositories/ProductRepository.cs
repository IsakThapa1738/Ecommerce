using Microsoft.EntityFrameworkCore;
using YourNamespace.Models;

namespace Ecommerce.Repositories
{
    public interface IProductRepository
    {
        Task AddProduct(Product product);
        Task DeleteProduct(Product product);
        Task<Product?> GetProductById(int id);
        Task<IEnumerable<Product>> GetProducts();
        Task UpdateProduct(Product product);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddProduct(Product product)
        {
            try
            {
                // Validate the product manually
                if (product == null)
                    throw new ArgumentNullException(nameof(product));

                if (string.IsNullOrWhiteSpace(product.Name))
                    throw new ArgumentException("Product name cannot be empty");

                if (product.Price <= 0)
                    throw new ArgumentException("Price must be greater than zero");

                if (product.CategoryId <= 0)
                    throw new ArgumentException("Invalid category");

                // Ensure the product is not tracking an existing entity
                _context.Entry(product).State = EntityState.Added;

                _context.Products.Add(product);
                int result = await _context.SaveChangesAsync();

                Console.WriteLine($"SaveChangesAsync result: {result}");

                if (result == 0)
                    throw new Exception("No changes were saved to the database");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw; // Re-throw to allow controller to handle
            }
        }

        public async Task UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<Product?> GetProductById(int id) => await _context.Products.FindAsync(id);

        public async Task<IEnumerable<Product>> GetProducts() => await _context.Products.Include(a => a.Category).ToListAsync();
    }
}