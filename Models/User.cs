namespace TaskManageApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property for related tasks
        public ICollection<TaskItem> Tasks { get; set; }
        // Navigation property for related comments
        public ICollection<Comment> Comments { get; set; }
    }
}