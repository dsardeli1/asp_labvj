using Microsoft.AspNetCore.Mvc;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers
{
    [Route("data/users")]
    public class DataUsersController : Controller
    {
        private readonly ITaskRepository _repo;

        public DataUsersController(ITaskRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("")]
        public async Task<IActionResult> Users()
        {
            var users = await _repo.GetAllUsersAsync();
            return View("~/Web/Views/Data/Users.cshtml", users.OrderBy(u => u.Id).ToList());
        }

        [HttpGet("with-tasks")]
        public async Task<IActionResult> UsersWithTasks()
        {
            var users = await _repo.GetUsersWithTasksAsync();
            return View("~/Web/Views/Data/Users.cshtml", users.OrderBy(u => u.Id).ToList());
        }

        [HttpGet("without-tasks")]
        public async Task<IActionResult> UsersWithoutTasks()
        {
            var users = await _repo.GetUsersWithoutTasksAsync();
            return View("~/Web/Views/Data/Users.cshtml", users.OrderBy(u => u.Id).ToList());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> UserDetails(int id)
        {
            var user = await _repo.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return View("~/Web/Views/Data/UserDetails.cshtml", user);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View("~/Web/Views/Data/UserCreate.cshtml", new Models.User { CreatedAt = DateTime.UtcNow });
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Email,FirstName,LastName,PasswordHash")] Models.User user)
        {
            if (!ModelState.IsValid) return View("~/Web/Views/Data/UserCreate.cshtml", user);
            user.CreatedAt = DateTime.UtcNow;
            var created = await _repo.AddUserAsync(user);
            TempData["SuccessMessage"] = "User created.";
            return RedirectToAction(nameof(Users));
        }

        [HttpGet("{id:int}/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _repo.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return View("~/Web/Views/Data/UserEdit.cshtml", user);
        }

        [HttpPost("{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Email,FirstName,LastName,PasswordHash")] Models.User user)
        {
            if (id != user.Id) return BadRequest();
            if (!ModelState.IsValid) return View("~/Web/Views/Data/UserEdit.cshtml", user);
            var updated = await _repo.UpdateUserAsync(user);
            if (!updated) TempData["ErrorMessage"] = "Could not update user.";
            return RedirectToAction(nameof(Users));
        }

        [HttpGet("{id:int}/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _repo.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return View("~/Web/Views/Data/UserDelete.cshtml", user);
        }

        [HttpPost("{id:int}/delete")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _repo.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            var deleted = await _repo.DeleteUserAsync(id);
            if (!deleted) TempData["ErrorMessage"] = "Could not delete user (might have tasks).";
            return RedirectToAction(nameof(Users));
        }
    }
}