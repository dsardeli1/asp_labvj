using TaskManageApp.Models;

namespace TaskManageApp.DTOs
{
    public sealed record TaskItemDto
    {
        public int Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public DateTime CreatedDate { get; init; }
        public DateTime DueDate { get; init; }
        public bool IsCompleted { get; init; }
        public int PriorityId { get; init; }
        public string PriorityName { get; init; } = string.Empty;
        public int UserId { get; init; }
        public string UserName { get; init; } = string.Empty;
        public int CategoryId { get; init; }
        public string CategoryName { get; init; } = string.Empty;
    }

    public sealed record UserDto
    {
        public int Id { get; init; }
        public string Username { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
    }

    public sealed record CategoryDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string Color { get; init; } = string.Empty;
        public DateTime CreatedDate { get; init; }
        public bool IsActive { get; init; }
    }

    public sealed record CommentDto
    {
        public int Id { get; init; }
        public string Content { get; init; } = string.Empty;
        public DateTime CreatedDate { get; init; }
        public bool IsEdited { get; init; }
        public int TaskItemId { get; init; }
        public string TaskTitle { get; init; } = string.Empty;
        public int UserId { get; init; }
        public string UserName { get; init; } = string.Empty;
    }

    public sealed record TaskAttachmentDto
    {
        public int Id { get; init; }
        public string FileName { get; init; } = string.Empty;
        public string FilePath { get; init; } = string.Empty;
        public int TaskItemId { get; init; }
        public string TaskTitle { get; init; } = string.Empty;
    }

    public sealed record TaskHistoryDto
    {
        public int Id { get; init; }
        public string Action { get; init; } = string.Empty;
        public DateTime ActionDate { get; init; }
        public int TaskItemId { get; init; }
        public string TaskTitle { get; init; } = string.Empty;
    }

    public static class ApiDtoExtensions
    {
        public static TaskItemDto ToDTO(this TaskItem task)
        {
            return new TaskItemDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                CreatedDate = task.CreatedDate,
                DueDate = task.DueDate,
                IsCompleted = task.IsCompleted,
                PriorityId = task.PriorityId,
                PriorityName = Enum.GetName(typeof(Priority), task.PriorityId) ?? task.PriorityId.ToString(),
                UserId = task.UserId,
                UserName = task.User?.Username ?? string.Empty,
                CategoryId = task.CategoryId,
                CategoryName = task.Category?.Name ?? string.Empty
            };
        }

        public static UserDto ToDTO(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedAt = user.CreatedAt
            };
        }

        public static CategoryDto ToDTO(this Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Color = category.Color,
                CreatedDate = category.CreatedDate,
                IsActive = category.IsActive
            };
        }

        public static CommentDto ToDTO(this Comment comment)
        {
            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedDate = comment.CreatedDate,
                IsEdited = comment.IsEdited,
                TaskItemId = comment.TaskItemId,
                TaskTitle = comment.TaskItem?.Title ?? string.Empty,
                UserId = comment.UserId,
                UserName = comment.User?.Username ?? string.Empty
            };
        }

        public static TaskAttachmentDto ToDTO(this TaskAttachment taskAttachment)
        {
            return new TaskAttachmentDto
            {
                Id = taskAttachment.Id,
                FileName = taskAttachment.FileName,
                FilePath = taskAttachment.FilePath,
                TaskItemId = taskAttachment.TaskItemId,
                TaskTitle = taskAttachment.TaskItem?.Title ?? string.Empty
            };
        }

        public static TaskHistoryDto ToDTO(this TaskHistory taskHistory)
        {
            return new TaskHistoryDto
            {
                Id = taskHistory.Id,
                Action = taskHistory.Action,
                ActionDate = taskHistory.ActionDate,
                TaskItemId = taskHistory.TaskItemId,
                TaskTitle = taskHistory.TaskItem?.Title ?? string.Empty
            };
        }
    }
}