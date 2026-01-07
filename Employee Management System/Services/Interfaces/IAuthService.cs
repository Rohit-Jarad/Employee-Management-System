using Employee_Management_System.Models.Entities;

namespace Employee_Management_System.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task<User> RegisterAsync(string firstName, string lastName, string email, string password);
        Task<bool> UserExistsAsync(string email);
    }
}
