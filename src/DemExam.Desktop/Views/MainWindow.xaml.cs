using System.Windows;
using DemExam.Desktop.Services;
using DemExam.Desktop.ViewModels;

namespace DemExam.Desktop.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly INavigationService _navigationService;

    public MainWindow(INavigationService navigationService)
    {
        InitializeComponent();
        _navigationService = navigationService;
        DataContext = navigationService;

        Loaded += OnWindowLoaded;
    }

    private async void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        await _navigationService.NavigateToAsync<AuthorizationViewModel>();
    }
}