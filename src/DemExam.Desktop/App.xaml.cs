using System.IO;
using System.Windows;
using DemExam.Desktop.Data;
using DemExam.Desktop.Services;
using DemExam.Desktop.ViewModels;
using DemExam.Desktop.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DemExam.Desktop;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IConfiguration _configuration;
    private readonly ServiceProvider _serviceProvider;

    public App()
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var configPath = Path.Combine(basePath, "appsettings.json");

        if (!File.Exists(configPath)) throw new FileNotFoundException($"Configuration file not found at: {configPath}");

        _configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(_configuration);
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        // Data
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        // Services
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ICaptchaService, CaptchaService>();

        // ViewModels
        services.AddTransient<AuthorizationViewModel>();
        services.AddTransient<AdminViewModel>();
        services.AddTransient<CreateEditUserViewModel>();

        // Views
        services.AddSingleton<MainWindow>();
        services.AddTransient<AuthorizationView>();
        services.AddTransient<AdminView>();
        services.AddTransient<CreateEditUserView>();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}