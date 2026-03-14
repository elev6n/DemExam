using DemExam.Desktop.Models;

namespace DemExam.Desktop.Store;

public static class Session
{
    public static User? CurrentUser { get; set; }

    public static int FailedAttempts { get; set; } = 0;
}