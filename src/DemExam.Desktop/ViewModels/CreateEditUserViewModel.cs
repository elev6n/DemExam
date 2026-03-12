using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DemExam.Desktop.Data;
using DemExam.Desktop.Exceptions;
using DemExam.Desktop.Models;
using DemExam.Desktop.Services;
using Microsoft.EntityFrameworkCore;

namespace DemExam.Desktop.ViewModels;

public partial class CreateEditUserViewModel(AppDbContext context, INavigationService navigationService)
    : ViewModelBase, IParameterReceiver
{
    [ObservableProperty] private User _user = null!;

    [ObservableProperty] private bool _isEditMode;

    [ObservableProperty] private ObservableCollection<UserRole> _roles = [];

    [ObservableProperty] private ObservableCollection<UserStatus> _statuses = [];

    [ObservableProperty] private UserRole? _selectedRole;

    [ObservableProperty] private UserStatus? _selectedStatus;

    partial void OnSelectedRoleChanged(UserRole? value)
    {
        if (value != null)
            User.UserRole = value.Id;
    }

    partial void OnSelectedStatusChanged(UserStatus? value)
    {
        if (value != null)
            User.UserStatus = value.Id;
    }

    public async Task OnNavigatedToAsync(object? parameter)
    {
        await LoadRolesAndStatusesAsync();

        if (parameter is int userId)
        {
            await LoadUser(userId);
            IsEditMode = true;
            Title = "Изменить пользователя";

            SelectedRole = Roles.First(r => r.Id == User.UserRole);
            SelectedStatus = Statuses.First(s => s.Id == User.UserStatus);
        }
        else
        {
            User = new User();
            IsEditMode = false;
            Title = "Добавить нового пользователя";

            SelectedRole = Roles.First();
            SelectedStatus = Statuses.First();
        }
    }

    private async Task LoadRolesAndStatusesAsync()
    {
        var roles = await context.UserRoles.ToListAsync();
        var statuses = await context.UserStatuses.ToListAsync();

        Roles = new ObservableCollection<UserRole>(roles);
        Statuses = new ObservableCollection<UserStatus>(statuses);
    }

    private async Task LoadUser(int userId)
    {
        try
        {
            User = await context.Users.FindAsync(userId) ??
                   throw new NotFoundException($"Пользователь с ID: {userId} не найден");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(User.FirstName) ||
            string.IsNullOrWhiteSpace(User.LastName) ||
            string.IsNullOrWhiteSpace(User.Login) ||
            string.IsNullOrWhiteSpace(User.Password))
        {
            ErrorMessage = "Заполните все поля!";
            return;
        }

        try
        {
            if (IsEditMode)
                context.Users.Update(User);
            else
                await context.Users.AddAsync(User);

            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            MessageBox.Show(ex.Message);
        }
        finally
        {
            await navigationService.NavigateToAsync<AdminViewModel>();
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        User = new User();
        Roles.Clear();
        Statuses.Clear();
        navigationService.NavigateToAsync<AdminViewModel>();
    }
}