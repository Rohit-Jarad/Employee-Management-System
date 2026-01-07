using System.Security.Cryptography;
using System.Text;
using Employee_Management_System.Models.Entities;
using Employee_Management_System.Repositories.Interfaces;
using Employee_Management_System.Services.Interfaces;

namespace Employee_Management_System.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    return null;
                }

                // Verify password
                if (VerifyPassword(password, user.PasswordHash))
                {
                    // Update last login time
                    user.LastLoginAt = DateTime.Now;
                    await _userRepository.UpdateAsync(user);
                    return user;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating user with email {Email}", email);
                throw;
            }
        }

        public async Task<User> RegisterAsync(string firstName, string lastName, string email, string password)
        {
            try
            {
                // Check if user already exists
                if (await _userRepository.GetByEmailAsync(email) != null)
                {
                    throw new InvalidOperationException("A user with this email already exists.");
                }

                var passwordHash = HashPassword(password);
                var user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PasswordHash = passwordHash,
                    CreatedAt = DateTime.Now
                };

                return await _userRepository.CreateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user with email {Email}", email);
                throw;
            }
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null;
        }

        // Password hashing using SHA256 (simple approach)
        // For production, consider using bcrypt or ASP.NET Core Identity's PasswordHasher
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private static bool VerifyPassword(string password, string passwordHash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == passwordHash;
        }
    }
}
