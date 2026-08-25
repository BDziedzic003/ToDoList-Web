using Microsoft.EntityFrameworkCore;
using ToDoList.Models;

namespace ToDoList.Data
{
    public class TodoContext : DbContext
    {
        private string connectionString = "Host=localhost;Port=5432;Database=ToDoList;Username=postgres;Password=haslo;";
        public TodoContext() { }
        public TodoContext(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public TodoContext(DbContextOptions<TodoContext> options) : base(options) { }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseNpgsql(connectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}
