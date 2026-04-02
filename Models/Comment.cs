namespace TaskManageApp.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsEdited { get; set; }

        // Navigation property for TaskItem
        public int TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; }

        // Navigation properties for User
        public int UserId { get; set; }
        public User User { get; set; }
        
    }
}