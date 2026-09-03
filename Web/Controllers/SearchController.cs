using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManageApp.Repositories;
using TaskManageApp.ViewModels;

namespace TaskManageApp.Controllers
{
    [AllowAnonymous]
    [Route("search")]
    public class SearchController : Controller
    {
        private readonly ITaskRepository _taskRepository;

        private static readonly SearchResultViewModel[] Pages =
        {
            new("Home", "Your task management overview.", "Page", "/"),
            new("Tasks", "View, sort, and track work items.", "Page", "/tasks"),
            new("Privacy", "Read the application privacy information.", "Page", "/Home/Privacy"),
            new("Data Browser", "Browse repository entities and records.", "Menu", "/Data")
        };

        public SearchController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index([FromQuery] string? q)
        {
            var query = q?.Trim() ?? string.Empty;
            var results = Pages
                .Where(page => Matches(query, page.Title, page.Description))
                .ToList();
            var includesData = User.Identity?.IsAuthenticated == true;

            if (includesData)
            {
                var tasks = await _taskRepository.GetAllTasksAsync();
                results.AddRange(tasks
                    .Where(task => Matches(query, task.Id.ToString(), task.Title, task.Description))
                    .Select(task => new SearchResultViewModel(
                        task.Title,
                        $"Task #{task.Id} | {task.Category?.Name ?? "Uncategorized"}",
                        "Task",
                        $"/data/tasks/{task.Id}")));

                var categories = await _taskRepository.GetAllCategoriesAsync();
                results.AddRange(categories
                    .Where(category => Matches(query, category.Id.ToString(), category.Name, category.Description))
                    .Select(category => new SearchResultViewModel(
                        category.Name,
                        category.Description,
                        "Category",
                        $"/data/categories/{category.Id}")));

                var users = await _taskRepository.GetAllUsersAsync();
                results.AddRange(users
                    .Where(user => Matches(query, user.Id.ToString(), user.Username, user.Email, user.FirstName, user.LastName))
                    .Select(user => new SearchResultViewModel(
                        user.Username,
                        $"{user.FirstName} {user.LastName} | {user.Email}",
                        "User",
                        $"/data/users/{user.Id}")));

                var comments = await _taskRepository.GetAllCommentsAsync();
                results.AddRange(comments
                    .Where(comment => Matches(query, comment.Id.ToString(), comment.Content, comment.User?.Username, comment.TaskItem?.Title))
                    .Select(comment => new SearchResultViewModel(
                        $"Comment #{comment.Id}",
                        comment.Content,
                        "Comment",
                        $"/data/comments/{comment.Id}")));

                var attachments = await _taskRepository.GetAllTaskAttachmentsAsync();
                results.AddRange(attachments
                    .Where(attachment => Matches(query, attachment.Id.ToString(), attachment.FileName, attachment.FilePath, attachment.TaskItem?.Title))
                    .Select(attachment => new SearchResultViewModel(
                        attachment.FileName,
                        $"Attachment #{attachment.Id} | {attachment.TaskItem?.Title ?? "Unlinked task"}",
                        "Attachment",
                        $"/data/attachments/{attachment.Id}")));

                var histories = await _taskRepository.GetAllTaskHistoriesAsync();
                results.AddRange(histories
                    .Where(history => Matches(query, history.Id.ToString(), history.Action, history.TaskItemId.ToString(), history.TaskItem?.Title))
                    .Select(history => new SearchResultViewModel(
                        $"History #{history.Id}",
                        $"{history.Action} | {history.TaskItem?.Title ?? "Unlinked task"}",
                        "History",
                        $"/data/histories/{history.Id}")));
            }

            return View("~/Web/Views/Search/Index.cshtml", new SearchViewModel
            {
                Query = query,
                Results = results
                    .OrderBy(result => result.Type == "Page" ? 0 : 1)
                    .ThenBy(result => result.Title)
                    .Take(100)
                    .ToList(),
                IncludesData = includesData
            });
        }

        private static bool Matches(string query, params string?[] values)
        {
            return string.IsNullOrWhiteSpace(query) || values.Any(value =>
                !string.IsNullOrWhiteSpace(value) && value.Contains(query, StringComparison.OrdinalIgnoreCase));
        }
    }
}
