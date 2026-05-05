using Microsoft.AspNetCore.Mvc;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers
{
    public class DataCategoriesController : Controller
    {
        private readonly ITaskRepository _taskRepository;

        public DataCategoriesController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<IActionResult> Categories()
        {
            var categories = await _taskRepository.GetAllCategoriesAsync();
            return View("~/Views/Data/Categories.cshtml", categories.OrderBy(c => c.Id).ToList());
        }

        public async Task<IActionResult> CategoryDetails(int id)
        {
            var category = await _taskRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View("~/Views/Data/CategoryDetails.cshtml", category);
        }
    }
}