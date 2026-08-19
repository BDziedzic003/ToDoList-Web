using ToDoList.Data;
using ToDoList.Models;
using ToDoList.Security;

namespace ToDoList.Repositories
{
    public class UserRepository
    {
        private TodoContext db;

        public UserRepository(TodoContext db)
        {
            this.db = db;
        }

        public User Register(string username, string password)
        {
            if (db.Users.Any(u => u.Username == username))
            {
                return null;
            }
            var (hash, salt) = PasswordHasher.Hash(password);
            var user = new User { Username = username, PasswordHash = hash, PasswordSalt = salt };
            db.Users.Add(user);
            db.SaveChanges();
            return user;
        }

        public User Login(string username, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return null;

            return PasswordHasher.Verify(password, user.PasswordHash, user.PasswordSalt) ? user : null;
        }
    }
}
