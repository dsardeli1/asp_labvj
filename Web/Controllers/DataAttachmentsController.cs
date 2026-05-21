using Microsoft.AspNetCore.Mvc;
using TaskManageApp.Models;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers
{
    [Route("data/attachments")]
    public class DataAttachmentsController : Controller
    {
        private readonly ITaskRepository _taskRepository;

        public DataAttachmentsController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> Attachments()
        {
            var attachments = await _taskRepository.GetAllTaskAttachmentsAsync();
            return View("~/Web/Views/Data/Attachments.cshtml", attachments.OrderBy(a => a.Id).ToList());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> AttachmentDetails(int id)
        {
            var attachment = await _taskRepository.GetTaskAttachmentByIdAsync(id);
            if (attachment == null)
            {
                return NotFound();
            }

            return View("~/Web/Views/Data/AttachmentDetails.cshtml", attachment);
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup([FromQuery] string? q, [FromQuery] int limit = 10)
        {
            var searchTerm = q?.Trim();
            var maxResults = Math.Clamp(limit, 1, 50);
            var attachments = await _taskRepository.GetAllTaskAttachmentsAsync();

            var results = attachments
                .Where(attachment =>
                    string.IsNullOrWhiteSpace(searchTerm) ||
                    attachment.Id.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    attachment.FileName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    attachment.FilePath.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (attachment.TaskItem != null && attachment.TaskItem.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(attachment => attachment.FileName)
                .ThenBy(attachment => attachment.Id)
                .Take(maxResults)
                .Select(attachment => new
                {
                    value = attachment.Id,
                    text = $"#{attachment.Id} {attachment.FileName}",
                    hint = attachment.TaskItem != null ? attachment.TaskItem.Title : null
                });

            return Json(results);
        }

        [HttpGet("find")]
        public IActionResult Find([FromQuery] int? id)
        {
            if (!id.HasValue || id.Value <= 0)
            {
                return RedirectToAction(nameof(Attachments));
            }

            return RedirectToAction(nameof(AttachmentDetails), new { id = id.Value });
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            ViewData["Tasks"] = await _taskRepository.GetAllTasksAsync();
            return View("~/Web/Views/Data/AttachmentCreate.cshtml", new TaskAttachment());
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FileName,FilePath,TaskItemId")] TaskAttachment taskAttachment)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Tasks"] = await _taskRepository.GetAllTasksAsync();
                return View("~/Web/Views/Data/AttachmentCreate.cshtml", taskAttachment);
            }

            await _taskRepository.AddTaskAttachmentAsync(taskAttachment);
            TempData["SuccessMessage"] = "Task attachment was created successfully.";
            return RedirectToAction(nameof(Attachments));
        }

        [HttpGet("{id:int}/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var attachment = await _taskRepository.GetTaskAttachmentByIdAsync(id);
            if (attachment == null)
            {
                return NotFound();
            }

            ViewData["Tasks"] = await _taskRepository.GetAllTasksAsync();
            return View("~/Web/Views/Data/AttachmentEdit.cshtml", attachment);
        }

        [HttpPost("{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FileName,FilePath,TaskItemId")] TaskAttachment taskAttachment)
        {
            if (id != taskAttachment.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                ViewData["Tasks"] = await _taskRepository.GetAllTasksAsync();
                return View("~/Web/Views/Data/AttachmentEdit.cshtml", taskAttachment);
            }

            var updated = await _taskRepository.UpdateTaskAttachmentAsync(taskAttachment);
            if (!updated)
            {
                TempData["ErrorMessage"] = "The task attachment could not be updated. Please try again.";
                return RedirectToAction(nameof(Attachments));
            }

            TempData["SuccessMessage"] = "Task attachment was updated successfully.";
            return RedirectToAction(nameof(Attachments));
        }

        [HttpGet("{id:int}/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var attachment = await _taskRepository.GetTaskAttachmentByIdAsync(id);
            if (attachment == null)
            {
                return NotFound();
            }

            return View("~/Web/Views/Data/AttachmentDelete.cshtml", attachment);
        }

        [HttpPost("{id:int}/delete")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _taskRepository.DeleteTaskAttachmentAsync(id);
            if (!deleted)
            {
                TempData["ErrorMessage"] = "The task attachment could not be deleted. Please try again.";
                return RedirectToAction(nameof(Attachments));
            }

            TempData["SuccessMessage"] = "Task attachment was deleted successfully.";
            return RedirectToAction(nameof(Attachments));
        }
    }
}