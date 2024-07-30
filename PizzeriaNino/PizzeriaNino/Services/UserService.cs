using PizzeriaNino.Data;
using PizzeriaNino.Models;
using Microsoft.EntityFrameworkCore;

namespace PizzeriaNino.Services
{
    public class UserService
    {
        private readonly PizzeriaContext _context;

        public UserService(PizzeriaContext context)
        {
            _context = context;
        }

        public async Task<User> RegisterUserAsync(string username, string email, string password, string role)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                UserName = username,
                Email = email,
                PasswordHash = passwordHash,
                Role = role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> AuthenticateUserAsync(string email, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return user;
        }
    }
}
