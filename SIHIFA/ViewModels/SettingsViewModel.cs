using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using SIHIFA.Data;
using SIHIFA.Services.Interfaces;

namespace SIHIFA.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IBackupService _backupService;
    private readonly IThemeService _themeService;
    private readonly IMediaService _mediaService;
    private readonly IImageCacheService _imageCacheService;

    [ObservableProperty]
    private bool _isDarkMode;

    [ObservableProperty]
    private string _databasePath = SihifaDbContext.DatabasePath;

    [ObservableProperty]
    private string _coversDirectory;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    public event Action? RequestRefreshAll;

    public SettingsViewModel(
        IBackupService backupService,
        IThemeService themeService,
        IMediaService mediaService,
        IImageCacheService imageCacheService)
    {
        _backupService = backupService;
        _themeService = themeService;
        _mediaService = mediaService;
        _imageCacheService = imageCacheService;

        _isDarkMode = _themeService.IsDarkMode;
        _coversDirectory = _imageCacheService.CoversDirectory;

        _themeService.ThemeChanged += (isDark) => IsDarkMode = isDark;
    }

    partial void OnIsDarkModeChanged(bool value)
    {
        if (_themeService.IsDarkMode != value)
        {
            _themeService.SetTheme(value);
        }
    }

    [RelayCommand]
    private async Task BackupDatabaseAsync()
    {
        var dialog = new SaveFileDialog
        {
            Title = "Backup Database SQLite SIHIFA",
            Filter = "SQLite Database (*.db)|*.db",
            FileName = $"SIHIFA_Backup_{DateTime.Now:yyyyMMdd_HHmm}.db"
        };

        if (dialog.ShowDialog() == true)
        {
            IsBusy = true;
            try
            {
                await _backupService.BackupDatabaseAsync(dialog.FileName);
                StatusMessage = $"✓ Database berhasil dibackup ke: {dialog.FileName}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"✗ Gagal backup: {ex.Message}";
                MessageBox.Show(ex.Message, "Error Backup", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    [RelayCommand]
    private async Task RestoreDatabaseAsync()
    {
        var confirm = MessageBox.Show(
            "PERINGATAN: Memulihkan database akan menimpa seluruh data saat ini dengan file yang dipilih.\n\nApakah Anda yakin ingin melanjutkan?",
            "Konfirmasi Pemulihan Data",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirm != MessageBoxResult.Yes) return;

        var dialog = new OpenFileDialog
        {
            Title = "Pilih Berkas Database SQLite untuk Dipulihkan",
            Filter = "SQLite Database (*.db)|*.db|Semua File (*.*)|*.*"
        };

        if (dialog.ShowDialog() == true)
        {
            IsBusy = true;
            try
            {
                await _backupService.RestoreDatabaseAsync(dialog.FileName);
                StatusMessage = $"✓ Database berhasil dipulihkan dari: {dialog.FileName}";
                RequestRefreshAll?.Invoke();
            }
            catch (Exception ex)
            {
                StatusMessage = $"✗ Gagal restore: {ex.Message}";
                MessageBox.Show(ex.Message, "Error Restore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    [RelayCommand]
    private async Task ExportJsonAsync()
    {
        var dialog = new SaveFileDialog
        {
            Title = "Ekspor Koleksi ke Format JSON",
            Filter = "JSON Files (*.json)|*.json",
            FileName = $"SIHIFA_Export_{DateTime.Now:yyyyMMdd}.json"
        };

        if (dialog.ShowDialog() == true)
        {
            IsBusy = true;
            try
            {
                await _backupService.ExportToJsonAsync(dialog.FileName);
                StatusMessage = $"✓ Data berhasil diekspor ke JSON: {dialog.FileName}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"✗ Gagal ekspor JSON: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    [RelayCommand]
    private async Task ExportCsvAsync()
    {
        var dialog = new SaveFileDialog
        {
            Title = "Ekspor Koleksi ke Format CSV",
            Filter = "CSV Files (*.csv)|*.csv",
            FileName = $"SIHIFA_Export_{DateTime.Now:yyyyMMdd}.csv"
        };

        if (dialog.ShowDialog() == true)
        {
            IsBusy = true;
            try
            {
                await _backupService.ExportToCsvAsync(dialog.FileName);
                StatusMessage = $"✓ Data berhasil diekspor ke CSV: {dialog.FileName}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"✗ Gagal ekspor CSV: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    [RelayCommand]
    private async Task ImportJsonAsync()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Pilih Berkas JSON untuk Diimpor",
            Filter = "JSON Files (*.json)|*.json"
        };

        if (dialog.ShowDialog() == true)
        {
            IsBusy = true;
            try
            {
                int count = await _backupService.ImportFromJsonAsync(dialog.FileName);
                StatusMessage = $"✓ Berhasil mengimpor {count} entri dari JSON.";
                RequestRefreshAll?.Invoke();
            }
            catch (Exception ex)
            {
                StatusMessage = $"✗ Gagal impor JSON: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    [RelayCommand]
    private async Task ImportCsvAsync()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Pilih Berkas CSV untuk Diimpor",
            Filter = "CSV Files (*.csv)|*.csv"
        };

        if (dialog.ShowDialog() == true)
        {
            IsBusy = true;
            try
            {
                int count = await _backupService.ImportFromCsvAsync(dialog.FileName);
                StatusMessage = $"✓ Berhasil mengimpor {count} entri dari CSV.";
                RequestRefreshAll?.Invoke();
            }
            catch (Exception ex)
            {
                StatusMessage = $"✗ Gagal impor CSV: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    [RelayCommand]
    private async Task RunStressTestAsync()
    {
        var confirm = MessageBox.Show(
            "Apakah Anda ingin menambahkan 10.000 entri dummy untuk menguji performa DataGrid Virtualization & Filter (< 50ms)?\n\nOperasi ini membutuhkan waktu beberapa detik.",
            "Stress Test 10.000 Entri",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirm != MessageBoxResult.Yes) return;

        IsBusy = true;
        StatusMessage = "Sedang memproses injeksi 10.000 baris data dummy...";
        try
        {
            using var context = new SihifaDbContext();
            await DbInitializer.SeedStressTestDataAsync(context, 10000);
            StatusMessage = "✓ 10.000 data dummy berhasil diinjeksi ke SQLite!";
            RequestRefreshAll?.Invoke();
        }
        catch (Exception ex)
        {
            StatusMessage = $"✗ Gagal injeksi stress test: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CleanCoversAsync()
    {
        IsBusy = true;
        try
        {
            var items = await _mediaService.GetAllAsync();
            var referenced = items
                .Where(x => !string.IsNullOrEmpty(x.PathCoverLocal))
                .Select(x => x.PathCoverLocal!)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            int deleted = await _imageCacheService.CleanOrphanedCoversAsync(referenced);
            StatusMessage = $"✓ Berhasil membersihkan {deleted} file poster yang tidak terpakai.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"✗ Gagal membersihkan poster: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
