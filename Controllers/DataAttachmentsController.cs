using Microsoft.AspNetCore.Mvc;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers
{
    public class DataAttachmentsController : Controller
    {
        private readonly ITaskRepository _taskRepository;

        public DataAttachmentsController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<IActionResult> Attachments()
        {
            var attachments = await _taskRepository.GetAllTaskAttachmentsAsync();
            return View("~/Views/Data/Attachments.cshtml", attachments.OrderBy(a => a.Id).ToList());
        }

        public async Task<IActionResult> AttachmentDetails(int id)
        {
            var attachment = await _taskRepository.GetTaskAttachmentByIdAsync(id);
            if (attachment == null)
            {
                return NotFound();
            }

            return View("~/Views/Data/AttachmentDetails.cshtml", attachment);
        }
    }
}