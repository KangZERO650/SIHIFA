using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIHIFA.Services.Interfaces;

namespace SIHIFA.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IThemeService _themeService;

    [ObservableProperty]
    private ObservableObject _currentViewModel;

    [ObservableProperty]
    private bool _isDashboardSelected = true;

    [ObservableProperty]
    private bool _isMediaListSelected;

    [ObservableProperty]
    private bool _isSettingsSelected;

    [ObservableProperty]
    private bool _isDarkMode;

    [ObservableProperty]
    private MediaFormViewModel? _activeDialogViewModel;

    [ObservableProperty]
    private bool _isDialogOpen;

    public DashboardViewModel DashboardVM { get; }
    public MediaListViewModel MediaListVM { get; }
    public SettingsViewModel SettingsVM { get; }

    public MainViewModel(
        DashboardViewModel dashboardVM,
        MediaListViewModel mediaListVM,
        SettingsViewModel settingsVM,
        IThemeService themeService)
    {
        DashboardVM = dashboardVM;
        MediaListVM = mediaListVM;
        SettingsVM = settingsVM;
        _themeService = themeService;

        _isDarkMode = _themeService.IsDarkMode;
        _themeService.ThemeChanged += (isDark) => IsDarkMode = isDark;

        _currentViewModel = DashboardVM;

        // Hook cross-ViewModel events
        DashboardVM.RequestNavigateToList += () => NavigateToMediaList();
        DashboardVM.RequestAddNew += () => OpenCreateDialog();

        MediaListVM.RequestOpenFormDialog += (dialogVm) =>
        {
            ActiveDialogViewModel = dialogVm;
            IsDialogOpen = true;
            dialogVm.RequestClose += (saved) =>
            {
                IsDialogOpen = false;
                ActiveDialogViewModel = null;
                if (saved)
                {
                    _ = DashboardVM.LoadDashboardAsync();
                }
            };
        };

        SettingsVM.RequestRefreshAll += () =>
        {
            _ = DashboardVM.LoadDashboardAsync();
            _ = MediaListVM.LoadDataAsync();
        };
    }

    [RelayCommand]
    public async Task InitializeAsync()
    {
        await DashboardVM.LoadDashboardAsync();
        await MediaListVM.LoadDataAsync();
    }

    [RelayCommand]
    public void NavigateToDashboard()
    {
        CurrentViewModel = DashboardVM;
        IsDashboardSelected = true;
        IsMediaListSelected = false;
        IsSettingsSelected = false;
        _ = DashboardVM.LoadDashboardAsync();
    }

    [RelayCommand]
    public void NavigateToMediaList()
    {
        CurrentViewModel = MediaListVM;
        IsDashboardSelected = false;
        IsMediaListSelected = true;
        IsSettingsSelected = false;
        _ = MediaListVM.LoadDataAsync();
    }

    [RelayCommand]
    public void NavigateToSettings()
    {
        CurrentViewModel = SettingsVM;
        IsDashboardSelected = false;
        IsMediaListSelected = false;
        IsSettingsSelected = true;
    }

    [RelayCommand]
    public void OpenCreateDialog()
    {
        MediaListVM.AddNewCommand.Execute(null);
    }

    [RelayCommand]
    public void ToggleTheme()
    {
        _themeService.ToggleTheme();
    }

    [RelayCommand]
    public void CloseDialog()
    {
        IsDialogOpen = false;
        ActiveDialogViewModel = null;
    }
}
