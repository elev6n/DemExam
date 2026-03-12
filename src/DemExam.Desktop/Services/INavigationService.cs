using DemExam.Desktop.ViewModels;

namespace DemExam.Desktop.Services;

public interface INavigationService
{
    IViewModel CurrentViewModel { get; }

    Task NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
}