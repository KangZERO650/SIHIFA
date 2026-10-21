namespace SIHIFA.Services.Interfaces;

public interface IThemeService
{
    bool IsDarkMode { get; }
    event Action<bool>? ThemeChanged;
    void SetTheme(bool isDark);
    void ToggleTheme();
}
