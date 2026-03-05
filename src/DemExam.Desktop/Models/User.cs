namespace DemExam.Desktop.Models;

public partial class User
{
    public int Id { get; set; }

    public int UserRole { get; set; }

    public int UserStatus { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? Patronymic { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual UserRole UserRoleNavigation { get; set; } = null!;

    public virtual UserStatus UserStatusNavigation { get; set; } = null!;
}
