using Microsoft.AspNetCore.Mvc;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers
{
    public class DataController : Controller
    {
        private readonly ITaskRepository _taskRepository;

        public DataController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Tasks()
        {
            var tasks = await _taskRepository.GetAllTasksAsync();
            return View(tasks.OrderBy(t => t.Id).ToList());
        }

        public async Task<IActionResult> Users()
        {
            var users = await _taskRepository.GetAllUsersAsync();
            return View(users.OrderBy(u => u.Id).ToList());
        }

        public async Task<IActionResult> Categories()
        {
            var categories = await _taskRepository.GetAllCategoriesAsync();
            return View(categories.OrderBy(c => c.Id).ToList());
        }

        public async Task<IActionResult> Comments()
        {
            var comments = await _taskRepository.GetAllCommentsAsync();
            return View(comments.OrderBy(c => c.Id).ToList());
        }

        public async Task<IActionResult> Attachments()
        {
            var attachments = await _taskRepository.GetAllTaskAttachmentsAsync();
            return View(attachments.OrderBy(a => a.Id).ToList());
        }

        public async Task<IActionResult> Histories()
        {
            var histories = await _taskRepository.GetAllTaskHistoriesAsync();
            return View(histories.OrderBy(h => h.Id).ToList());
        }

        public async Task<IActionResult> TaskDetails(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        public async Task<IActionResult> UserDetails(int id)
        {
            var user = await _taskRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public async Task<IActionResult> CategoryDetails(int id)
        {
            var category = await _taskRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        public async Task<IActionResult> CommentDetails(int id)
        {
            var comment = await _taskRepository.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        public async Task<IActionResult> AttachmentDetails(int id)
        {
            var attachment = await _taskRepository.GetTaskAttachmentByIdAsync(id);
            if (attachment == null)
            {
                return NotFound();
            }

            return View(attachment);
        }

        public async Task<IActionResult> HistoryDetails(int id)
        {
            var history = await _taskRepository.GetTaskHistoryByIdAsync(id);
            if (history == null)
            {
                return NotFound();
            }

            return View(history);
        }
    }
}
