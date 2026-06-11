using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers
{
    [Route("data/users")]
    [Authorize]
    public class DataUsersController : Controller
    {
        private readonly ITaskRepository _repo;
        private readonly IPasswordHasher<Models.User> _passwordHasher;

        public DataUsersController(ITaskRepository repo, IPasswordHasher<Models.User> passwordHasher)
        {
            _repo = repo;
            _passwordHasher = passwordHasher;
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
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                ModelState.AddModelError(nameof(user.PasswordHash), "Password is required.");
            }

            if (!ModelState.IsValid) return View("~/Web/Views/Data/UserCreate.cshtml", user);

            user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash ?? string.Empty);
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
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                var existing = await _repo.GetUserByIdAsync(id);
                if (existing != null)
                {
                    user.PasswordHash = existing.PasswordHash;
                }
            }
            else
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash ?? string.Empty);
            }

            if (!ModelState.IsValid) return View("~/Web/Views/Data/UserEdit.cshtml", user);
            var updated = await _repo.UpdateUserAsync(user);
            if (!updated)
            {
                TempData["ErrorMessage"] = "Could not update user.";
            }
            else
            {
                TempData["SuccessMessage"] = "User updated.";
            }

            return RedirectToAction(nameof(Users));
        }

        [HttpGet("{id:int}/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _repo.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return View("~/Web/Views/Data/UserDelete.cshtml", user);
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup([FromQuery] string? q, [FromQuery] int limit = 10)
        {
            var searchTerm = q?.Trim();
            var maxResults = Math.Clamp(limit, 1, 50);
            var users = await _repo.GetAllUsersAsync();

            var results = users
                .Where(user =>
                    string.IsNullOrWhiteSpace(searchTerm) ||
                    user.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrWhiteSpace(user.Email) && user.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(user.FirstName) && user.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(user.LastName) && user.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(user => user.Username)
                .ThenBy(user => user.Id)
                .Take(maxResults)
                .Select(user => new
                {
                    value = user.Id,
                    text = user.Username,
                    hint = user.Email
                });

            return Json(results);
        }

        [HttpGet("find")]
        public IActionResult Find([FromQuery] int? id)
        {
            if (!id.HasValue || id.Value <= 0)
            {
                return RedirectToAction(nameof(Users));
            }

            return RedirectToAction(nameof(UserDetails), new { id = id.Value });
        }

        [HttpPost("{id:int}/delete")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _repo.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            var deleted = await _repo.DeleteUserAsync(id);
            if (!deleted)
            {
                TempData["ErrorMessage"] = "Could not delete user (might have tasks).";
            }
            else
            {
                TempData["SuccessMessage"] = "User deleted.";
            }

            return RedirectToAction(nameof(Users));
        }
    }
}