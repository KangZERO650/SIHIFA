namespace SIHIFA.Services.Interfaces;

public interface IImageCacheService
{
    string CoversDirectory { get; }
    Task<string> CacheLocalImageAsync(string sourceFilePath);
    Task<string> DownloadAndCacheImageAsync(string imageUrl);
    void DeleteImage(string? localPath);
    Task<int> CleanOrphanedCoversAsync(HashSet<string> referencedPaths);
}
