using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIHIFA.Models;
using SIHIFA.Services.Interfaces;

namespace SIHIFA.ViewModels;

public partial class MediaListViewModel : ObservableObject
{
    private readonly IMediaService _mediaService;
    private readonly Func<MediaFormViewModel> _formFactory;

    public ObservableCollection<MediaItem> MediaItems { get; } = new();

    public ICollectionView FilteredItems { get; }

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _selectedTypeFilter = "Semua";

    [ObservableProperty]
    private string _selectedStatusFilter = "Semua";

    [ObservableProperty]
    private bool _isCardView = false; // False = DataGrid modern, True = Card Gallery

    [ObservableProperty]
    private MediaItem? _selectedItem;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private int _itemCount;

    public List<string> TypeFilterOptions { get; } = ["Semua", .. MediaConstants.Types.All];
    public List<string> StatusFilterOptions { get; } = ["Semua", .. MediaConstants.Statuses.All];

    public event Action<MediaFormViewModel>? RequestOpenFormDialog;

    public MediaListViewModel(IMediaService mediaService, Func<MediaFormViewModel> formFactory)
    {
        _mediaService = mediaService;
        _formFactory = formFactory;

        FilteredItems = CollectionViewSource.GetDefaultView(MediaItems);
        FilteredItems.Filter = FilterPredicate;
    }

    partial void OnSearchTextChanged(string value) => RefreshFilter();
    partial void OnSelectedTypeFilterChanged(string value) => RefreshFilter();
    partial void OnSelectedStatusFilterChanged(string value) => RefreshFilter();

    private void RefreshFilter()
    {
        FilteredItems.Refresh();
        ItemCount = FilteredItems.Cast<object>().Count();
    }

    private bool FilterPredicate(object obj)
    {
        if (obj is not MediaItem item) return false;

        // Filter Tipe Media
        if (SelectedTypeFilter != "Semua" && !string.Equals(item.TipeMedia, SelectedTypeFilter, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // Filter Status
        if (SelectedStatusFilter != "Semua" && !item.Status.Contains(SelectedStatusFilter, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // Filter Pencarian Teks (Judul, Genre, Catatan)
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var query = SearchText.Trim();
            bool matchTitle = item.Judul.Contains(query, StringComparison.OrdinalIgnoreCase);
            bool matchGenre = item.Genre?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false;
            bool matchNotes = item.Catatan?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false;

            if (!matchTitle && !matchGenre && !matchNotes)
            {
                return false;
            }
        }

        return true;
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _mediaService.GetAllAsync();
            MediaItems.Clear();
            foreach (var item in data)
            {
                MediaItems.Add(item);
            }
            RefreshFilter();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void AddNew()
    {
        var formVm = _formFactory();
        formVm.ResetForCreate();
        formVm.RequestClose += async (saved) =>
        {
            if (saved)
            {
                await LoadDataAsync();
            }
        };

        RequestOpenFormDialog?.Invoke(formVm);
    }

    [RelayCommand]
    private void Edit(MediaItem? item)
    {
        item ??= SelectedItem;
        if (item == null) return;

        var formVm = _formFactory();
        formVm.LoadForEdit(item);
        formVm.RequestClose += async (saved) =>
        {
            if (saved)
            {
                await LoadDataAsync();
            }
        };

        RequestOpenFormDialog?.Invoke(formVm);
    }

    [RelayCommand]
    private async Task DeleteAsync(MediaItem? item)
    {
        item ??= SelectedItem;
        if (item == null) return;

        var result = System.Windows.MessageBox.Show(
            $"Apakah Anda yakin ingin menghapus \"{item.Judul}\" dari pustaka?",
            "Konfirmasi Hapus",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Question);

        if (result == System.Windows.MessageBoxResult.Yes)
        {
            await _mediaService.DeleteAsync(item.Id);
            MediaItems.Remove(item);
            RefreshFilter();
        }
    }

    [RelayCommand]
    private void ToggleViewMode()
    {
        IsCardView = !IsCardView;
    }

    [RelayCommand]
    private void ResetFilters()
    {
        SearchText = string.Empty;
        SelectedTypeFilter = "Semua";
        SelectedStatusFilter = "Semua";
    }
}
