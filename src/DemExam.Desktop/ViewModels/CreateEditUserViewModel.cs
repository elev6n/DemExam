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
    : ViewModelBase(context, navigationService)
{
    [ObservableProperty] private User? _user = new();

    [ObservableProperty] private bool _isEditMode;

    [ObservableProperty] private string _title = "Добавить нового пользователя";

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

    public async Task InitializeAsync(int? userId = null)
    {
        var rolesTask = Context.UserRoles.ToListAsync();
        var statusesTask = Context.UserStatuses.ToListAsync();
        await Task.WhenAll(rolesTask, statusesTask);

        Roles = new ObservableCollection<UserRole>(rolesTask.Result);
        Statuses = new ObservableCollection<UserStatus>(statusesTask.Result);

        if (userId.HasValue)
        {
            await GetUserAsync(userId.Value);
            SelectedRole = Roles.FirstOrDefault(r => r.Id == User!.UserRole);
            SelectedStatus = Statuses.FirstOrDefault(s => s.Id == User!.UserStatus);
            IsEditMode = true;
            Title = "Изменить пользователя";
        }
        else
        {
            User = new User();
            SelectedRole = Roles.FirstOrDefault();
            SelectedStatus = Statuses.FirstOrDefault();
            IsEditMode = false;
            Title = "Добавить нового пользователя";
        }
    }

    private async Task GetUserAsync(int userId)
    {
        var existingUser = await Context.Users.FindAsync(userId);
        if (existingUser != null)
            User = existingUser;
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
            await NavigationService.NavigateTo<AdminViewModel>();
        }
    }

    [RelayCommand]
    private void Cancel() => NavigationService.NavigateTo<AdminViewModel>();
}