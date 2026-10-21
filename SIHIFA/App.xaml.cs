using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SIHIFA.Data;
using SIHIFA.Services;
using SIHIFA.Services.Interfaces;
using SIHIFA.ViewModels;

namespace SIHIFA;

public partial class App : Application
{
    private IServiceProvider? _serviceProvider;

    public IServiceProvider ServiceProvider => _serviceProvider ?? throw new InvalidOperationException("ServiceProvider belum diinisialisasi.");

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        // 1. Inisialisasi Database SQLite & Sample Data
        try
        {
            var dbFactory = _serviceProvider.GetRequiredService<IDbContextFactory<SihifaDbContext>>();
            await using var dbContext = await dbFactory.CreateDbContextAsync();
            await DbInitializer.InitializeAsync(dbContext);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Inisialisasi database SQLite gagal: {ex.Message}", "Database Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // 2. Terapkan Preferensi Tema Pengguna
        var themeService = _serviceProvider.GetRequiredService<IThemeService>();
        themeService.SetTheme(themeService.IsDarkMode);

        // 3. Tampilkan Jendela Utama
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Database Context Factory (Thread-safe untuk async WPF)
        services.AddDbContextFactory<SihifaDbContext>(options =>
        {
            options.UseSqlite($"Data Source={SihifaDbContext.DatabasePath}");
        });

        // Services
        services.AddSingleton<IImageCacheService, ImageCacheService>();
        services.AddSingleton<IMediaService, MediaService>();
        services.AddSingleton<IBackupService, BackupService>();
        services.AddSingleton<IThemeService, ThemeService>();

        // ViewModels
        services.AddSingleton<DashboardViewModel>();
        services.AddSingleton<MediaListViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddTransient<MediaFormViewModel>();
        services.AddSingleton<Func<MediaFormViewModel>>(sp => () => sp.GetRequiredService<MediaFormViewModel>());
        services.AddSingleton<MainViewModel>();

        // Views
        services.AddSingleton<MainWindow>();
    }
}
