using TaskManageApp.Models;

namespace TaskManageApp.Repositories
{
    /// <summary>
    /// Repository interface for Task operations.
    /// Allows mock and real implementations to be swapped easily.
    /// </summary>
    public interface ITaskRepository
    {
        Task<List<TaskItem>> GetAllTasksAsync();
        Task<TaskItem> GetTaskByIdAsync(int id);
        Task<List<TaskItem>> GetTasksByCategoryAsync(int categoryId);
        Task<List<TaskItem>> GetTasksByUserAsync(int userId);
        Task<List<TaskItem>> GetCompletedTasksAsync();
        Task<List<TaskItem>> GetPendingTasksAsync();
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<List<Comment>> GetAllCommentsAsync();
        Task<Comment> GetCommentByIdAsync(int id);
        Task<List<Comment>> GetCommentsByTaskIdAsync(int taskItemId);
        Task<List<TaskAttachment>> GetAllTaskAttachmentsAsync();
        Task<TaskAttachment> GetTaskAttachmentByIdAsync(int id);
        Task<List<TaskHistory>> GetAllTaskHistoriesAsync();
        Task<TaskHistory> GetTaskHistoryByIdAsync(int id);
    }
}
