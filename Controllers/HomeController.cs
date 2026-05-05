using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaskManageApp.Models;
using TaskManageApp.Repositories;

namespace TaskManageApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITaskRepository _taskRepository;

        public HomeController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Tasks(string prioritySort = "none", string dueDateSort = "none", bool showCompleted = true)
        {
            var tasks = await _taskRepository.GetAllTasksAsync();

            var normalizedPrioritySort = NormalizePrioritySort(prioritySort);
            var normalizedDueDateSort = NormalizeDueDateSort(dueDateSort);

            IEnumerable<TaskItem> query = tasks;

            if (!showCompleted)
            {
                query = query.Where(t => !t.IsCompleted);
            }

            IOrderedEnumerable<TaskItem>? orderedTasks = null;

            switch (normalizedPrioritySort)
            {
                case "high":
                    orderedTasks = query.OrderByDescending(t => t.PriorityId);
                    break;
                case "low":
                    orderedTasks = query.OrderBy(t => t.PriorityId);
                    break;
            }

            switch (normalizedDueDateSort)
            {
                case "soonest":
                    orderedTasks = orderedTasks == null
                        ? query.OrderBy(t => t.DueDate)
                        : orderedTasks.ThenBy(t => t.DueDate);
                    break;
                case "latest":
                    orderedTasks = orderedTasks == null
                        ? query.OrderByDescending(t => t.DueDate)
                        : orderedTasks.ThenByDescending(t => t.DueDate);
                    break;
            }

            // Preserve the previous page order when no filters are selected.
            if (orderedTasks == null)
            {
                orderedTasks = query
                    .OrderBy(t => t.DueDate)
                    .ThenByDescending(t => t.PriorityId);
            }

            ViewBag.PrioritySort = normalizedPrioritySort;
            ViewBag.DueDateSort = normalizedDueDateSort;
            ViewBag.ShowCompleted = showCompleted;

            return View(orderedTasks.ToList());
        }

        private static string NormalizePrioritySort(string? prioritySort)
        {
            return prioritySort?.ToLowerInvariant() switch
            {
                "high" => "high",
                "low" => "low",
                _ => "none"
            };
        }

        private static string NormalizeDueDateSort(string? dueDateSort)
        {
            return dueDateSort?.ToLowerInvariant() switch
            {
                "soonest" => "soonest",
                "latest" => "latest",
                _ => "none"
            };
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}