using SIHIFA.Models;

namespace SIHIFA.Services.Interfaces;

public interface IMediaService
{
    Task<List<MediaItem>> GetAllAsync();
    Task<MediaItem?> GetByIdAsync(int id);
    Task<MediaItem> AddAsync(MediaItem item);
    Task<MediaItem> UpdateAsync(MediaItem item);
    Task<bool> DeleteAsync(int id);
    Task<DashboardStats> GetStatsAsync();
    Task<int> BulkInsertAsync(IEnumerable<MediaItem> items);
    Task<int> ClearAllAsync();
}
