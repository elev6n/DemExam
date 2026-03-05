using System.Windows;

namespace DemExam.Desktop.Services;

public interface INavigationService
{
    void ShowLoginWindow();

    bool? ShowCaptchaWindow();

    void ShowMainWindowForRole(int roleId);

    void CloseCurrentWindow(Window window);
}