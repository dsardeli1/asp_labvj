using System.ComponentModel.DataAnnotations;

namespace TaskManageApp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property for related tasks
        public virtual ICollection<TaskItem> Tasks { get; set; }
        // Navigation property for related comments
        public virtual ICollection<Comment> Comments { get; set; }
    }
}