using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManageApp.Models
{
    public class TaskHistory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Action { get; set; } = string.Empty;
        public DateTime ActionDate { get; set; }

        // Navigation property for TaskItem
        [Range(1, int.MaxValue, ErrorMessage = "Task is required.")]
        public int TaskItemId { get; set; }
        [ForeignKey(nameof(TaskItemId))]
        public TaskItem? TaskItem { get; set; }
    }
}