using Microsoft.AspNetCore.Mvc;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers
{
    public class DataHistoriesController : Controller
    {
        private readonly ITaskRepository _taskRepository;

        public DataHistoriesController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<IActionResult> Histories()
        {
            var histories = await _taskRepository.GetAllTaskHistoriesAsync();
            return View("~/Web/Views/Data/Histories.cshtml", histories.OrderBy(h => h.Id).ToList());
        }

        public async Task<IActionResult> HistoryDetails(int id)
        {
            var history = await _taskRepository.GetTaskHistoryByIdAsync(id);
            if (history == null)
            {
                return NotFound();
            }

            return View("~/Web/Views/Data/HistoryDetails.cshtml", history);
        }
    }
}