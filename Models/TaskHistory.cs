using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManageApp.Models
{
    public class TaskHistory
    {
        [Key]
        public int Id { get; set; }
        public string Action { get; set; }
        public DateTime ActionDate { get; set; }

        // Navigation property for TaskItem
        public int TaskItemId { get; set; }
        [ForeignKey(nameof(TaskItemId))]
        public TaskItem TaskItem { get; set; }
    }
}