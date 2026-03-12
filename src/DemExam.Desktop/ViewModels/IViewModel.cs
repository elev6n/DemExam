namespace DemExam.Desktop.ViewModels;

public interface IViewModel
{
    Task OnActivatedAsync() => Task.CompletedTask;
}