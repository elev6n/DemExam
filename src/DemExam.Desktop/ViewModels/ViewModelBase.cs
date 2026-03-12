using CommunityToolkit.Mvvm.ComponentModel;

namespace DemExam.Desktop.ViewModels;

public abstract partial class ViewModelBase
    : ObservableObject, IViewModel
{
    [ObservableProperty] private string _title = "";

    public string? ErrorMessage
    {
        get;
        protected set => SetProperty(ref field, value);
    }

    public virtual Task OnActivatedAsync() => Task.CompletedTask;
}