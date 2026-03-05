using DemExam.Desktop.Models;

namespace DemExam.Desktop.Services;

public class CurrentUserService : ICurrentUserService
{
    public User? CurrentUser { get; private set; }

    public bool IsAuthenticated => CurrentUser != null;

    public void SetCurrentUser(User user)
    {
        CurrentUser = user;
    }

    public void Logout()
    {
        CurrentUser = null;
    }
}