namespace TaskManageApp.Models
{
    public class TaskAttachment
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        
        // Navigation property for TaskItem
        public int TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; }
    }
}