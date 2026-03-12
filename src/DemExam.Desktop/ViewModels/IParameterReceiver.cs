namespace DemExam.Desktop.ViewModels;

public interface IParameterReceiver
{
    Task OnNavigatedToAsync(object? parameter);
}