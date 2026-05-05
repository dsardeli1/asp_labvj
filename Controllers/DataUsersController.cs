using Microsoft.AspNetCore.Mvc;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers
{
    public class DataUsersController : Controller
    {
        private readonly ITaskRepository _taskRepository;

        public DataUsersController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<IActionResult> Users()
        {
            var users = await _taskRepository.GetAllUsersAsync();
            return View("~/Views/Data/Users.cshtml", users.OrderBy(u => u.Id).ToList());
        }

        public async Task<IActionResult> UserDetails(int id)
        {
            var user = await _taskRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View("~/Views/Data/UserDetails.cshtml", user);
        }
    }
}