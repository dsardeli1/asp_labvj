using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManageApp.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TaskManageApp.Controllers
{
    [Route("data/tasks")]
    [Authorize]
    public class DataTasksController : Controller
    {
        private readonly ITaskRepository _taskRepository;

        public DataTasksController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> Tasks()
        {
            var tasks = await _taskRepository.GetAllTasksAsync();
            return View("~/Web/Views/Data/Tasks.cshtml", tasks.OrderBy(t => t.Id).ToList());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> TaskDetails(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            return View("~/Web/Views/Data/TaskDetails.cshtml", task);
        }

        [HttpGet("completed")]
        public async Task<IActionResult> CompletedTasks()
        {
            var tasks = await _taskRepository.GetCompletedTasksAsync();
            return View("~/Web/Views/Data/Tasks.cshtml", tasks.OrderBy(t => t.Id).ToList());
        }

        [HttpGet("pending")]
        public async Task<IActionResult> PendingTasks()
        {
            var tasks = await _taskRepository.GetPendingTasksAsync();
            return View("~/Web/Views/Data/Tasks.cshtml", tasks.OrderBy(t => t.Id).ToList());
        }

        [HttpGet("category/{categoryId:int}")]
        public async Task<IActionResult> TasksByCategory(int categoryId)
        {
            var tasks = await _taskRepository.GetTasksByCategoryAsync(categoryId);
            return View("~/Web/Views/Data/Tasks.cshtml", tasks.OrderBy(t => t.Id).ToList());
        }

        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> TasksByUser(int userId)
        {
            var tasks = await _taskRepository.GetTasksByUserAsync(userId);
            return View("~/Web/Views/Data/Tasks.cshtml", tasks.OrderBy(t => t.Id).ToList());
        }

        [HttpGet("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var users = await _taskRepository.GetAllUsersAsync();
            var categories = await _taskRepository.GetAllCategoriesAsync();
            ViewData["UsersSelect"] = new SelectList(users, "Id", "Username");
            ViewData["CategoriesSelect"] = new SelectList(categories, "Id", "Name");
            ViewData["PrioritiesSelect"] = new SelectList(new[] {
                new { Id = (int)Models.Priority.Low, Name = "Low" },
                new { Id = (int)Models.Priority.Medium, Name = "Medium" },
                new { Id = (int)Models.Priority.High, Name = "High" }
            }, "Id", "Name");

            return View("~/Web/Views/Data/TaskCreate.cshtml", new Models.TaskItem { CreatedDate = DateTime.Now, DueDate = DateTime.Today.AddDays(7) });
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,DueDate,IsCompleted,PriorityId,UserId,CategoryId")] Models.TaskItem task)
        {
            if (!ModelState.IsValid)
            {
                var users = await _taskRepository.GetAllUsersAsync();
                var categories = await _taskRepository.GetAllCategoriesAsync();
                ViewData["UsersSelect"] = new SelectList(users, "Id", "Username", task.UserId);
                ViewData["CategoriesSelect"] = new SelectList(categories, "Id", "Name", task.CategoryId);
                ViewData["PrioritiesSelect"] = new SelectList(new[] {
                    new { Id = (int)Models.Priority.Low, Name = "Low" },
                    new { Id = (int)Models.Priority.Medium, Name = "Medium" },
                    new { Id = (int)Models.Priority.High, Name = "High" }
                }, "Id", "Name", task.PriorityId);
                return View("~/Web/Views/Data/TaskCreate.cshtml", task);
            }

            try
            {
                task.CreatedDate = DateTime.UtcNow;
                var created = await _taskRepository.AddTaskAsync(task);
                TempData["SuccessMessage"] = "Task was created successfully.";
                return RedirectToAction(nameof(Tasks));
            }
            catch
            {
                TempData["ErrorMessage"] = "The task could not be created. Please try again.";
                var users = await _taskRepository.GetAllUsersAsync();
                var categories = await _taskRepository.GetAllCategoriesAsync();
                ViewData["UsersSelect"] = new SelectList(users, "Id", "Username", task.UserId);
                ViewData["CategoriesSelect"] = new SelectList(categories, "Id", "Name", task.CategoryId);
                ViewData["PrioritiesSelect"] = new SelectList(new[] {
                    new { Id = (int)Models.Priority.Low, Name = "Low" },
                    new { Id = (int)Models.Priority.Medium, Name = "Medium" },
                    new { Id = (int)Models.Priority.High, Name = "High" }
                }, "Id", "Name", task.PriorityId);
                return View("~/Web/Views/Data/TaskCreate.cshtml", task);
            }
        }

        [HttpGet("{id:int}/edit")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null) return NotFound();
            var users = await _taskRepository.GetAllUsersAsync();
            var categories = await _taskRepository.GetAllCategoriesAsync();
            ViewData["UsersSelect"] = new SelectList(users, "Id", "Username", task.UserId);
            ViewData["CategoriesSelect"] = new SelectList(categories, "Id", "Name", task.CategoryId);
            ViewData["PrioritiesSelect"] = new SelectList(new[] {
                new { Id = (int)Models.Priority.Low, Name = "Low" },
                new { Id = (int)Models.Priority.Medium, Name = "Medium" },
                new { Id = (int)Models.Priority.High, Name = "High" }
            }, "Id", "Name", task.PriorityId);
            return View("~/Web/Views/Data/TaskEdit.cshtml", task);
        }

        [HttpPost("{id:int}/edit")]
        [Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,DueDate,IsCompleted,PriorityId,UserId,CategoryId")] Models.TaskItem task)
        {
            if (id != task.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                var users = await _taskRepository.GetAllUsersAsync();
                var categories = await _taskRepository.GetAllCategoriesAsync();
                ViewData["UsersSelect"] = new SelectList(users, "Id", "Username", task.UserId);
                ViewData["CategoriesSelect"] = new SelectList(categories, "Id", "Name", task.CategoryId);
                ViewData["PrioritiesSelect"] = new SelectList(new[] {
                    new { Id = (int)Models.Priority.Low, Name = "Low" },
                    new { Id = (int)Models.Priority.Medium, Name = "Medium" },
                    new { Id = (int)Models.Priority.High, Name = "High" }
                }, "Id", "Name", task.PriorityId);
                return View("~/Web/Views/Data/TaskEdit.cshtml", task);
            }

            var existing = await _taskRepository.GetTaskByIdAsync(id);
            if (existing == null) return NotFound();

            var updated = await _taskRepository.UpdateTaskAsync(task);
            if (!updated)
            {
                TempData["ErrorMessage"] = "The task could not be updated. Please try again.";
                return RedirectToAction(nameof(Tasks));
            }

            TempData["SuccessMessage"] = "Task was updated successfully.";
            return RedirectToAction(nameof(Tasks));
        }

        [HttpGet("{id:int}/delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null) return NotFound();
            return View("~/Web/Views/Data/TaskDelete.cshtml", task);
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup([FromQuery] string? q, [FromQuery] int limit = 10)
        {
            var searchTerm = q?.Trim();
            var maxResults = Math.Clamp(limit, 1, 50);
            var tasks = await _taskRepository.GetAllTasksAsync();

            var results = tasks
                .Where(task =>
                    string.IsNullOrWhiteSpace(searchTerm) ||
                    task.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrWhiteSpace(task.Description) && task.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    task.Id.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .OrderBy(task => task.Title)
                .ThenBy(task => task.Id)
                .Take(maxResults)
                .Select(task => new
                {
                    value = task.Id,
                    text = task.Title,
                    hint = $"#{task.Id}"
                });

            return Json(results);
        }

        [HttpGet("find")]
        public IActionResult Find([FromQuery] int? id)
        {
            if (!id.HasValue || id.Value <= 0)
            {
                return RedirectToAction(nameof(Tasks));
            }

            return RedirectToAction(nameof(TaskDetails), new { id = id.Value });
        }

        [HttpPost("{id:int}/delete")]
        [Authorize(Roles = "Admin")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null) return NotFound();

            var deleted = await _taskRepository.DeleteTaskAsync(id);
            if (!deleted)
            {
                TempData["ErrorMessage"] = "The task could not be deleted. Please try again.";
                return RedirectToAction(nameof(Tasks));
            }

            TempData["SuccessMessage"] = "Task was deleted successfully.";
            return RedirectToAction(nameof(Tasks));
        }
    }
}