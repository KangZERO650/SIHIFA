namespace SIHIFA.Models;

public class DashboardStats
{
    public int TotalCount { get; set; }
    public int CompletedCount { get; set; }
    public int OngoingCount { get; set; }
    public int PlanToCount { get; set; }
    public int DroppedCount { get; set; }
    public double AverageRating { get; set; }

    public Dictionary<string, int> CountByType { get; set; } = new();
    public Dictionary<string, int> CountByStatus { get; set; } = new();
    public Dictionary<string, int> RatingDistribution { get; set; } = new();
    public List<MediaItem> RecentItems { get; set; } = new();
}
