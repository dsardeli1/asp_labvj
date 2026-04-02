namespace TaskManageApp.Models
{
    public class TaskHistory
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public DateTime ActionDate { get; set; }

        // Navigation property for TaskItem
        public int TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; }
    }
}