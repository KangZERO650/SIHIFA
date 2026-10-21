using SIHIFA.Models;

namespace SIHIFA.Services.Interfaces;

public interface IBackupService
{
    Task BackupDatabaseAsync(string targetFilePath);
    Task RestoreDatabaseAsync(string sourceFilePath);
    Task ExportToJsonAsync(string targetFilePath);
    Task ExportToCsvAsync(string targetFilePath);
    Task<int> ImportFromJsonAsync(string sourceFilePath);
    Task<int> ImportFromCsvAsync(string sourceFilePath);
}
