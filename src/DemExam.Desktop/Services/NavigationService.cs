using CommunityToolkit.Mvvm.ComponentModel;
using DemExam.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DemExam.Desktop.Services;

public partial class NavigationService(IServiceProvider serviceProvider) : ObservableObject, INavigationService
{
    [ObservableProperty] private IViewModel? _currentViewModel;

    public async Task NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        var viewModel = serviceProvider.GetRequiredService<TViewModel>();

        CurrentViewModel = viewModel;

        if (viewModel is IViewModel activatable)
            await activatable.OnActivatedAsync();
    }
}