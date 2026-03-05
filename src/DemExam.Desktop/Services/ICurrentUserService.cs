using DemExam.Desktop.Models;

namespace DemExam.Desktop.Services;

public interface ICurrentUserService
{
    User? CurrentUser { get; }
    bool IsAuthenticated { get; }

    void SetCurrentUser(User user);
    void Logout();
}