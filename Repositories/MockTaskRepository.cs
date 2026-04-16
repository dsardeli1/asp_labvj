using TaskManageApp.Models;

namespace TaskManageApp.Repositories
{
    /// <summary>
    /// Mock implementation of ITaskRepository with deterministic seeded data.
    /// Used for UI development before the real database layer is ready.
    /// </summary>
    public class MockTaskRepository : ITaskRepository
    {
        private readonly List<User> _users;
        private readonly List<Category> _categories;
        private readonly List<Comment> _comments;
        private readonly List<TaskAttachment> _attachments;
        private readonly List<TaskHistory> _taskHistories;
        private readonly List<TaskItem> _tasks;

        public MockTaskRepository()
        {
            _users = SeedUsers();
            _categories = SeedCategories();
            _tasks = SeedTasks();
            _comments = SeedComments();
            _attachments = SeedTaskAttachments();
            _taskHistories = SeedTaskHistories();
            WireRelations();
        }

        public Task<List<TaskItem>> GetAllTasksAsync()
        {
            return Task.FromResult(_tasks);
        }

        public Task<TaskItem> GetTaskByIdAsync(int id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            return Task.FromResult(task);
        }

        public Task<List<TaskItem>> GetTasksByCategoryAsync(int categoryId)
        {
            return Task.FromResult(_tasks.Where(t => t.CategoryId == categoryId).ToList());
        }

        public Task<List<TaskItem>> GetTasksByUserAsync(int userId)
        {
            return Task.FromResult(_tasks.Where(t => t.UserId == userId).ToList());
        }

        public Task<List<TaskItem>> GetCompletedTasksAsync()
        {
            return Task.FromResult(_tasks.Where(t => t.IsCompleted).ToList());
        }

        public Task<List<TaskItem>> GetPendingTasksAsync()
        {
            return Task.FromResult(_tasks.Where(t => !t.IsCompleted).ToList());
        }

        public Task<List<User>> GetAllUsersAsync()
        {
            return Task.FromResult(_users);
        }

        public Task<User> GetUserByIdAsync(int id)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
        }

        public Task<List<Category>> GetAllCategoriesAsync()
        {
            return Task.FromResult(_categories);
        }

        public Task<Category> GetCategoryByIdAsync(int id)
        {
            return Task.FromResult(_categories.FirstOrDefault(c => c.Id == id));
        }

        public Task<List<Comment>> GetAllCommentsAsync()
        {
            return Task.FromResult(_comments);
        }

        public Task<Comment> GetCommentByIdAsync(int id)
        {
            return Task.FromResult(_comments.FirstOrDefault(c => c.Id == id));
        }

        public Task<List<Comment>> GetCommentsByTaskIdAsync(int taskItemId)
        {
            return Task.FromResult(_comments.Where(c => c.TaskItemId == taskItemId).ToList());
        }

        public Task<List<TaskAttachment>> GetAllTaskAttachmentsAsync()
        {
            return Task.FromResult(_attachments);
        }

        public Task<TaskAttachment> GetTaskAttachmentByIdAsync(int id)
        {
            return Task.FromResult(_attachments.FirstOrDefault(a => a.Id == id));
        }

        public Task<List<TaskHistory>> GetAllTaskHistoriesAsync()
        {
            return Task.FromResult(_taskHistories);
        }

        public Task<TaskHistory> GetTaskHistoryByIdAsync(int id)
        {
            return Task.FromResult(_taskHistories.FirstOrDefault(h => h.Id == id));
        }

