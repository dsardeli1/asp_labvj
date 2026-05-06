using Microsoft.AspNetCore.Mvc;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers
{
    [Route("data/tasks")]
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
            return View("~/Views/Data/Tasks.cshtml", tasks.OrderBy(t => t.Id).ToList());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> TaskDetails(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            return View("~/Views/Data/TaskDetails.cshtml", task);
        }

        [HttpGet("completed")]
        public async Task<IActionResult> CompletedTasks()
        {
            var tasks = await _taskRepository.GetCompletedTasksAsync();
            return View("~/Views/Data/Tasks.cshtml", tasks.OrderBy(t => t.Id).ToList());
        }

        [HttpGet("pending")]
        public async Task<IActionResult> PendingTasks()
        {
            var tasks = await _taskRepository.GetPendingTasksAsync();
            return View("~/Views/Data/Tasks.cshtml", tasks.OrderBy(t => t.Id).ToList());
        }

        [HttpGet("category/{categoryId:int}")]
        public async Task<IActionResult> TasksByCategory(int categoryId)
        {
            var tasks = await _taskRepository.GetTasksByCategoryAsync(categoryId);
            return View("~/Views/Data/Tasks.cshtml", tasks.OrderBy(t => t.Id).ToList());
        }

        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> TasksByUser(int userId)
        {
            var tasks = await _taskRepository.GetTasksByUserAsync(userId);
            return View("~/Views/Data/Tasks.cshtml", tasks.OrderBy(t => t.Id).ToList());
        }
    }
}