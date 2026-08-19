using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using ToDoList.Models;

namespace ToDoList.Data
{
    public class TodoContext : DbContext
    {
        private string connectionString = "Server=localhost;Database=ToDoList;Trusted_Connection=True;TrustServerCertificate=True;";
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
                options.UseSqlServer(connectionString);
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
