using Microsoft.AspNetCore.Mvc;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers
{
    public class DataTasksController : Controller
    {
        private readonly ITaskRepository _taskRepository;

        public DataTasksController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<IActionResult> Tasks()
        {
            var tasks = await _taskRepository.GetAllTasksAsync();
            return View("~/Views/Data/Tasks.cshtml", tasks.OrderBy(t => t.Id).ToList());
        }

        public async Task<IActionResult> TaskDetails(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            return View("~/Views/Data/TaskDetails.cshtml", task);
        }
    }
}