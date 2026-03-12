using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DemExam.Desktop.Data;
using DemExam.Desktop.Models;
using DemExam.Desktop.Services;
using Microsoft.EntityFrameworkCore;

namespace DemExam.Desktop.ViewModels;

public partial class AdminViewModel(AppDbContext context, INavigationService navigationService) : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<User> _allUsers = [];

    [ObservableProperty] private User? _selectedUser;

    public override async Task OnActivatedAsync()
    {
        AllUsers = new ObservableCollection<User>(await context.Users.ToListAsync());
    }

    [RelayCommand]
    private void AddUser()
    {
        navigationService.NavigateToAsync<CreateEditUserViewModel>();
    }

    [RelayCommand]
    private void EditUser(User? user)
    {
        if (user != null)
            navigationService.NavigateToAsync<CreateEditUserViewModel>(user.Id);
    }

    [RelayCommand]
    private async Task DeleteUser(User? user)
    {
        if (user == null) return;

        var choice = MessageBox.Show("Вы уверены, что хотите удалить пользователя?", "Удаление",
            MessageBoxButton.YesNo);
        if (choice == MessageBoxResult.Yes)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
            AllUsers.Remove(user);
        }
    }
}