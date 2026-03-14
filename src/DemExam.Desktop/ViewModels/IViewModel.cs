namespace DemExam.Desktop.ViewModels;

public interface IViewModel
{
    string? ErrorMessage { get; }

    string Title { get; }

    Task OnActivatedAsync()
    {
        return Task.CompletedTask;
    }
}