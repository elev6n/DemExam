using DemExam.Desktop.ViewModels;

namespace DemExam.Desktop.Services;

public interface INavigationService
{
    IViewModel CurrentViewModel { get; }

    Task NavigateToAsync<TViewModel>() where TViewModel : IViewModel;
    Task NavigateToAsync<TViewModel>(object? parameter) where TViewModel : IViewModel;
}