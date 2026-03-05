using System.Windows;
using System.Windows.Controls;
using DemExam.Desktop.ViewModels;
using DemExam.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;

namespace DemExam.Desktop.Services;

public class NavigationService(IServiceProvider serviceProvider) : INavigationService
{
    private readonly IServiceProvider _serviceProvider =
        serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    private readonly Dictionary<Type, Type> _viewModelToViewMap = new();
    private readonly Stack<ViewModelBase> _navigationStack = new();
    private Frame _frame = null!;

    public bool CanGoBack => _navigationStack.Count > 1;

    public ViewModelBase? CurrentViewModel => _navigationStack.Count > 0 ? _navigationStack.Peek() : null;

    public void SetFrame(Frame frame)
    {
        _frame = frame;
    }

    public void RegisterView<TView, TViewModel>() where TViewModel : ViewModelBase
    {
        _viewModelToViewMap[typeof(TViewModel)] = typeof(TView);
    }

    public void GoBack()
    {
        if (_navigationStack.Count <= 1) return;

        _navigationStack.Pop();
        _frame.GoBack();
    }

    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        NavigateTo<TViewModel>(null);
    }

    public void NavigateTo<TViewModel>(object? parameter) where TViewModel : ViewModelBase
    {
        var viewModelType = typeof(TViewModel);

        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();

        if (!_viewModelToViewMap.TryGetValue(viewModelType, out var viewType))
        {
            throw new InvalidOperationException($"View для {viewModelType.Name} не зарегистрирована");
        }

        var view = (FrameworkElement)_serviceProvider.GetRequiredService(viewType);
        view.DataContext = viewModel;

        _navigationStack.Push(viewModel);
        _frame.Navigate(view);
    }
}