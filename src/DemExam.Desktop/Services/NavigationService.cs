using System.Windows;
using DemExam.Desktop.Views;

namespace DemExam.Desktop.Services;

public class NavigationService(IServiceProvider serviceProvider)
    : INavigationService
{
    public void ShowLoginWindow()
    {
        var loginWindow = serviceProvider.GetService(typeof(LoginWindow)) as Window;
        loginWindow?.Show();
    }

    public bool? ShowCaptchaWindow()
    {
        var captchaWindow = serviceProvider.GetService(typeof(CaptchaWindow)) as Window;
        return captchaWindow?.ShowDialog();
    }

    public void ShowMainWindowForRole(int roleId)
    {
        var mainWindow = serviceProvider.GetService(typeof(MainWindow)) as Window;
        mainWindow?.Show();
    }

    public void CloseCurrentWindow(Window window)
    {
        window?.Close();
    }
}