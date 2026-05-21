using Microsoft.AspNetCore.Mvc;
using TaskManageApp.Models;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers
{
    [Route("data/histories")]
    public class DataHistoriesController : Controller
    {
        private readonly ITaskRepository _taskRepository;

        public DataHistoriesController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> Histories()
        {
            var histories = await _taskRepository.GetAllTaskHistoriesAsync();
            return View("~/Web/Views/Data/Histories.cshtml", histories.OrderBy(h => h.Id).ToList());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> HistoryDetails(int id)
        {
            var history = await _taskRepository.GetTaskHistoryByIdAsync(id);
            if (history == null)
            {
                return NotFound();
            }

            return View("~/Web/Views/Data/HistoryDetails.cshtml", history);
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup([FromQuery] string? q, [FromQuery] int limit = 10)
        {
            var searchTerm = q?.Trim();
            var maxResults = Math.Clamp(limit, 1, 50);
            var histories = await _taskRepository.GetAllTaskHistoriesAsync();

            var results = histories
                .Where(history =>
                    string.IsNullOrWhiteSpace(searchTerm) ||
                    history.Id.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    history.Action.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    history.TaskItemId.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (history.TaskItem != null && history.TaskItem.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                .OrderByDescending(history => history.ActionDate)
                .ThenBy(history => history.Id)
                .Take(maxResults)
                .Select(history => new
                {
                    value = history.Id,
                    text = $"#{history.Id} {history.Action}",
                    hint = history.TaskItem != null ? history.TaskItem.Title : null
                });

            return Json(results);
        }

        [HttpGet("find")]
        public IActionResult Find([FromQuery] int? id)
        {
            if (!id.HasValue || id.Value <= 0)
            {
                return RedirectToAction(nameof(Histories));
            }

            return RedirectToAction(nameof(HistoryDetails), new { id = id.Value });
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            ViewData["Tasks"] = await _taskRepository.GetAllTasksAsync();
            return View("~/Web/Views/Data/HistoryCreate.cshtml", new TaskHistory { ActionDate = DateTime.Now });
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Action,ActionDate,TaskItemId")] TaskHistory taskHistory)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Tasks"] = await _taskRepository.GetAllTasksAsync();
                return View("~/Web/Views/Data/HistoryCreate.cshtml", taskHistory);
            }

            await _taskRepository.AddTaskHistoryAsync(taskHistory);
            TempData["SuccessMessage"] = "Task history was created successfully.";
            return RedirectToAction(nameof(Histories));
        }

        [HttpGet("{id:int}/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var history = await _taskRepository.GetTaskHistoryByIdAsync(id);
            if (history == null)
            {
                return NotFound();
            }

            ViewData["Tasks"] = await _taskRepository.GetAllTasksAsync();
            return View("~/Web/Views/Data/HistoryEdit.cshtml", history);
        }

        [HttpPost("{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Action,ActionDate,TaskItemId")] TaskHistory taskHistory)
        {
            if (id != taskHistory.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                ViewData["Tasks"] = await _taskRepository.GetAllTasksAsync();
                return View("~/Web/Views/Data/HistoryEdit.cshtml", taskHistory);
            }

            var updated = await _taskRepository.UpdateTaskHistoryAsync(taskHistory);
            if (!updated)
            {
                TempData["ErrorMessage"] = "The task history could not be updated. Please try again.";
                return RedirectToAction(nameof(Histories));
            }

            TempData["SuccessMessage"] = "Task history was updated successfully.";
            return RedirectToAction(nameof(Histories));
        }

        [HttpGet("{id:int}/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var history = await _taskRepository.GetTaskHistoryByIdAsync(id);
            if (history == null)
            {
                return NotFound();
            }

            return View("~/Web/Views/Data/HistoryDelete.cshtml", history);
        }

        [HttpPost("{id:int}/delete")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _taskRepository.DeleteTaskHistoryAsync(id);
            if (!deleted)
            {
                TempData["ErrorMessage"] = "The task history could not be deleted. Please try again.";
                return RedirectToAction(nameof(Histories));
            }

            TempData["SuccessMessage"] = "Task history was deleted successfully.";
            return RedirectToAction(nameof(Histories));
        }
    }
}