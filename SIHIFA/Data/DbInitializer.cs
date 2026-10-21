using Microsoft.EntityFrameworkCore;
using SIHIFA.Models;

namespace SIHIFA.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(SihifaDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.MediaItems.AnyAsync())
        {
            return; // Data sudah ada
        }

        // Seed data sampel awal
        var sampleItems = new List<MediaItem>
        {
            new()
            {
                Judul = "Sousou no Frieren (Frieren: Beyond Journey's End)",
                TipeMedia = MediaConstants.Types.Anime,
                Status = MediaConstants.Statuses.Completed,
                Rating = 9.8,
                Genre = "Fantasy, Adventure, Drama",
                Catatan = "Masterpiece narasi tentang waktu, kenangan, dan makna kehidupan.",
                CreatedAt = DateTime.Now.AddDays(-20),
                UpdatedAt = DateTime.Now.AddDays(-10)
            },
            new()
            {
                Judul = "Steins;Gate",
                TipeMedia = MediaConstants.Types.Anime,
                Status = MediaConstants.Statuses.Completed,
                Rating = 9.6,
                Genre = "Sci-Fi, Thriller, Mystery",
                Catatan = "Plot time travel terbaik dengan eksekusi karakter Okabe Rintaro yang luar biasa.",
                CreatedAt = DateTime.Now.AddDays(-35),
                UpdatedAt = DateTime.Now.AddDays(-25)
            },
            new()
            {
                Judul = "Jujutsu Kaisen Season 2 (Shibuya Incident)",
                TipeMedia = MediaConstants.Types.Anime,
                Status = MediaConstants.Statuses.Ongoing,
                Rating = 9.0,
                Genre = "Action, Fantasy, Supernatural",
                Catatan = "Animasi pertarungan tingkat tinggi dari MAPPA, pacing sangat intens.",
                CreatedAt = DateTime.Now.AddDays(-12),
                UpdatedAt = DateTime.Now.AddDays(-2)
            },
            new()
            {
                Judul = "Interstellar",
                TipeMedia = MediaConstants.Types.Film,
                Status = MediaConstants.Statuses.Completed,
                Rating = 9.5,
                Genre = "Sci-Fi, Adventure, Drama",
                Catatan = "Karya agung Christopher Nolan dengan skor musik Hans Zimmer yang menggetarkan.",
                CreatedAt = DateTime.Now.AddDays(-60),
                UpdatedAt = DateTime.Now.AddDays(-40)
            },
            new()
            {
                Judul = "Oppenheimer",
                TipeMedia = MediaConstants.Types.Film,
                Status = MediaConstants.Statuses.Completed,
                Rating = 9.2,
                Genre = "Biography, Drama, History",
                Catatan = "Visual dan audio montase spektakuler mengenai dilema moral penciptaan bom atom.",
                CreatedAt = DateTime.Now.AddDays(-15),
                UpdatedAt = DateTime.Now.AddDays(-5)
            },
            new()
            {
                Judul = "Dune: Part Two",
                TipeMedia = MediaConstants.Types.Film,
                Status = MediaConstants.Statuses.PlanTo,
                Rating = 8.9,
                Genre = "Sci-Fi, Action, Adventure",
                Catatan = "Rencana tonton marathon weekend bersama teman.",
                CreatedAt = DateTime.Now.AddDays(-3),
                UpdatedAt = DateTime.Now.AddDays(-3)
            },
            new()
            {
                Judul = "Random Access Memories - Daft Punk",
                TipeMedia = MediaConstants.Types.Musik,
                Status = MediaConstants.Statuses.Completed,
                Rating = 9.7,
                Genre = "Electronic, Funk, Disco",
                Catatan = "Album legendaris. Sound engineering dan mixing audio sangat detail dan hangat.",
                CreatedAt = DateTime.Now.AddDays(-45),
                UpdatedAt = DateTime.Now.AddDays(-30)
            },
            new()
            {
                Judul = "Atomic Habits - James Clear",
                TipeMedia = MediaConstants.Types.Buku,
                Status = MediaConstants.Statuses.Completed,
                Rating = 9.3,
                Genre = "Self-Improvement, Psychology",
                Catatan = "Metodologi 1% better every day dan identity-based habit sangat aplikatif.",
                CreatedAt = DateTime.Now.AddDays(-50),
                UpdatedAt = DateTime.Now.AddDays(-20)
            },
            new()
            {
                Judul = "Clean Architecture - Robert C. Martin",
                TipeMedia = MediaConstants.Types.Buku,
                Status = MediaConstants.Statuses.Ongoing,
                Rating = 9.1,
                Genre = "Teknologi, Software Engineering",
                Catatan = "Prinsip pemisahan concerns dan dependency inversion untuk sistem modular.",
                CreatedAt = DateTime.Now.AddDays(-8),
                UpdatedAt = DateTime.Now.AddDays(-1)
            },
            new()
            {
                Judul = "Desain Pola WPF & MVVM Best Practices",
                TipeMedia = MediaConstants.Types.Materi,
                Status = MediaConstants.Statuses.Ongoing,
                Rating = 9.4,
                Genre = "Teknologi, Programming",
                Catatan = "Rangkuman CommunityToolkit.Mvvm, ObservableProperty, dan Data Virtualization.",
                CreatedAt = DateTime.Now.AddDays(-5),
                UpdatedAt = DateTime.Now
            },
            new()
            {
                Judul = "Catatan Optimasi Kueri SQLite & ACID",
                TipeMedia = MediaConstants.Types.Materi,
                Status = MediaConstants.Statuses.PlanTo,
                Rating = 8.5,
                Genre = "Teknologi, Database",
                Catatan = "Indeks B-Tree, pragma synchronous, dan transaksi bulk insert di SQLite.",
                CreatedAt = DateTime.Now.AddDays(-2),
                UpdatedAt = DateTime.Now.AddDays(-2)
            }
        };

        await context.MediaItems.AddRangeAsync(sampleItems);
        await context.SaveChangesAsync();
    }

    public static async Task SeedStressTestDataAsync(SihifaDbContext context, int count = 10000)
    {
        var random = new Random();
        var types = MediaConstants.Types.All;
        var statuses = MediaConstants.Statuses.All;
        var genres = MediaConstants.Genres.Common;

        var items = new List<MediaItem>(count);
        for (int i = 1; i <= count; i++)
        {
            var t = types[random.Next(types.Length)];
            var s = statuses[random.Next(statuses.Length)];
            var g = genres[random.Next(genres.Length)];
            var rating = Math.Round(5.0 + (random.NextDouble() * 5.0), 1);

            items.Add(new MediaItem
            {
                Judul = $"Entri Koleksi #{i:D5} - {t} Favorit {g}",
                TipeMedia = t,
                Status = s,
                Rating = rating,
                Genre = g,
                Catatan = $"Catatan dummy benchmarking untuk data entri ke-{i}...",
                CreatedAt = DateTime.Now.AddDays(-random.Next(1, 365)),
                UpdatedAt = DateTime.Now
            });
        }

        // Simpan menggunakan transaksi batch
        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // Batch per 1000 baris agar memori efisien
            const int batchSize = 1000;
            for (int i = 0; i < items.Count; i += batchSize)
            {
                var batch = items.Skip(i).Take(batchSize);
                await context.MediaItems.AddRangeAsync(batch);
                await context.SaveChangesAsync();
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
