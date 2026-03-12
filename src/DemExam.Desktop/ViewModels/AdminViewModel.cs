using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DemExam.Desktop.Data;
using DemExam.Desktop.Models;
using DemExam.Desktop.Services;
using Microsoft.EntityFrameworkCore;

namespace DemExam.Desktop.ViewModels;

public partial class AdminViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<User> _allUsers = [];

    [ObservableProperty] private User? _selectedUser;

    public AdminViewModel(AppDbContext context, INavigationService navigationService) : base(context, navigationService)
    {
        LoadUsers();
    }

    private async void LoadUsers()
    {
        AllUsers = new ObservableCollection<User>(await Context.Users.ToListAsync());
    }

    [RelayCommand]
    private void AddUser()
    {
        NavigationService.NavigateTo<CreateEditUserViewModel>();
    }

    [RelayCommand]
    private void EditUser(User? user)
    {
        if (user != null)
            NavigationService.NavigateTo<CreateEditUserViewModel>();
    }

    [RelayCommand]
    private async Task DeleteUser(User? user) 
    {
        if (user == null) return;

        var choice = MessageBox.Show("Вы уверены, что хотите удалить пользователя?", "Удаление",
            MessageBoxButton.YesNo);
        if (choice == MessageBoxResult.Yes)
        {
            Context.Users.Remove(user);
            await Context.SaveChangesAsync();
            AllUsers.Remove(user);
        }
    }
}