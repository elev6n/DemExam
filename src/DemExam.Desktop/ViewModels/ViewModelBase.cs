using CommunityToolkit.Mvvm.ComponentModel;
using DemExam.Desktop.Data;
using DemExam.Desktop.Services;

namespace DemExam.Desktop.ViewModels;

public abstract partial class ViewModelBase(AppDbContext context, INavigationService navigationService)
    : ObservableObject, IViewModel
{
    protected readonly AppDbContext Context = context;
    protected readonly INavigationService NavigationService = navigationService;

    [ObservableProperty] private string _title = "";

    public string? ErrorMessage
    {
        get;
        protected set => SetProperty(ref field, value);
    }

    public virtual Task OnActivatedAsync()
    {
        return Task.CompletedTask;
    }
}