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
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public App()
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var configPath = Path.Combine(basePath, "appsettings.json");
    
        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"Configuration file not found at: {configPath}");
        }
        
        _configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
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
        services.AddScoped<INavigationService, NavigationService>();

        // ViewModels
        services.AddTransient<AdminViewModel>();

        // Views
        services.AddSingleton<MainWindow>();
        services.AddTransient<LoginWindow>();
        services.AddTransient<CaptchaWindow>();
        services.AddTransient<AdminView>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var navigationService = _serviceProvider.GetRequiredService<INavigationService>();

        navigationService.RegisterView<AdminView, AdminViewModel>();

        var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
        loginWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}