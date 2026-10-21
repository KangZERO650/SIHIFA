using System.IO;
using System.Net.Http;
using SIHIFA.Services.Interfaces;

namespace SIHIFA.Services;

public class ImageCacheService : IImageCacheService
{
    private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(15) };

    public string CoversDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SIHIFA", "Covers");

    public ImageCacheService()
    {
        if (!Directory.Exists(CoversDirectory))
        {
            Directory.CreateDirectory(CoversDirectory);
        }
    }

    public async Task<string> CacheLocalImageAsync(string sourceFilePath)
    {
        if (!File.Exists(sourceFilePath))
        {
            throw new FileNotFoundException("File gambar sumber tidak ditemukan", sourceFilePath);
        }

        var ext = Path.GetExtension(sourceFilePath);
        if (string.IsNullOrWhiteSpace(ext)) ext = ".jpg";

        var destFileName = $"cover_{Guid.NewGuid():N}{ext}";
        var destFilePath = Path.Combine(CoversDirectory, destFileName);

        await using var sourceStream = File.OpenRead(sourceFilePath);
        await using var destStream = File.Create(destFilePath);
        await sourceStream.CopyToAsync(destStream);

        return destFilePath;
    }

    public async Task<string> DownloadAndCacheImageAsync(string imageUrl)
    {
        var response = await _httpClient.GetAsync(imageUrl);
        response.EnsureSuccessStatusCode();

        var ext = ".jpg";
        if (imageUrl.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) ext = ".png";
        else if (imageUrl.EndsWith(".webp", StringComparison.OrdinalIgnoreCase)) ext = ".webp";

        var destFileName = $"cover_{Guid.NewGuid():N}{ext}";
        var destFilePath = Path.Combine(CoversDirectory, destFileName);

        await using var contentStream = await response.Content.ReadAsStreamAsync();
        await using var fileStream = File.Create(destFilePath);
        await contentStream.CopyToAsync(fileStream);

        return destFilePath;
    }

    public void DeleteImage(string? localPath)
    {
        if (string.IsNullOrWhiteSpace(localPath)) return;

        try
        {
            if (File.Exists(localPath) && localPath.StartsWith(CoversDirectory, StringComparison.OrdinalIgnoreCase))
            {
                File.Delete(localPath);
            }
        }
        catch
        {
            // Abaikan kesalahan penghapusan non-kritis
        }
    }

    public Task<int> CleanOrphanedCoversAsync(HashSet<string> referencedPaths)
    {
        return Task.Run(() =>
        {
            int deletedCount = 0;
            if (!Directory.Exists(CoversDirectory)) return deletedCount;

            var files = Directory.GetFiles(CoversDirectory);
            foreach (var file in files)
            {
                if (!referencedPaths.Contains(file))
                {
                    try
                    {
                        File.Delete(file);
                        deletedCount++;
                    }
                    catch
                    {
                        // Ignore locked files
                    }
                }
            }

            return deletedCount;
        });
    }
}
