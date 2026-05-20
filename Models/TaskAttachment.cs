using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManageApp.Models
{
    public class TaskAttachment
    {
        [Key]
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        
        // Navigation property for TaskItem
        public int TaskItemId { get; set; }
        [ForeignKey(nameof(TaskItemId))]
        public TaskItem? TaskItem { get; set; }
    }
}