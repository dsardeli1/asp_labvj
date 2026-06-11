using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManageApp.Models;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers
{
    [Route("data/categories")]
    [Authorize]
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
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View("~/Web/Views/Data/CategoryCreate.cshtml", new Category { IsActive = true });
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin,Manager")]
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
        [Authorize(Roles = "Admin,Manager")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup([FromQuery] string? q, [FromQuery] int limit = 10)
        {
            var searchTerm = q?.Trim();
            var maxResults = Math.Clamp(limit, 1, 50);
            var categories = await _taskRepository.GetAllCategoriesAsync();

            var results = categories
                .Where(category =>
                    string.IsNullOrWhiteSpace(searchTerm) ||
                    category.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrWhiteSpace(category.Description) && category.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    category.Id.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .OrderBy(category => category.Name)
                .ThenBy(category => category.Id)
                .Take(maxResults)
                .Select(category => new
                {
                    value = category.Id,
                    text = category.Name,
                    hint = category.Description
                });

            return Json(results);
        }

        [HttpGet("find")]
        public IActionResult Find([FromQuery] int? id)
        {
            if (!id.HasValue || id.Value <= 0)
            {
                return RedirectToAction(nameof(Categories));
            }

            return RedirectToAction(nameof(CategoryDetails), new { id = id.Value });
        }
    }
}