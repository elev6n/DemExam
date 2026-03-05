using DemExam.Desktop.Data;
using DemExam.Desktop.Models;
using Microsoft.EntityFrameworkCore;

namespace DemExam.Desktop.Services;

public class AuthenticationService(AppDbContext context) : IAuthenticationService
{
    public async Task<User?> AuthenticateAsync(string login, string password)
    {
        return await context.Users
            .Include(u => u.UserRoleNavigation)
            .Include(u => u.UserStatusNavigation)
            .FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
    }

    public async Task<bool> UpdateUserStatusAsync(int userId, int newStatus)
    {
        try
        {
            var user = await context.Users.FindAsync(userId) ?? throw new Exception();

            user.UserStatus = newStatus;
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await context.Users
            .Include(u => u.UserRoleNavigation)
            .Include(u => u.UserStatusNavigation)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
}