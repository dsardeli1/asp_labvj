using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManageApp.Models
{
    public class User : IdentityUser<int>
    {
        [NotMapped]
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(100, ErrorMessage = "The Username must be at most {1} characters long.")]
        public string Username
        {
            get => UserName ?? string.Empty;
            set => UserName = value;
        }

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