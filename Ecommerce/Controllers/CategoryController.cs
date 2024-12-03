using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _CategoryRepo;

        public CategoryController(ICategoryRepository CategoryRepo)
        {
            _CategoryRepo = CategoryRepo;
        }

        public async Task<IActionResult> Index()
        {
            var Categorys = await _CategoryRepo.GetCategories();
            return View(Categorys);
        }

        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryDTO Category)
        {
            if (!ModelState.IsValid)
            {
                return View(Category);
            }
            try
            {
                var CategoryToAdd = new Category { Name = Category.Name, Id = Category.Id };
                await _CategoryRepo.AddCategory(CategoryToAdd);
                TempData["successMessage"] = "Category added successfully";
                return RedirectToAction(nameof(AddCategory));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Category could not added!";
                return View(Category);
            }

        }

        public async Task<IActionResult> UpdateCategory(int id)
        {
            var Category = await _CategoryRepo.GetCategoryById(id);
            if (Category is null)
                throw new InvalidOperationException($"Category with id: {id} does not found");
            var CategoryToUpdate = new CategoryDTO
            {
                Id = Category.Id,
                Name = Category.Name
            };
            return View(CategoryToUpdate);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(CategoryDTO CategoryToUpdate)
        {
            if (!ModelState.IsValid)
            {
                return View(CategoryToUpdate);
            }
            try
            {
                var Category = new Category { Name = CategoryToUpdate.Name, Id = CategoryToUpdate.Id };
                await _CategoryRepo.UpdateCategory(Category);
                TempData["successMessage"] = "Category is updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Category could not updated!";
                return View(CategoryToUpdate);
            }

        }

        public async Task<IActionResult> DeleteCategory(int id)
        {
            var Category = await _CategoryRepo.GetCategoryById(id);
            if (Category is null)
                throw new InvalidOperationException($"Category with id: {id} does not found");
            await _CategoryRepo.DeleteCategory(Category);
            return RedirectToAction(nameof(Index));

        }

    }
}