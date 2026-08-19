namespace ToDoList.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }

        public List<TaskItem> Tasks { get; set; } = new();
    }
}