        private List<User> SeedUsers()
        {
            var now = DateTime.Now;
            return new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "ana.kovacic",
                    Email = "ana.kovacic@example.com",
                    PasswordHash = "mock-hash-1",
                    FirstName = "Ana",
                    LastName = "Kovacic",
                    CreatedAt = now.AddMonths(-6),
                    Tasks = new List<TaskItem>(),
                    Comments = new List<Comment>()
                },
                new User
                {
                    Id = 2,
                    Username = "marko.horvat",
                    Email = "marko.horvat@example.com",
                    PasswordHash = "mock-hash-2",
                    FirstName = "Marko",
                    LastName = "Horvat",
                    CreatedAt = now.AddMonths(-4),
                    Tasks = new List<TaskItem>(),
                    Comments = new List<Comment>()
                },
                new User
                {
                    Id = 3,
                    Username = "petra.babic",
                    Email = "petra.babic@example.com",
                    PasswordHash = "mock-hash-3",
                    FirstName = "Petra",
                    LastName = "Babic",
                    CreatedAt = now.AddMonths(-2),
                    Tasks = new List<TaskItem>(),
                    Comments = new List<Comment>()
                }
            };
        }

        private List<Category> SeedCategories()
        {
            var now = DateTime.Now;
            return new List<Category>
            {
                new Category
                {
                    Id = 1,
                    Name = "Planning",
                    Description = "Planning and documentation tasks",
                    Color = "#3b82f6",
                    CreatedDate = now.AddMonths(-8),
                    IsActive = true,
                    Tasks = new List<TaskItem>()
                },
                new Category
                {
                    Id = 2,
                    Name = "Development",
                    Description = "Implementation and code review work",
                    Color = "#10b981",
                    CreatedDate = now.AddMonths(-8),
                    IsActive = true,
                    Tasks = new List<TaskItem>()
                },
                new Category
                {
                    Id = 3,
                    Name = "Operations",
                    Description = "Infrastructure and monitoring",
                    Color = "#f59e0b",
                    CreatedDate = now.AddMonths(-8),
                    IsActive = true,
                    Tasks = new List<TaskItem>()
                }
            };
        }

        /// <summary>
        /// Seed realistic, varied mock data covering normal and edge cases.
        /// </summary>
        private List<TaskItem> SeedTasks()
        {
            var now = DateTime.Now;
            return new List<TaskItem>
            {
                // High priority, overdue
                new TaskItem
                {
                    Id = 1,
                    Title = "Complete project proposal",
                    Description = "Write and finalize the comprehensive project proposal document including scope, timeline, and budget estimates.",
                    CreatedDate = now.AddDays(-15),
                    DueDate = now.AddDays(-2),
                    IsCompleted = false,
                    PriorityId = (int)Priority.High,
                    UserId = 1,
                    CategoryId = 1,
                    Comments = new List<Comment>(),
                    TaskHistories = new List<TaskHistory>()
                },
                // High priority, due soon
                new TaskItem
                {
                    Id = 2,
                    Title = "Review code changes",
                    Description = "Review the latest code changes in the main repository branch.",
                    CreatedDate = now.AddDays(-5),
                    DueDate = now.AddDays(2),
                    IsCompleted = false,
                    PriorityId = (int)Priority.High,
                    UserId = 2,
                    CategoryId = 2,
                    Comments = new List<Comment>(),
                    TaskHistories = new List<TaskHistory>()
                },
                // Medium priority, completed
                new TaskItem
                {
                    Id = 3,
                    Title = "Update documentation",
                    Description = "Update the user manual with new features released in v2.1.",
                    CreatedDate = now.AddDays(-10),
                    DueDate = now.AddDays(-1),
                    IsCompleted = true,
                    PriorityId = (int)Priority.Medium,
                    UserId = 1,
                    CategoryId = 1,
                    Comments = new List<Comment>(),
                    TaskHistories = new List<TaskHistory>()
                },
                // Medium priority, upcoming
                new TaskItem
                {
                    Id = 4,
                    Title = "Refactor authentication module",
                    Description = "Refactor the authentication module to improve security and performance.",
                    CreatedDate = now.AddDays(-3),
                    DueDate = now.AddDays(7),
                    IsCompleted = false,
                    PriorityId = (int)Priority.Medium,
                    UserId = 1,
                    CategoryId = 2,
                    Comments = new List<Comment>(),
                    TaskHistories = new List<TaskHistory>()
                },
                // Low priority, long due date
                new TaskItem
                {
                    Id = 5,
                    Title = "Setup monitoring dashboard",
                    Description = "Setup application monitoring and performance dashboard.",
                    CreatedDate = now.AddDays(-1),
                    DueDate = now.AddDays(30),
                    IsCompleted = false,
                    PriorityId = (int)Priority.Low,
                    UserId = 2,
                    CategoryId = 3,
                    Comments = new List<Comment>(),
                    TaskHistories = new List<TaskHistory>()
                },
                // Completed recently
                new TaskItem
                {
                    Id = 6,
                    Title = "Fix login bug",
                    Description = "Fix the issue where users cannot login with special characters in password.",
                    CreatedDate = now.AddDays(-7),
                    DueDate = now.AddDays(-1),
                    IsCompleted = true,
                    PriorityId = (int)Priority.High,
                    UserId = 2,
                    CategoryId = 2,
                    Comments = new List<Comment>(),
                    TaskHistories = new List<TaskHistory>()
                },
                // Edge case: very long title
                new TaskItem
                {
                    Id = 7,
                    Title = "Implement comprehensive API rate limiting and caching strategy to prevent abuse while maintaining optimal performance",
                    Description = "Design and implement rate limiting and caching mechanisms.",
                    CreatedDate = now.AddDays(-2),
                    DueDate = now.AddDays(14),
                    IsCompleted = false,
                    PriorityId = (int)Priority.Medium,
                    UserId = 1,
                    CategoryId = 2,
                    Comments = new List<Comment>(),
                    TaskHistories = new List<TaskHistory>()
                }
            };
        }

        private List<Comment> SeedComments()
        {
            var now = DateTime.Now;
            return new List<Comment>
            {
                new Comment
                {
                    Id = 1,
                    Content = "Proposal draft looks good. Please expand the risk section.",
                    CreatedDate = now.AddDays(-6),
                    IsEdited = false,
                    TaskItemId = 1,
                    UserId = 2
                },
                new Comment
                {
                    Id = 2,
                    Content = "I pushed updates to the timeline and dependency chart.",
                    CreatedDate = now.AddDays(-4),
                    IsEdited = false,
                    TaskItemId = 1,
                    UserId = 1
                },
                new Comment
                {
                    Id = 3,
                    Content = "Please verify null-handling in authentication middleware.",
                    CreatedDate = now.AddDays(-2),
                    IsEdited = true,
                    TaskItemId = 4,
                    UserId = 3
                },
                new Comment
                {
                    Id = 4,
                    Content = "Monitoring panel draft is ready for review.",
                    CreatedDate = now.AddHours(-20),
                    IsEdited = false,
                    TaskItemId = 5,
                    UserId = 2
                },
                new Comment
                {
                    Id = 5,
                    Content = "Rate limiting thresholds need product confirmation.",
                    CreatedDate = now.AddHours(-8),
                    IsEdited = false,
                    TaskItemId = 7,
                    UserId = 1
                }
            };
        }

        private List<TaskAttachment> SeedTaskAttachments()
        {
            return new List<TaskAttachment>
            {
                new TaskAttachment
                {
                    Id = 1,
                    FileName = "project-proposal-v2.pdf",
                    FilePath = "/mock-files/project-proposal-v2.pdf",
                    TaskItemId = 1
                },
                new TaskAttachment
                {
                    Id = 2,
                    FileName = "code-review-checklist.docx",
                    FilePath = "/mock-files/code-review-checklist.docx",
                    TaskItemId = 2
                },
                new TaskAttachment
                {
                    Id = 3,
                    FileName = "monitoring-dashboard-sketch.png",
                    FilePath = "/mock-files/monitoring-dashboard-sketch.png",
                    TaskItemId = 5
                }
            };
        }

        private List<TaskHistory> SeedTaskHistories()
        {
            var now = DateTime.Now;
            return new List<TaskHistory>
            {
                new TaskHistory
                {
                    Id = 1,
                    Action = "Task created",
                    ActionDate = now.AddDays(-15),
                    TaskItemId = 1
                },
                new TaskHistory
                {
                    Id = 2,
                    Action = "Priority changed to High",
                    ActionDate = now.AddDays(-8),
                    TaskItemId = 1
                },
                new TaskHistory
                {
                    Id = 3,
                    Action = "Assigned to Marko Horvat",
                    ActionDate = now.AddDays(-5),
                    TaskItemId = 2
                },
                new TaskHistory
                {
                    Id = 4,
                    Action = "Status changed to Completed",
                    ActionDate = now.AddDays(-1),
                    TaskItemId = 3
                },
                new TaskHistory
                {
                    Id = 5,
                    Action = "Description updated",
                    ActionDate = now.AddHours(-12),
                    TaskItemId = 7
                }
            };
        }

        private void WireRelations()
        {
            foreach (var task in _tasks)
            {
                task.User = _users.FirstOrDefault(u => u.Id == task.UserId);
                task.Category = _categories.FirstOrDefault(c => c.Id == task.CategoryId);
                task.Comments = _comments.Where(c => c.TaskItemId == task.Id).ToList();
                task.TaskHistories = _taskHistories.Where(h => h.TaskItemId == task.Id).ToList();

                if (task.User != null)
                {
                    task.User.Tasks.Add(task);
                }

                if (task.Category != null)
                {
                    task.Category.Tasks.Add(task);
                }
            }

            foreach (var comment in _comments)
            {
                comment.TaskItem = _tasks.FirstOrDefault(t => t.Id == comment.TaskItemId);
                comment.User = _users.FirstOrDefault(u => u.Id == comment.UserId);

                if (comment.User != null)
                {
                    comment.User.Comments.Add(comment);
                }
            }

            foreach (var attachment in _attachments)
            {
                attachment.TaskItem = _tasks.FirstOrDefault(t => t.Id == attachment.TaskItemId);
            }

            foreach (var history in _taskHistories)
            {
                history.TaskItem = _tasks.FirstOrDefault(t => t.Id == history.TaskItemId);
            }
        }
    }
}
