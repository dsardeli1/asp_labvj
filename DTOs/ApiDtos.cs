using System.ComponentModel.DataAnnotations;
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

    public sealed record TaskItemWriteDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; init; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; init; } = string.Empty;

        [Range(typeof(DateTime), "2000-01-01", "2100-12-31")]
        public DateTime DueDate { get; init; }

        public bool IsCompleted { get; init; }

        [Range(0, 2)]
        public int PriorityId { get; init; }

        [Range(1, int.MaxValue)]
        public int UserId { get; init; }

        [Range(1, int.MaxValue)]
        public int CategoryId { get; init; }
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

    public sealed record UserWriteDto
    {
        [Required]
        [StringLength(100)]
        public string Username { get; init; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; init; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; init; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FirstName { get; init; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; init; } = string.Empty;
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

    public sealed record CategoryWriteDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; init; } = string.Empty;

        [StringLength(500)]
        public string Description { get; init; } = string.Empty;

        [StringLength(7)]
        [RegularExpression("^#([A-Fa-f0-9]{6})$")]
        public string Color { get; init; } = string.Empty;

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

    public sealed record CommentWriteDto
    {
        [Required]
        [StringLength(2000)]
        public string Content { get; init; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int TaskItemId { get; init; }

        [Range(1, int.MaxValue)]
        public int UserId { get; init; }
    }

    public sealed record TaskAttachmentDto
    {
        public int Id { get; init; }
        public string FileName { get; init; } = string.Empty;
        public string FilePath { get; init; } = string.Empty;
        public int TaskItemId { get; init; }
        public string TaskTitle { get; init; } = string.Empty;
    }

    public sealed record TaskAttachmentWriteDto
    {
        [Required]
        [StringLength(255)]
        public string FileName { get; init; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string FilePath { get; init; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int TaskItemId { get; init; }
    }

    public sealed record TaskHistoryDto
    {
        public int Id { get; init; }
        public string Action { get; init; } = string.Empty;
        public DateTime ActionDate { get; init; }
        public int TaskItemId { get; init; }
        public string TaskTitle { get; init; } = string.Empty;
    }

    public sealed record TaskHistoryWriteDto
    {
        [Required]
        [StringLength(255)]
        public string Action { get; init; } = string.Empty;

        [Range(typeof(DateTime), "2000-01-01", "2100-12-31")]
        public DateTime ActionDate { get; init; }

        [Range(1, int.MaxValue)]
        public int TaskItemId { get; init; }
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

        public static TaskItem ToEntity(this TaskItemWriteDto dto)
        {
            return new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                IsCompleted = dto.IsCompleted,
                PriorityId = dto.PriorityId,
                UserId = dto.UserId,
                CategoryId = dto.CategoryId
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

        public static User ToEntity(this UserWriteDto dto)
        {
            return new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = dto.PasswordHash,
                FirstName = dto.FirstName,
                LastName = dto.LastName
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

        public static Category ToEntity(this CategoryWriteDto dto)
        {
            return new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                Color = dto.Color,
                IsActive = dto.IsActive
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

        public static Comment ToEntity(this CommentWriteDto dto)
        {
            return new Comment
            {
                Content = dto.Content,
                TaskItemId = dto.TaskItemId,
                UserId = dto.UserId
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

        public static TaskAttachment ToEntity(this TaskAttachmentWriteDto dto)
        {
            return new TaskAttachment
            {
                FileName = dto.FileName,
                FilePath = dto.FilePath,
                TaskItemId = dto.TaskItemId
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

        public static TaskHistory ToEntity(this TaskHistoryWriteDto dto)
        {
            return new TaskHistory
            {
                Action = dto.Action,
                ActionDate = dto.ActionDate,
                TaskItemId = dto.TaskItemId
            };
        }
    }
}