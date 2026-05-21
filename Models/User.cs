using System.ComponentModel.DataAnnotations;

namespace TaskManageApp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(100, ErrorMessage = "The Username must be at most {1} characters long.")]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(200, ErrorMessage = "The Email must be at most {1} characters long.")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password hash is required.")]
        [StringLength(255, ErrorMessage = "The Password Hash must be at most {1} characters long.")]
        public string PasswordHash { get; set; } = string.Empty;
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, ErrorMessage = "The First Name must be at most {1} characters long.")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100, ErrorMessage = "The Last Name must be at most {1} characters long.")]
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Navigation property for related tasks
        public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
        // Navigation property for related comments
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}