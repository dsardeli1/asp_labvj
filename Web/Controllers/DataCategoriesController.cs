using Microsoft.AspNetCore.Mvc;
using TaskManageApp.Models;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers
{
    [Route("data/categories")]
    public class DataCategoriesController : Controller
    {
        private readonly ITaskRepository _taskRepository;

        public DataCategoriesController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> Categories()
        {
            var categories = await _taskRepository.GetAllCategoriesAsync();
            return View("~/Web/Views/Data/Categories.cshtml", categories.OrderBy(c => c.Id).ToList());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> CategoryDetails(int id)
        {
            var category = await _taskRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View("~/Web/Views/Data/CategoryDetails.cshtml", category);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View("~/Web/Views/Data/CategoryCreate.cshtml", new Category { IsActive = true });
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Color,IsActive")] Category category)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Web/Views/Data/CategoryCreate.cshtml", category);
            }

            // Server-side uniqueness validation
            var isUnique = await _taskRepository.IsCategoryNameUniqueAsync(category.Name);
            if (!isUnique)
            {
                ModelState.AddModelError("Name", "A category with this name already exists.");
                return View("~/Web/Views/Data/CategoryCreate.cshtml", category);
            }

            try
            {
                category.CreatedDate = DateTime.UtcNow;
                var createdCategory = await _taskRepository.AddCategoryAsync(category);
                TempData["SuccessMessage"] = $"Category '{createdCategory.Name}' was created successfully.";
                return RedirectToAction(nameof(Categories));
            }
            catch
            {
                TempData["ErrorMessage"] = "The category could not be created. Please try again.";
                return View("~/Web/Views/Data/CategoryCreate.cshtml", category);
            }
        }

        [HttpGet("{id:int}/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _taskRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View("~/Web/Views/Data/CategoryEdit.cshtml", category);
        }

        [HttpPost("{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Color,IsActive,CreatedDate")] Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View("~/Web/Views/Data/CategoryEdit.cshtml", category);
            }

            var existingCategory = await _taskRepository.GetCategoryByIdAsync(id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            category.CreatedDate = existingCategory.CreatedDate;
            // Server-side uniqueness validation for edits (exclude current id)
            var editUnique = await _taskRepository.IsCategoryNameUniqueAsync(category.Name, category.Id);
            if (!editUnique)
            {
                ModelState.AddModelError("Name", "A category with this name already exists.");
                return View("~/Web/Views/Data/CategoryEdit.cshtml", category);
            }

            var updated = await _taskRepository.UpdateCategoryAsync(category);
            if (!updated)
            {
                TempData["ErrorMessage"] = "The category could not be updated. Please try again.";
                return RedirectToAction(nameof(Categories));
            }

            TempData["SuccessMessage"] = $"Category '{category.Name}' was updated successfully.";
            return RedirectToAction(nameof(Categories));
        }

        [HttpGet("{id:int}/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _taskRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View("~/Web/Views/Data/CategoryDelete.cshtml", category);
        }

        [HttpPost("{id:int}/delete")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _taskRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var deleted = await _taskRepository.DeleteCategoryAsync(id);
            if (!deleted)
            {
                TempData["ErrorMessage"] = "This category cannot be deleted while tasks are still assigned to it.";
                return RedirectToAction(nameof(Categories));
            }

            TempData["SuccessMessage"] = $"Category '{category.Name}' was deleted successfully.";
            return RedirectToAction(nameof(Categories));
        }
    }
}