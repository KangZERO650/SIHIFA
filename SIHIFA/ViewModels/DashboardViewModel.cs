using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using SIHIFA.Models;
using SIHIFA.Services.Interfaces;

namespace SIHIFA.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IMediaService _mediaService;

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private int _completedCount;

    [ObservableProperty]
    private int _ongoingCount;

    [ObservableProperty]
    private int _planToCount;

    [ObservableProperty]
    private double _averageRating;

    [ObservableProperty]
    private bool _isLoading;

    public ObservableCollection<MediaItem> RecentItems { get; } = new();

    // LiveCharts2 Series
    [ObservableProperty]
    private ISeries[] _typeSeries = Array.Empty<ISeries>();

    [ObservableProperty]
    private ISeries[] _ratingSeries = Array.Empty<ISeries>();

    [ObservableProperty]
    private Axis[] _ratingXAxes = Array.Empty<Axis>();

    public event Action? RequestNavigateToList;
    public event Action? RequestAddNew;

    public DashboardViewModel(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    [RelayCommand]
    public async Task LoadDashboardAsync()
    {
        IsLoading = true;
        try
        {
            var stats = await _mediaService.GetStatsAsync();

            TotalCount = stats.TotalCount;
            CompletedCount = stats.CompletedCount;
            OngoingCount = stats.OngoingCount;
            PlanToCount = stats.PlanToCount;
            AverageRating = stats.AverageRating;

            RecentItems.Clear();
            foreach (var item in stats.RecentItems)
            {
                RecentItems.Add(item);
            }

            BuildCharts(stats);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void BuildCharts(DashboardStats stats)
    {
        // 1. Doughnut Chart Distribusi Tipe Media
        var colors = new[]
        {
            SKColor.Parse("#3B82F6"), // Blue (Anime)
            SKColor.Parse("#8B5CF6"), // Purple (Film)
            SKColor.Parse("#10B981"), // Emerald (Musik)
            SKColor.Parse("#F59E0B"), // Amber (Buku)
            SKColor.Parse("#EC4899"), // Pink (Materi)
        };

        var typeSeriesList = new List<ISeries>();
        int colorIdx = 0;
        foreach (var kvp in stats.CountByType)
        {
            if (kvp.Value > 0)
            {
                var color = colors[colorIdx % colors.Length];
                typeSeriesList.Add(new PieSeries<int>
                {
                    Values = new[] { kvp.Value },
                    Name = kvp.Key,
                    Fill = new SolidColorPaint(color),
                    Pushout = 4,
                    InnerRadius = 55
                });
                colorIdx++;
            }
        }
        TypeSeries = typeSeriesList.ToArray();

        // 2. Bar Chart Distribusi Rating
        var ratingLabels = stats.RatingDistribution.Keys.ToArray();
        var ratingValues = stats.RatingDistribution.Values.ToArray();

        RatingSeries =
        [
            new ColumnSeries<int>
            {
                Values = ratingValues,
                Name = "Jumlah Media",
                Fill = new SolidColorPaint(SKColor.Parse("#3B82F6")),
                Rx = 6,
                Ry = 6
            }
        ];

        RatingXAxes =
        [
            new Axis
            {
                Labels = ratingLabels,
                LabelsPaint = new SolidColorPaint(SKColor.Parse("#94A3B8")),
                TextSize = 12
            }
        ];
    }

    [RelayCommand]
    private void NavigateToList() => RequestNavigateToList?.Invoke();

    [RelayCommand]
    private void AddNew() => RequestAddNew?.Invoke();
}
