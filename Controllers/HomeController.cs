using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
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

        public async Task<IActionResult> Tasks()
        {
            var tasks = await _taskRepository.GetAllTasksAsync();
            return View(tasks);
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