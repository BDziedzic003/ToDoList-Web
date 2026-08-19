namespace ToDoList.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool Done { get; set; }
        public Priority Priority { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public TaskItem() { }
        public TaskItem(string text, Priority priority, int userId)
        {
            Text = text;
            Done = false;
            Priority = priority;
            UserId = userId;
        }

        public void ToggleDone()
        {
            Done = !Done;
        }
    }
}
