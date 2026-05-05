using Microsoft.AspNetCore.Mvc;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers
{
    public class DataCommentsController : Controller
    {
        private readonly ITaskRepository _taskRepository;

        public DataCommentsController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<IActionResult> Comments()
        {
            var comments = await _taskRepository.GetAllCommentsAsync();
            return View("~/Views/Data/Comments.cshtml", comments.OrderBy(c => c.Id).ToList());
        }

        public async Task<IActionResult> CommentDetails(int id)
        {
            var comment = await _taskRepository.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            return View("~/Views/Data/CommentDetails.cshtml", comment);
        }
    }
}