using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using DemExam.Desktop.Data;
using DemExam.Desktop.Services;

namespace DemExam.Desktop.ViewModels;

public partial class ViewModelBase(AppDbContext context, INavigationService navigationService)
    : ObservableObject, IViewModel
{
    protected readonly AppDbContext Context = context;
    protected readonly INavigationService NavigationService = navigationService;

    [ObservableProperty] private string _title = "";

    public string? ErrorMessage
    {
        get;
        set
        {
            SetProperty(ref field, value);
            HasError = !string.IsNullOrEmpty(value);
        }
    }

    public bool HasError
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public Task OnActivatedAsync() => Task.CompletedTask;
}