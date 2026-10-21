using Microsoft.EntityFrameworkCore;
using SIHIFA.Data;
using SIHIFA.Models;
using SIHIFA.Services.Interfaces;

namespace SIHIFA.Services;

public class MediaService : IMediaService
{
    private readonly IDbContextFactory<SihifaDbContext> _contextFactory;

    public MediaService(IDbContextFactory<SihifaDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<MediaItem>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.MediaItems
            .AsNoTracking()
            .OrderByDescending(m => m.UpdatedAt)
            .ToListAsync();
    }

    public async Task<MediaItem?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.MediaItems
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<MediaItem> AddAsync(MediaItem item)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        item.CreatedAt = DateTime.Now;
        item.UpdatedAt = DateTime.Now;

        context.MediaItems.Add(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<MediaItem> UpdateAsync(MediaItem item)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var existing = await context.MediaItems.FirstOrDefaultAsync(m => m.Id == item.Id);
        if (existing == null)
        {
            throw new InvalidOperationException($"Item dengan Id {item.Id} tidak ditemukan.");
        }

        existing.Judul = item.Judul;
        existing.TipeMedia = item.TipeMedia;
        existing.Status = item.Status;
        existing.Rating = item.Rating;
        existing.Genre = item.Genre;
        existing.PathCoverLocal = item.PathCoverLocal;
        existing.Catatan = item.Catatan;
        existing.UpdatedAt = DateTime.Now;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var item = await context.MediaItems.FirstOrDefaultAsync(m => m.Id == id);
        if (item == null) return false;

        context.MediaItems.Remove(item);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<DashboardStats> GetStatsAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var allItems = await context.MediaItems.AsNoTracking().ToListAsync();

        var stats = new DashboardStats
        {
            TotalCount = allItems.Count,
            CompletedCount = allItems.Count(x => x.Status.Contains("Selesai") || x.Status.Contains("Completed")),
            OngoingCount = allItems.Count(x => x.Status.Contains("Sedang")),
            PlanToCount = allItems.Count(x => x.Status.Contains("Plan")),
            DroppedCount = allItems.Count(x => x.Status.Contains("Ditinggalkan") || x.Status.Contains("Dropped")),
            AverageRating = allItems.Count > 0 ? Math.Round(allItems.Average(x => x.Rating), 1) : 0.0,
            RecentItems = allItems.OrderByDescending(x => x.UpdatedAt).Take(6).ToList()
        };

        // Grouping by Type
        foreach (var type in MediaConstants.Types.All)
        {
            stats.CountByType[type] = allItems.Count(x => x.TipeMedia == type);
        }

        // Grouping by Status
        foreach (var status in MediaConstants.Statuses.All)
        {
            stats.CountByStatus[status] = allItems.Count(x => x.Status == status);
        }

        // Rating ranges: 1-4, 5-6, 7-8, 9-10
        stats.RatingDistribution["9.0 - 10.0"] = allItems.Count(x => x.Rating >= 9.0);
        stats.RatingDistribution["7.0 - 8.9"] = allItems.Count(x => x.Rating >= 7.0 && x.Rating < 9.0);
        stats.RatingDistribution["5.0 - 6.9"] = allItems.Count(x => x.Rating >= 5.0 && x.Rating < 7.0);
        stats.RatingDistribution["1.0 - 4.9"] = allItems.Count(x => x.Rating < 5.0);

        return stats;
    }

    public async Task<int> BulkInsertAsync(IEnumerable<MediaItem> items)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var list = items.ToList();
            await context.MediaItems.AddRangeAsync(list);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return list.Count;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<int> ClearAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var all = await context.MediaItems.ToListAsync();
        context.MediaItems.RemoveRange(all);
        return await context.SaveChangesAsync();
    }
}
