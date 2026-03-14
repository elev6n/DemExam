using DemExam.Desktop.Views;

namespace DemExam.Desktop.Services;

public class CaptchaService : ICaptchaService
{
    public bool ShowCaptcha()
    {
        var window = new CaptchaWindow();
        return window.ShowDialog() == true && window.IsCaptchaPassed;
    }
}