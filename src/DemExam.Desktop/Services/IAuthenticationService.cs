using DemExam.Desktop.Models;

namespace DemExam.Desktop.Services;

public interface IAuthenticationService
{
    Task<User?> AuthenticateAsync(string login, string password);
    Task<bool> UpdateUserStatusAsync(int userId, int newStatus);
    Task<User?> GetUserByIdAsync(int userId);
}