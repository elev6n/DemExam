using System.Windows;
using System.Windows.Controls;
using DemExam.Desktop.ViewModels;

namespace DemExam.Desktop.Services;

public interface INavigationService
{
    void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
    void NavigateTo<TViewModel>(object? parameter) where TViewModel : ViewModelBase;
    void GoBack();
    void SetFrame(Frame frame);
    void RegisterView<TView, TViewModel>() where TViewModel : ViewModelBase;

    bool CanGoBack { get; }
    ViewModelBase? CurrentViewModel { get; }
}