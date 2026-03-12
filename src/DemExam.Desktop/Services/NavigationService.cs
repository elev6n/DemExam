using CommunityToolkit.Mvvm.ComponentModel;
using DemExam.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DemExam.Desktop.Services;

public partial class NavigationService(IServiceProvider serviceProvider) : ObservableObject, INavigationService
{
    [ObservableProperty] private IViewModel? _currentViewModel;

    public async Task NavigateToAsync<TViewModel>() where TViewModel : IViewModel
    {
        await NavigateToAsync<TViewModel>(null);
    }

    public async Task NavigateToAsync<TViewModel>(object? parameter) where TViewModel : IViewModel
    {
        var viewModel = serviceProvider.GetRequiredService<TViewModel>();

        CurrentViewModel = viewModel;

        if (viewModel is IParameterReceiver parameterReceiver)
            await parameterReceiver.OnNavigatedToAsync(parameter);
        if (viewModel is IViewModel activatable)
            await activatable.OnActivatedAsync();
    }
}