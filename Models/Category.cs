using System.ComponentModel.DataAnnotations;

namespace TaskManageApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation property for related tasks
        public virtual ICollection<TaskItem> Tasks { get; set; }
    }
}