using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using TaskManageApp.Models;

namespace TaskManageApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Create placeholder TaskItem objects
            var tasks = new List<TaskItem>
            {
                new TaskItem
                {
                    Id = 1,
                    Title = "Complete project proposal",
                    Description = "Write and finalize the project proposal document",
                    CreatedDate = DateTime.Now.AddDays(-5),
                    DueDate = DateTime.Now.AddDays(10),
                    IsCompleted = false,
                    PriorityId = (int)Priority.High,
                    UserId = 1,
                    CategoryId = 1
                },
                new TaskItem
                {
                    Id = 2,
                    Title = "Review code changes",
                    Description = "Review the latest code changes in the repository",
                    CreatedDate = DateTime.Now.AddDays(-3),
                    DueDate = DateTime.Now.AddDays(5),
                    IsCompleted = false,
                    PriorityId = (int)Priority.Medium,
                    UserId = 2,
                    CategoryId = 2
                },
                new TaskItem
                {
                    Id = 3,
                    Title = "Update documentation",
                    Description = "Update the user manual with new features",
                    CreatedDate = DateTime.Now.AddDays(-1),
                    DueDate = DateTime.Now.AddDays(15),
                    IsCompleted = true,
                    PriorityId = (int)Priority.Low,
                    UserId = 1,
                    CategoryId = 1
                }
            };

            // Create placeholder Comment objects
            var comments = new List<Comment>
            {
                new Comment
                {
                    Id = 1,
                    Content = "This task is critical for the project timeline.",
                    CreatedDate = DateTime.Now.AddDays(-4),
                    IsEdited = false,
                    TaskItemId = 1,
                    UserId = 1
                },
                new Comment
                {
                    Id = 2,
                    Content = "Please check the edge cases in the code review.",
                    CreatedDate = DateTime.Now.AddDays(-2),
                    IsEdited = true,
                    TaskItemId = 2,
                    UserId = 2
                },
                new Comment
                {
                    Id = 3,
                    Content = "Documentation has been updated successfully.",
                    CreatedDate = DateTime.Now.AddHours(-5),
                    IsEdited = false,
                    TaskItemId = 3,
                    UserId = 1
                }
            };

        var tasksWithLowId = tasks
        .Where(p => p.Id < 3);

        ViewData["TaskCount"] = tasksWithLowId.Count();

        var commentsWithFirstUser = comments
        .Where(p => p.UserId == 1);

        ViewData["CommentCount"] = commentsWithFirstUser.Count();

            return View();
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