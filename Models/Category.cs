using System.ComponentModel.DataAnnotations;

namespace TaskManageApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "The Name must be at most {1} characters long.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "The Description must be at most {1} characters long.")]
        public string Description { get; set; } = string.Empty;

        [StringLength(7, ErrorMessage = "Color must be a hex code like #3b82f6.")]
        [RegularExpression("^#([A-Fa-f0-9]{6})$", ErrorMessage = "Color must be a hex code like #3b82f6.")]
        public string Color { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation property for related tasks
        public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}