namespace TaskManageApp.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }

        // Navigation properties
        public int PriorityId { get; set; } // Enum...? use (int)

        public ICollection<Comment> Comments { get; set; }

        // Navigation properties for User
        public int UserId { get; set; }
        public User User { get; set; }

        // Navigation properties for Category
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        // Navigation property for TaskHistory
        public ICollection<TaskHistory> TaskHistories { get; set; }
    }
}