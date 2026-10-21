using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Globalization;
using System.Text.RegularExpressions;
using SIHIFA.Models;
using SIHIFA.Services.Interfaces;

namespace SIHIFA.ViewModels;

public partial class MediaFormViewModel : ObservableValidator
{
    private readonly IMediaService _mediaService;
    private readonly IImageCacheService _imageCacheService;

    public int Id { get; set; }

    [ObservableProperty]
    private bool _isEditMode;

    [ObservableProperty]
    private string _formTitle = "Tambah Entri Media Baru";

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Judul media wajib diisi.")]
    [MinLength(2, ErrorMessage = "Judul minimal terdiri dari 2 karakter.")]
    [MaxLength(200, ErrorMessage = "Judul tidak boleh melebihi 200 karakter.")]
    private string _judul = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Pilih salah satu tipe media.")]
    private string _tipeMedia = MediaConstants.Types.Anime;

    [ObservableProperty]
    [Required(ErrorMessage = "Pilih status konsumsi media.")]
    private string _status = MediaConstants.Statuses.PlanTo;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Range(0.0, 10.0, ErrorMessage = "Rating harus berada pada rentang 0.0 - 10.0")]
    private double _rating = 8.5;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [CustomValidation(typeof(MediaFormViewModel), nameof(ValidateRatingText))]
    private string _ratingText = "8.5";

    private bool _isSyncingRating;

    partial void OnRatingChanged(double value)
    {
        if (!_isSyncingRating)
        {
            _isSyncingRating = true;
            RatingText = value.ToString("0.0", CultureInfo.InvariantCulture);
            _isSyncingRating = false;
        }
    }

    partial void OnRatingTextChanged(string value)
    {
        if (_isSyncingRating || string.IsNullOrWhiteSpace(value)) return;

        var normalized = value.Trim().Replace(',', '.');
        if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double val))
        {
            if (val >= 0.0 && val <= 10.0)
            {
                var rounded = Math.Round(val, 1);
                if (Math.Abs(Rating - rounded) > 0.001)
                {
                    _isSyncingRating = true;
                    Rating = rounded;
                    _isSyncingRating = false;
                }
            }
        }
    }

    public void FormatRatingText()
    {
        var normalized = RatingText?.Trim().Replace(',', '.') ?? "0";
        if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double val))
        {
            val = Math.Clamp(val, 0.0, 10.0);
            _isSyncingRating = true;
            Rating = Math.Round(val, 1);
            RatingText = Rating.ToString("0.0", CultureInfo.InvariantCulture);
            _isSyncingRating = false;
            ClearErrors(nameof(RatingText));
        }
        else
        {
            RatingText = Rating.ToString("0.0", CultureInfo.InvariantCulture);
            ClearErrors(nameof(RatingText));
        }
    }

    public static ValidationResult? ValidateRatingText(string value, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ValidationResult("Rating wajib diisi angka.");
        }

        var normalized = value.Trim().Replace(',', '.');
        if (!Regex.IsMatch(normalized, @"^[0-9]+(\.[0-9]+)?$"))
        {
            return new ValidationResult("Hanya boleh berisi karakter angka (0-9).");
        }

        if (!double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double val))
        {
            return new ValidationResult("Format angka rating tidak valid.");
        }

        if (val < 0.0 || val > 10.0)
        {
            return new ValidationResult("Rating harus antara 0.0 - 10.0.");
        }

        return ValidationResult.Success;
    }

    [ObservableProperty]
    private string? _genre;

    [ObservableProperty]
    private string? _pathCoverLocal;

    [ObservableProperty]
    private string? _catatan;

    [ObservableProperty]
    private bool _isSaving;

    public IReadOnlyList<string> AvailableTypes => MediaConstants.Types.All;
    public IReadOnlyList<string> AvailableStatuses => MediaConstants.Statuses.All;

    public event Action<bool>? RequestClose;

    public MediaFormViewModel(IMediaService mediaService, IImageCacheService imageCacheService)
    {
        _mediaService = mediaService;
        _imageCacheService = imageCacheService;
    }

    public void LoadForEdit(MediaItem item)
    {
        Id = item.Id;
        Judul = item.Judul;
        TipeMedia = item.TipeMedia;
        Status = item.Status;
        Rating = item.Rating;
        RatingText = item.Rating.ToString("0.0", CultureInfo.InvariantCulture);
        Genre = item.Genre;
        PathCoverLocal = item.PathCoverLocal;
        Catatan = item.Catatan;

        IsEditMode = true;
        FormTitle = $"Edit: {item.Judul}";
        ValidateAllProperties();
    }

    public void ResetForCreate()
    {
        Id = 0;
        Judul = string.Empty;
        TipeMedia = MediaConstants.Types.Anime;
        Status = MediaConstants.Statuses.PlanTo;
        Rating = 8.0;
        RatingText = "8.0";
        Genre = string.Empty;
        PathCoverLocal = null;
        Catatan = string.Empty;

        IsEditMode = false;
        FormTitle = "Tambah Entri Media Baru";
        ClearErrors();
    }

    [RelayCommand]
    private async Task BrowseCoverAsync()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Pilih Gambar Cover / Poster",
            Filter = "File Gambar (*.jpg;*.jpeg;*.png;*.webp)|*.jpg;*.jpeg;*.png;*.webp|Semua File (*.*)|*.*"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                var cachedPath = await _imageCacheService.CacheLocalImageAsync(dialog.FileName);
                PathCoverLocal = cachedPath;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Gagal menyimpan gambar cover: {ex.Message}", "Kesalahan Gambar",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }
        }
    }

    [RelayCommand]
    private void RemoveCover()
    {
        PathCoverLocal = null;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        FormatRatingText();
        ValidateAllProperties();
        if (HasErrors)
        {
            return;
        }

        IsSaving = true;
        try
        {
            var item = new MediaItem
            {
                Id = Id,
                Judul = Judul.Trim(),
                TipeMedia = TipeMedia,
                Status = Status,
                Rating = Math.Round(Rating, 1),
                Genre = string.IsNullOrWhiteSpace(Genre) ? null : Genre.Trim(),
                PathCoverLocal = PathCoverLocal,
                Catatan = string.IsNullOrWhiteSpace(Catatan) ? null : Catatan.Trim()
            };

            if (IsEditMode)
            {
                await _mediaService.UpdateAsync(item);
            }
            else
            {
                await _mediaService.AddAsync(item);
            }

            RequestClose?.Invoke(true);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Gagal menyimpan data: {ex.Message}", "Database Error",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        RequestClose?.Invoke(false);
    }
}
