using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManageApp.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Content is required.")]
        [StringLength(2000, ErrorMessage = "The Content must be at most {1} characters long.")]
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsEdited { get; set; }

        // Navigation property for TaskItem
        [Range(1, int.MaxValue, ErrorMessage = "Task is required.")]
        public int TaskItemId { get; set; }
        [ForeignKey(nameof(TaskItemId))]
        public TaskItem? TaskItem { get; set; }

        // Navigation properties for User
        [Range(1, int.MaxValue, ErrorMessage = "User is required.")]
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
        
    }
}