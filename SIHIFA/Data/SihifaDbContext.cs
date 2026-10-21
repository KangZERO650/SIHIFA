using System.IO;
using Microsoft.EntityFrameworkCore;
using SIHIFA.Models;

namespace SIHIFA.Data;

public class SihifaDbContext : DbContext
{
    public DbSet<MediaItem> MediaItems => Set<MediaItem>();

    public static string DatabaseDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SIHIFA");

    public static string DatabasePath =>
        Path.Combine(DatabaseDirectory, "SIHIFA_LocalDB.db");

    public SihifaDbContext()
    {
    }

    public SihifaDbContext(DbContextOptions<SihifaDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            if (!Directory.Exists(DatabaseDirectory))
            {
                Directory.CreateDirectory(DatabaseDirectory);
            }

            optionsBuilder.UseSqlite($"Data Source={DatabasePath}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MediaItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Judul);
            entity.HasIndex(e => e.TipeMedia);
            entity.HasIndex(e => e.Status);
            entity.Property(e => e.Judul).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TipeMedia).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
        });
    }
}
