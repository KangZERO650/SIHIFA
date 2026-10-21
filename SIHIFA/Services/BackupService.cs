using System.Globalization;
using System.IO;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using SIHIFA.Data;
using SIHIFA.Models;
using SIHIFA.Services.Interfaces;

namespace SIHIFA.Services;

public class BackupService : IBackupService
{
    private readonly IMediaService _mediaService;
    private readonly IDbContextFactory<SihifaDbContext> _contextFactory;

    public BackupService(IMediaService mediaService, IDbContextFactory<SihifaDbContext> contextFactory)
    {
        _mediaService = mediaService;
        _contextFactory = contextFactory;
    }

    public async Task BackupDatabaseAsync(string targetFilePath)
    {
        var dbPath = SihifaDbContext.DatabasePath;
        if (!File.Exists(dbPath))
        {
            throw new FileNotFoundException("File database SQLite belum dibuat.", dbPath);
        }

        // Flush and checkpoint WAL if any
        await using (var context = await _contextFactory.CreateDbContextAsync())
        {
            await context.Database.ExecuteSqlRawAsync("PRAGMA wal_checkpoint(FULL);");
        }

        File.Copy(dbPath, targetFilePath, overwrite: true);
    }

    public async Task RestoreDatabaseAsync(string sourceFilePath)
    {
        if (!File.Exists(sourceFilePath))
        {
            throw new FileNotFoundException("File database sumber tidak ditemukan.", sourceFilePath);
        }

        var dbPath = SihifaDbContext.DatabasePath;
        
        // Clear connection pool to avoid file locks
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();

        File.Copy(sourceFilePath, dbPath, overwrite: true);

        // Verify restored DB
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public async Task ExportToJsonAsync(string targetFilePath)
    {
        var items = await _mediaService.GetAllAsync();
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(items, options);
        await File.WriteAllTextAsync(targetFilePath, json);
    }

    public async Task ExportToCsvAsync(string targetFilePath)
    {
        var items = await _mediaService.GetAllAsync();
        await using var writer = new StreamWriter(targetFilePath);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(items);
    }

    public async Task<int> ImportFromJsonAsync(string sourceFilePath)
    {
        if (!File.Exists(sourceFilePath))
        {
            throw new FileNotFoundException("File JSON tidak ditemukan.", sourceFilePath);
        }

        var json = await File.ReadAllTextAsync(sourceFilePath);
        var items = JsonSerializer.Deserialize<List<MediaItem>>(json);
        if (items == null || items.Count == 0) return 0;

        // Reset IDs to let autoincrement assign new keys
        foreach (var item in items)
        {
            item.Id = 0;
            if (item.CreatedAt == default) item.CreatedAt = DateTime.Now;
            item.UpdatedAt = DateTime.Now;
        }

        return await _mediaService.BulkInsertAsync(items);
    }

    public async Task<int> ImportFromCsvAsync(string sourceFilePath)
    {
        if (!File.Exists(sourceFilePath))
        {
            throw new FileNotFoundException("File CSV tidak ditemukan.", sourceFilePath);
        }

        using var reader = new StreamReader(sourceFilePath);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null
        };
        using var csv = new CsvReader(reader, config);
        var records = csv.GetRecords<MediaItem>().ToList();

        foreach (var item in records)
        {
            item.Id = 0;
            if (item.CreatedAt == default) item.CreatedAt = DateTime.Now;
            item.UpdatedAt = DateTime.Now;
        }

        return await _mediaService.BulkInsertAsync(records);
    }
}
