using System.Windows;
using DemExam.Desktop.Data;
using DemExam.Desktop.Exceptions;
using DemExam.Desktop.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DemExam.Desktop.Views;

public partial class LoginWindow : Window
{
    private readonly AppDbContext _context;
    private readonly IServiceProvider _serviceProvider;

    public LoginWindow(AppDbContext context, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _context = context;
        _serviceProvider = serviceProvider;
    }


    private async void LoginButton_OnClick(object sender, RoutedEventArgs e)
    {
        var login = LoginBox.Text;
        var password = PasswordBox.Password;

        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            ErrorText.Text = "Заполните все поля!";
            return;
        }

        try
        {
            var user = await _context.Users
                .Include(u => u.UserRoleNavigation)
                .Include(u => u.UserStatusNavigation)
                .FirstOrDefaultAsync(u => u.Login == login && u.Password == password) ?? throw new NotFoundException();

            Session.UserRole = user.UserRole;

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            Close();
        }
        catch (NotFoundException)
        {
            ErrorText.Text = "Неверный логин или пароль!";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}