using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManageApp.Models
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }

        // Navigation properties
        public int PriorityId { get; set; } // Enum...? use (int)

        public virtual ICollection<Comment> Comments { get; set; }

        // Navigation properties for User
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        // Navigation properties for Category
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        // Navigation property for TaskHistory
        public virtual ICollection<TaskHistory> TaskHistories { get; set; }
    }
}