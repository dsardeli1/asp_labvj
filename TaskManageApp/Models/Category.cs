namespace TaskManageApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation property for related tasks
        public ICollection<TaskItem> Tasks { get; set; }
    }
}