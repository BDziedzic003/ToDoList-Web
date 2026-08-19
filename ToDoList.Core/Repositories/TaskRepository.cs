using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Repositories
{
    public class TaskRepository
    {
        private TodoContext db;
        public TaskRepository(TodoContext db)
        {
            this.db = db;
        }

        public List<TaskItem> GetAll(int userId)
        {
            return db.Tasks.Where(t => t.UserId == userId).OrderBy(t => t.Done).ThenByDescending(t => t.Priority).ToList();
        }

        public int Add(string text, Priority priority, int userId)
        {
            TaskItem task = new TaskItem(text, priority, userId);
            db.Tasks.Add(task);
            db.SaveChanges();
            return task.Id;
        }
        public void Remove(int id, int userId)
        {
            TaskItem task = db.Tasks.FirstOrDefault(t => t.Id == id && t.UserId == userId);
            if (task == null) return;
            db.Tasks.Remove(task);
            db.SaveChanges();
        }

        public void UpdateText(int id, string text, int userId) 
        {
            TaskItem task = db.Tasks.FirstOrDefault(t => t.Id == id && t.UserId == userId);
            if (task == null) return;
            task.Text = text;
            db.SaveChanges();
        }

        public void UpdatePriority(int id, Priority priority, int userId)
        {
            TaskItem task = db.Tasks.FirstOrDefault(t => t.Id == id && t.UserId == userId);
            if (task == null) return;
            task.Priority = priority;
            db.SaveChanges();
        }

        public void UpdateDone(int id, bool done, int userId)
        {
            TaskItem task = db.Tasks.FirstOrDefault(t => t.Id == id && t.UserId == userId);
            if (task == null) return;
            task.Done = done;
            db.SaveChanges();
        }

        public List<TaskItem> FindTasksByKeyword(string keyword, int userId)
        {
            return db.Tasks.Where(t => t.UserId == userId && t.Text.Contains(keyword)).ToList();
        }
    }
}
