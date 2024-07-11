using ManageRestaurant.Helper;
using ManageRestaurant.Interface;
using ManageRestaurant.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ManageRestaurant.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ManageRestaurantContext _context;
        private readonly List<User> users = new List<User>();
       // private readonly RoleManager<IdentityRole> roleManager;
        public UsersRepository(IConfiguration configuration, ManageRestaurantContext context)
        {
            _configuration = configuration;
            _context = context;
           // this.roleManager = roleManager;
        }

        public async Task<bool> RegisterUser(string userName, string email, string password)
        {
            if (!IsValidEmail(email))
                throw new ArgumentException("Invalid email format");

            if (users.Any(u => u.Email == email))
                return false;

            string hashedPassword = HashPassword(password);

            User newUser = new User
            {
                UserName = userName,
                Email = email,
                Password = hashedPassword,
                CreatedBy = "System",
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log the inner exception for more details using Console.WriteLine for simplicity
                Console.WriteLine($"Error: {ex.InnerException?.Message}");
                return false;
            }

            return true;
        }

        public async Task<User> LoginUser(string email, string password)
        {
            string hashedPassword = HashPassword(password);
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == hashedPassword);
          
        }


        private bool IsValidEmail(string email)
        {
            // Basic email validation
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private string HashPassword(string password)
        {
            // Basic password hashing using SHA256
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
