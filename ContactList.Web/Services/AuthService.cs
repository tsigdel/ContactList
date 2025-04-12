// Services/AuthService.cs
using ContactList.Web.Models;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace ContactList.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AppDbContext context, ILogger<AuthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public User? Authenticate(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    _logger.LogWarning("Authentication failed due to empty username or password.");
                    return null;
                }

                string hashed = HashPassword(password);
                var user = _context.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == hashed);

                if (user == null)
                {
                    _logger.LogWarning("Authentication failed for username: {Username}", username);
                }
                else
                {
                    _logger.LogInformation("User authenticated successfully: {Username}", username);
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during authentication for username: {Username}", username);
                throw;
            }
        }

        public bool Register(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    _logger.LogWarning("Registration failed due to empty username or password.");
                    return false;
                }

                if (_context.Users.Any(u => u.Username == username))
                {
                    _logger.LogWarning("Registration failed. Username already exists: {Username}", username);
                    return false;
                }

                var user = new User
                {
                    Username = username,
                    PasswordHash = HashPassword(password)
                };

                _context.Users.Add(user);
                _context.SaveChanges();
                _logger.LogInformation("User registered successfully: {Username}", username);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration for username: {Username}", username);
                throw;
            }
        }

        public bool ResetPassword(string username, string newPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(newPassword))
                {
                    _logger.LogWarning("Password reset failed due to empty username or new password.");
                    return false;
                }

                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user == null)
                {
                    _logger.LogWarning("Password reset failed. User not found: {Username}", username);
                    return false;
                }

                user.PasswordHash = HashPassword(newPassword);
                _context.SaveChanges();
                _logger.LogInformation("Password reset successfully for user: {Username}", username);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during password reset for username: {Username}", username);
                throw;
            }
        }

        private string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("Attempt to hash an empty or null password.");
                throw new ArgumentException("Password cannot be null or empty.");
            }

            using var sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
