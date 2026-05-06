using Microsoft.AspNetCore.Mvc;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers
{
    [Route("data/users")]
    public class DataUsersController : Controller
    {
        private readonly ITaskRepository _taskRepository;

        public DataUsersController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> Users()
        {
            var users = await _taskRepository.GetAllUsersAsync();
            return View("~/Web/Views/Data/Users.cshtml", users.OrderBy(u => u.Id).ToList());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> UserDetails(int id)
        {
            var user = await _taskRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View("~/Web/Views/Data/UserDetails.cshtml", user);
        }

        [HttpGet("with-tasks")]
        public async Task<IActionResult> UsersWithTasks()
        {
            var users = await _taskRepository.GetAllUsersAsync();
            var usersWithTasks = users.Where(u => u.Tasks != null && u.Tasks.Count > 0).ToList();
            return View("~/Web/Views/Data/Users.cshtml", usersWithTasks.OrderBy(u => u.Id).ToList());
        }

        [HttpGet("without-tasks")]
        public async Task<IActionResult> UsersWithoutTasks()
        {
            var users = await _taskRepository.GetAllUsersAsync();
            var usersWithoutTasks = users.Where(u => u.Tasks == null || u.Tasks.Count == 0).ToList();
            return View("~/Web/Views/Data/Users.cshtml", usersWithoutTasks.OrderBy(u => u.Id).ToList());
        }
    }
}