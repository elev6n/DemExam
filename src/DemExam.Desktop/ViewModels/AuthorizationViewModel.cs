using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DemExam.Desktop.Data;
using DemExam.Desktop.Services;
using DemExam.Desktop.Store;
using Microsoft.EntityFrameworkCore;

namespace DemExam.Desktop.ViewModels;

public partial class AuthorizationViewModel(AppDbContext context, INavigationService navigationService) : ViewModelBase
{
    [ObservableProperty] private string _login = "admin";

    [ObservableProperty] private string _errorMessage = "";

    [RelayCommand]
    private async Task LoginAsync(object parameter)
    {
        var passwordBox = parameter as PasswordBox;
        var password = passwordBox?.Password ?? string.Empty;
        
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(password))
        {
            ErrorMessage = "Заполните все поля";
            return;
        }

        var user = await context.Users
            .Include(u => u.UserRoleNavigation)
            .Include(u => u.UserStatusNavigation)
            .FirstOrDefaultAsync(u => u.Login == Login && u.Password == password);

        if (user == null)
        {
            await HandleFailedAttempt("Неверный логин или пароль");
            return;
        }

        if (user.UserStatus == 2)
        {
            ErrorMessage = "Вы заблокированы. Обратитесь к администратору";
            return;
        }

        Session.FailedAttempts = 0;
        Session.CurrentUser = user;

        if (user.UserRole == 1)
            await navigationService.NavigateToAsync<AdminViewModel>();
    }

    private async Task HandleFailedAttempt(string message)
    {
        Session.FailedAttempts++;
        if (Session.FailedAttempts >= 3)
        {
            if (!string.IsNullOrWhiteSpace(Login))
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Login == Login);
                if (user != null)
                {
                    user.UserStatus = 2;
                    await context.SaveChangesAsync();
                }
            }

            ErrorMessage = "Превышено число попыток. Учетная запись заблокирована.";

            Session.FailedAttempts = 0;
        }
        else
        {
            ErrorMessage = message;
        }
    }
}