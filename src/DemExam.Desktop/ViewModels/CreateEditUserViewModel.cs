using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DemExam.Desktop.Data;
using DemExam.Desktop.Models;
using DemExam.Desktop.Services;
using Microsoft.EntityFrameworkCore;

namespace DemExam.Desktop.ViewModels;

public partial class CreateEditUserViewModel(AppDbContext context, INavigationService navigationService)
    : ViewModelBase(context, navigationService), IParameterReceiver
{
    [ObservableProperty] private User? _user;

    [ObservableProperty] private bool _isEditMode;

    [ObservableProperty] private ObservableCollection<UserRole> _roles = [];

    [ObservableProperty] private ObservableCollection<UserStatus> _statuses = [];

    [ObservableProperty] private UserRole? _selectedRole;

    [ObservableProperty] private UserStatus? _selectedStatus;

    partial void OnSelectedRoleChanged(UserRole? value)
    {
        if (value != null && User != null)
            User.UserRole = value.Id;
    }

    partial void OnSelectedStatusChanged(UserStatus? value)
    {
        if (value != null && User != null)
            User.UserStatus = value.Id;
    }

    public async Task OnNavigatedToAsync(object? parameter)
    {
        await LoadRolesAndStatusesAsync();

        if (parameter is User user)
        {
            User = user;
            IsEditMode = true;
            Title = "Изменить пользователя";
        }
        else
        {
            User = new User();
            IsEditMode = false;
            Title = "Добавить нового пользователя";
        }

        if (User != null)
        {
            SelectedRole = Roles.FirstOrDefault(r => r.Id == User.UserRole);
            SelectedStatus = Statuses.FirstOrDefault(s => s.Id == User.UserStatus);
        }
    }

    private async Task LoadRolesAndStatusesAsync()
    {
        var rolesTask = Context.UserRoles.ToListAsync();
        var statusesTask = Context.UserStatuses.ToListAsync();
        await Task.WhenAll(rolesTask, statusesTask);

        Roles = new ObservableCollection<UserRole>(await rolesTask);
        Statuses = new ObservableCollection<UserStatus>(await statusesTask);
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(User!.FirstName) ||
            string.IsNullOrWhiteSpace(User.LastName) ||
            string.IsNullOrWhiteSpace(User.Login) ||
            string.IsNullOrWhiteSpace(User.Password))
        {
            ErrorMessage = "Заполните все поля!";
            return;
        }

        if (SelectedRole == null || SelectedStatus == null)
        {
            ErrorMessage = "Выберите роль и статус пользователя!";
            return;
        }

        try
        {
            if (IsEditMode)
                Context.Users.Update(User);
            else
                await Context.Users.AddAsync(User);

            await Context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            MessageBox.Show(ex.Message);
        }
        finally
        {
            await NavigationService.NavigateToAsync<AdminViewModel>();
        }
    }

    [RelayCommand]
    private void Cancel() => NavigationService.NavigateToAsync<AdminViewModel>();
}