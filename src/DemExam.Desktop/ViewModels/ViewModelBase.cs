using CommunityToolkit.Mvvm.ComponentModel;
using DemExam.Desktop.Data;
using DemExam.Desktop.Services;

namespace DemExam.Desktop.ViewModels;

public abstract partial class ViewModelBase
    : ObservableObject, IViewModel
{
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

    public virtual Task OnActivatedAsync() => Task.CompletedTask;
}