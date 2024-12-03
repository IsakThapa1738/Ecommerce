using Ecommerce.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using YourNamespace.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Data.Migrations;

namespace Ecommerce.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;

        public ProductController(IProductRepository productRepo, ICategoryRepository categoryRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productRepo.GetProducts();
            return View(products);
        }

        // AddProduct GET method
        public async Task<IActionResult> AddProduct()
        {
            var categorySelectList = (await _categoryRepo.GetCategories()).Select(category => new SelectListItem
            {
                Text = category.Name,
                Value = category.Id.ToString(),
            }).ToList();

            ProductDTO productToAdd = new()
            {
                CategoryList = categorySelectList
            };
            return View(productToAdd);
        }

        // AddProduct POST method
        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductDTO productToAdd)
        {
            // Populate the category list for the dropdown
            var categorySelectList = (await _categoryRepo.GetCategories()).Select(category => new SelectListItem
            {
                Text = category.Name,
                Value = category.Id.ToString(),
            }).ToList();
            productToAdd.CategoryList = categorySelectList;

            if (!ModelState.IsValid)
            {
                TempData["errorMessage"] = "Validation failed. Please correct the highlighted errors.";
                return View(productToAdd);
            }

            try
            {
                // Handle image upload if provided
                if (productToAdd.ImageFile != null)
                {
                    if (productToAdd.ImageFile.Length > 1 * 1024 * 1024) // 1 MB limit for image size
                    {
                        throw new InvalidOperationException("Image file size cannot exceed 1 MB.");
                    }

                    // Save the image file to wwwroot/product_images/
                    var wwwPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "product_images");

                    if (!Directory.Exists(wwwPath))
                    {
                        Directory.CreateDirectory(wwwPath); // Create the directory if it doesn't exist
                    }

                    string fileExtension = Path.GetExtension(productToAdd.ImageFile.FileName); // Get the file extension
                    string fileName = $"{Guid.NewGuid()}{fileExtension}"; // Generate a unique name for the file
                    string filePath = Path.Combine(wwwPath, fileName); // Full path where the file will be saved

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await productToAdd.ImageFile.CopyToAsync(fileStream); // Copy the file to the path
                    }

                    productToAdd.Image = fileName; // Save the filename to the DTO
                }

                // Map DTO to Product entity
                Product product = new()
                {
                    Name = productToAdd.Name,
                    Image = productToAdd.Image, // Use the file name saved above
                    Description = productToAdd.Description,
                    CategoryId = productToAdd.CategoryId,
                    Price = productToAdd.Price
                };

                // Save the product to the database
             
                    await _productRepo.AddProduct(product);
                    TempData["successMessage"] = "Product added successfully.";
                    return RedirectToAction(nameof(AddProduct));
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = $"An error occurred: {ex.Message}";
                    if (ex.InnerException != null)
                    {
                        TempData["errorMessage"] += $" Inner Exception: {ex.InnerException.Message}";
                    }
                    return View(productToAdd);
                }

            }

        // UpdateProduct GET method
        public async Task<IActionResult> UpdateProduct(int id)
        {
            var product = await _productRepo.GetProductById(id);
            if (product == null)
            {
                TempData["errorMessage"] = $"Product with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            var categorySelectList = (await _categoryRepo.GetCategories()).Select(category => new SelectListItem
            {
                Text = category.Name,
                Value = category.Id.ToString(),
                Selected = category.Id == product.CategoryId
            });

            ProductDTO productToUpdate = new()
            {
                Name = product.Name,
                CategoryId = product.CategoryId,
                Price = product.Price,
                Image = product.Image,
                Description = product.Description,
                CategoryList = categorySelectList
            };

            return View(productToUpdate);
        }

        // UpdateProduct POST method
        [HttpPost]
        public async Task<IActionResult> UpdateProduct(ProductDTO productToUpdate)
        {
            var categorySelectList = (await _categoryRepo.GetCategories()).Select(category => new SelectListItem
            {
                Text = category.Name,
                Value = category.Id.ToString(),
                Selected = category.Id == productToUpdate.CategoryId
            });
            productToUpdate.CategoryList = categorySelectList;

            if (!ModelState.IsValid)
                return View(productToUpdate);

            try
            {
                string oldImage = productToUpdate.Image;

                if (productToUpdate.ImageFile != null)
                {
                    if (productToUpdate.ImageFile.Length > 1 * 1024 * 1024) // 1 MB limit
                    {
                        throw new InvalidOperationException("Image file cannot exceed 1 MB.");
                    }

                    // Save the image file to wwwroot/product_images/
                    var wwwPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "product_images");

                    if (!Directory.Exists(wwwPath))
                    {
                        Directory.CreateDirectory(wwwPath); // Create the directory if it doesn't exist
                    }

                    string fileExtension = Path.GetExtension(productToUpdate.ImageFile.FileName); // Get the file extension
                    string fileName = $"{Guid.NewGuid()}{fileExtension}"; // Generate a unique name for the file
                    string filePath = Path.Combine(wwwPath, fileName); // Full path where the file will be saved

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await productToUpdate.ImageFile.CopyToAsync(fileStream); // Copy the file to the path
                    }

                    productToUpdate.Image = fileName; // Save the filename to the DTO
                }

                Product product = new()
                {
                    Id = productToUpdate.Id,
                    Name = productToUpdate.Name,
                    CategoryId = productToUpdate.CategoryId,
                    Description = productToUpdate.Description,
                    Price = productToUpdate.Price,
                    Image = productToUpdate.Image
                };

                await _productRepo.UpdateProduct(product);

                // Delete the old image if a new one is provided
                if (!string.IsNullOrWhiteSpace(oldImage) && oldImage != productToUpdate.Image)
                {
                    string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "product_images", oldImage);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                TempData["successMessage"] = "Product was updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(productToUpdate);
            }
        }

        // DeleteProduct method
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _productRepo.GetProductById(id);
                if (product == null)
                {
                    TempData["errorMessage"] = $"Product with ID {id} not found.";
                }
                else
                {
                    await _productRepo.DeleteProduct(product);

                    // Delete the product image
                    if (!string.IsNullOrWhiteSpace(product.Image))
                    {
                        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "product_images", product.Image);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    TempData["successMessage"] = "Product was deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
