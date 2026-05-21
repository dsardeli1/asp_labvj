using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManageApp.Models
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "The Title must be at most {1} characters long.")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(2000, ErrorMessage = "The Description must be at most {1} characters long.")]
        public string Description { get; set; } = string.Empty;
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        [Required(ErrorMessage = "Due date is required.")]
        [Display(Name = "Due Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "2000-01-01", "2100-12-31", ErrorMessage = "Enter a valid due date.")]
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }

        // Navigation properties
        [Range(0, 2, ErrorMessage = "Priority is required.")]
        public int PriorityId { get; set; } // Enum...? use (int)

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        // Navigation properties for User
        [Range(1, int.MaxValue, ErrorMessage = "Assigned user is required.")]
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        // Navigation properties for Category
        [Range(1, int.MaxValue, ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }

        // Navigation property for TaskHistory
        public virtual ICollection<TaskHistory> TaskHistories { get; set; } = new List<TaskHistory>();
    }
}