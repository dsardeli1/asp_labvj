using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManageApp.Models;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers
{
    [Route("data/comments")]
    [Authorize]
    public class DataCommentsController : Controller
    {
        private readonly ITaskRepository _taskRepository;

        public DataCommentsController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> Comments()
        {
            var comments = await _taskRepository.GetAllCommentsAsync();
            return View("~/Web/Views/Data/Comments.cshtml", comments.OrderBy(c => c.Id).ToList());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> CommentDetails(int id)
        {
            var comment = await _taskRepository.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            return View("~/Web/Views/Data/CommentDetails.cshtml", comment);
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup([FromQuery] string? q, [FromQuery] int limit = 10)
        {
            var searchTerm = q?.Trim();
            var maxResults = Math.Clamp(limit, 1, 50);
            var comments = await _taskRepository.GetAllCommentsAsync();

            var results = comments
                .Where(comment =>
                    string.IsNullOrWhiteSpace(searchTerm) ||
                    comment.Id.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    comment.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (comment.User != null && comment.User.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (comment.TaskItem != null && comment.TaskItem.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                .OrderByDescending(comment => comment.CreatedDate)
                .ThenBy(comment => comment.Id)
                .Take(maxResults)
                .Select(comment => new
                {
                    value = comment.Id,
                    text = $"#{comment.Id} {comment.Content}",
                    hint = comment.User != null ? comment.User.Username : null
                });

            return Json(results);
        }

        [HttpGet("find")]
        public IActionResult Find([FromQuery] int? id)
        {
            if (!id.HasValue || id.Value <= 0)
            {
                return RedirectToAction(nameof(Comments));
            }

            return RedirectToAction(nameof(CommentDetails), new { id = id.Value });
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            var tasks = await _taskRepository.GetAllTasksAsync();
            var users = await _taskRepository.GetAllUsersAsync();
            ViewData["Tasks"] = tasks;
            ViewData["Users"] = users;
            return View("~/Web/Views/Data/CommentCreate.cshtml", new Comment());
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Content,TaskItemId,UserId")] Comment comment)
        {
            if (!ModelState.IsValid)
            {
                var tasks = await _taskRepository.GetAllTasksAsync();
                var users = await _taskRepository.GetAllUsersAsync();
                ViewData["Tasks"] = tasks;
                ViewData["Users"] = users;
                return View("~/Web/Views/Data/CommentCreate.cshtml", comment);
            }

            try
            {
                comment.CreatedDate = DateTime.UtcNow;
                comment.IsEdited = false;
                var createdComment = await _taskRepository.AddCommentAsync(comment);
                TempData["SuccessMessage"] = $"Comment was created successfully.";
                return RedirectToAction(nameof(Comments));
            }
            catch
            {
                TempData["ErrorMessage"] = "The comment could not be created. Please try again.";
                var tasks = await _taskRepository.GetAllTasksAsync();
                var users = await _taskRepository.GetAllUsersAsync();
                ViewData["Tasks"] = tasks;
                ViewData["Users"] = users;
                return View("~/Web/Views/Data/CommentCreate.cshtml", comment);
            }
        }

        [HttpGet("{id:int}/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var comment = await _taskRepository.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var tasks = await _taskRepository.GetAllTasksAsync();
            var users = await _taskRepository.GetAllUsersAsync();
            ViewData["Tasks"] = tasks;
            ViewData["Users"] = users;
            return View("~/Web/Views/Data/CommentEdit.cshtml", comment);
        }

        [HttpPost("{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content,TaskItemId,UserId")] Comment comment)
        {
            if (id != comment.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                var tasks = await _taskRepository.GetAllTasksAsync();
                var users = await _taskRepository.GetAllUsersAsync();
                ViewData["Tasks"] = tasks;
                ViewData["Users"] = users;
                return View("~/Web/Views/Data/CommentEdit.cshtml", comment);
            }

            var existingComment = await _taskRepository.GetCommentByIdAsync(id);
            if (existingComment == null)
            {
                return NotFound();
            }

            var updated = await _taskRepository.UpdateCommentAsync(comment);
            if (!updated)
            {
                TempData["ErrorMessage"] = "The comment could not be updated. Please try again.";
                return RedirectToAction(nameof(Comments));
            }

            TempData["SuccessMessage"] = $"Comment was updated successfully.";
            return RedirectToAction(nameof(Comments));
        }

        [HttpGet("{id:int}/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _taskRepository.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            return View("~/Web/Views/Data/CommentDelete.cshtml", comment);
        }

        [HttpPost("{id:int}/delete")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _taskRepository.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var deleted = await _taskRepository.DeleteCommentAsync(id);
            if (!deleted)
            {
                TempData["ErrorMessage"] = "The comment could not be deleted. Please try again.";
                return RedirectToAction(nameof(Comments));
            }

            TempData["SuccessMessage"] = $"Comment was deleted successfully.";
            return RedirectToAction(nameof(Comments));
        }
    }
}