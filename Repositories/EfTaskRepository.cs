using Microsoft.EntityFrameworkCore;
using TaskManageApp.DAL;
using TaskManageApp.Models;

namespace TaskManageApp.Repositories
{
    /// <summary>
    /// Entity Framework-backed implementation of the Task repository.
    /// Uses ApplicationDbContext to query and persist data.
    /// </summary>
    public class EFTaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public EFTaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskItem>> GetAllTasksAsync()
        {
            return await _context.Tasks
                .Include(t => t.User)
                .Include(t => t.Category)
                .Include(t => t.Comments)
                .Include(t => t.TaskHistories)
                .ToListAsync();
        }

        public async Task<TaskItem> GetTaskByIdAsync(int id)
        {
            return await _context.Tasks
                .Include(t => t.User)
                .Include(t => t.Category)
                .Include(t => t.Comments)
                .Include(t => t.TaskHistories)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<TaskItem>> GetTasksByCategoryAsync(int categoryId)
        {
            return await _context.Tasks
                .Where(t => t.CategoryId == categoryId)
                .Include(t => t.User)
                .Include(t => t.Category)
                .Include(t => t.Comments)
                .ToListAsync();
        }

        public async Task<List<TaskItem>> GetTasksByUserAsync(int userId)
        {
            return await _context.Tasks
                .Where(t => t.UserId == userId)
                .Include(t => t.User)
                .Include(t => t.Category)
                .Include(t => t.Comments)
                .ToListAsync();
        }

        public async Task<List<TaskItem>> GetCompletedTasksAsync()
        {
            return await _context.Tasks
                .Where(t => t.IsCompleted)
                .Include(t => t.User)
                .Include(t => t.Category)
                .Include(t => t.Comments)
                .ToListAsync();
        }

        public async Task<List<TaskItem>> GetPendingTasksAsync()
        {
            return await _context.Tasks
                .Where(t => !t.IsCompleted)
                .Include(t => t.User)
                .Include(t => t.Category)
                .Include(t => t.Comments)
                .ToListAsync();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Tasks)
                .Include(u => u.Comments)
                .ToListAsync();
        }

        public async Task<User> AddUserAsync(User user)
        {
            user.CreatedAt = user.CreatedAt == default ? DateTime.UtcNow : user.CreatedAt;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var existing = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            if (existing == null) return false;

            existing.Username = user.Username;
            existing.Email = user.Email;
            existing.FirstName = user.FirstName;
            existing.LastName = user.LastName;
            existing.PasswordHash = user.PasswordHash;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Tasks)
                .Include(u => u.Comments)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null || (user.Tasks != null && user.Tasks.Any()))
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<User>> GetUsersWithTasksAsync()
        {
            return await _context.Users
                .Where(u => u.Tasks.Any())
                .Include(u => u.Tasks)
                .Include(u => u.Comments)
                .ToListAsync();
        }

        public async Task<List<User>> GetUsersWithoutTasksAsync()
        {
            return await _context.Users
                .Where(u => !u.Tasks.Any())
                .Include(u => u.Tasks)
                .Include(u => u.Comments)
                .ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Tasks)
                .Include(u => u.Comments)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Include(c => c.Tasks)
                .ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Tasks)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> IsCategoryNameUniqueAsync(string name, int? excludingId = null)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            return !await _context.Categories
                .AnyAsync(c => c.Name == name && (!excludingId.HasValue || c.Id != excludingId.Value));
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == category.Id);
            if (existingCategory == null)
            {
                return false;
            }

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.Color = category.Color;
            existingCategory.IsActive = category.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Tasks)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null || (category.Tasks != null && category.Tasks.Any()))
            {
                return false;
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Comment>> GetAllCommentsAsync()
        {
            return await _context.Comments
                .Include(c => c.TaskItem)
                .Include(c => c.User)
                .ToListAsync();
        }

        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            return await _context.Comments
                .Include(c => c.TaskItem)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Comment>> GetCommentsByTaskIdAsync(int taskItemId)
        {
            return await _context.Comments
                .Where(c => c.TaskItemId == taskItemId)
                .Include(c => c.User)
                .ToListAsync();
        }

        public async Task<Comment> AddCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<bool> UpdateCommentAsync(Comment comment)
        {
            var existingComment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == comment.Id);
            if (existingComment == null)
            {
                return false;
            }

            existingComment.Content = comment.Content;
            existingComment.IsEdited = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCommentAsync(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null)
            {
                return false;
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<TaskAttachment>> GetAllTaskAttachmentsAsync()
        {
            return await _context.TaskAttachments
                .Include(a => a.TaskItem)
                .ToListAsync();
        }

        public async Task<TaskAttachment> GetTaskAttachmentByIdAsync(int id)
        {
            return await _context.TaskAttachments
                .Include(a => a.TaskItem)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<TaskAttachment> AddTaskAttachmentAsync(TaskAttachment taskAttachment)
        {
            _context.TaskAttachments.Add(taskAttachment);
            await _context.SaveChangesAsync();
            return taskAttachment;
        }

        public async Task<bool> UpdateTaskAttachmentAsync(TaskAttachment taskAttachment)
        {
            var existingAttachment = await _context.TaskAttachments.FirstOrDefaultAsync(a => a.Id == taskAttachment.Id);
            if (existingAttachment == null)
            {
                return false;
            }

            existingAttachment.FileName = taskAttachment.FileName;
            existingAttachment.FilePath = taskAttachment.FilePath;
            existingAttachment.TaskItemId = taskAttachment.TaskItemId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTaskAttachmentAsync(int id)
        {
            var attachment = await _context.TaskAttachments.FirstOrDefaultAsync(a => a.Id == id);
            if (attachment == null)
            {
                return false;
            }

            _context.TaskAttachments.Remove(attachment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<TaskHistory>> GetAllTaskHistoriesAsync()
        {
            return await _context.TaskHistories
                .Include(h => h.TaskItem)
                .ToListAsync();
        }

        public async Task<TaskHistory> GetTaskHistoryByIdAsync(int id)
        {
            return await _context.TaskHistories
                .Include(h => h.TaskItem)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<TaskHistory> AddTaskHistoryAsync(TaskHistory taskHistory)
        {
            _context.TaskHistories.Add(taskHistory);
            await _context.SaveChangesAsync();
            return taskHistory;
        }

        public async Task<bool> UpdateTaskHistoryAsync(TaskHistory taskHistory)
        {
            var existingHistory = await _context.TaskHistories.FirstOrDefaultAsync(h => h.Id == taskHistory.Id);
            if (existingHistory == null)
            {
                return false;
            }

            existingHistory.Action = taskHistory.Action;
            existingHistory.ActionDate = taskHistory.ActionDate;
            existingHistory.TaskItemId = taskHistory.TaskItemId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTaskHistoryAsync(int id)
        {
            var history = await _context.TaskHistories.FirstOrDefaultAsync(h => h.Id == id);
            if (history == null)
            {
                return false;
            }

            _context.TaskHistories.Remove(history);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TaskItem> AddTaskAsync(TaskItem task)
        {
            task.CreatedDate = task.CreatedDate == default ? DateTime.Now : task.CreatedDate;
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> UpdateTaskAsync(TaskItem task)
        {
            var existing = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id);
            if (existing == null)
            {
                return false;
            }

            existing.Title = task.Title;
            existing.Description = task.Description;
            existing.DueDate = task.DueDate;
            existing.IsCompleted = task.IsCompleted;
            existing.PriorityId = task.PriorityId;
            existing.UserId = task.UserId;
            existing.CategoryId = task.CategoryId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task == null)
            {
                return false;
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
