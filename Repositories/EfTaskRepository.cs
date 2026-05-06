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
    }
}
