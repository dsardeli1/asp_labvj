using System.ComponentModel.DataAnnotations;

namespace TaskManageApp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Navigation property for related tasks
        public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
        // Navigation property for related comments
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}