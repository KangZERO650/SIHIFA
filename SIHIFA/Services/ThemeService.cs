using System.IO;
using System.Windows;
using SIHIFA.Services.Interfaces;

namespace SIHIFA.Services;

public class ThemeService : IThemeService
{
    private bool _isDarkMode = true; // Default Dark Mode yang modern

    public bool IsDarkMode => _isDarkMode;

    public event Action<bool>? ThemeChanged;

    private static string SettingsFilePath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SIHIFA", "theme.config");

    public ThemeService()
    {
        LoadSavedTheme();
    }

    private void LoadSavedTheme()
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                var content = File.ReadAllText(SettingsFilePath).Trim();
                if (bool.TryParse(content, out var isDark))
                {
                    _isDarkMode = isDark;
                }
            }
        }
        catch
        {
            _isDarkMode = true;
        }
    }

    private void SaveTheme()
    {
        try
        {
            var dir = Path.GetDirectoryName(SettingsFilePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(SettingsFilePath, _isDarkMode.ToString());
        }
        catch
        {
            // Ignore error saving theme
        }
    }

    public void SetTheme(bool isDark)
    {
        _isDarkMode = isDark;
        ApplyThemeToApp();
        SaveTheme();
        ThemeChanged?.Invoke(_isDarkMode);
    }

    public void ToggleTheme()
    {
        SetTheme(!_isDarkMode);
    }

    private void ApplyThemeToApp()
    {
        var app = Application.Current;
        if (app == null) return;

        var themeUri = _isDarkMode
            ? new Uri("pack://application:,,,/Styles/Colors.Dark.xaml", UriKind.Absolute)
            : new Uri("pack://application:,,,/Styles/Colors.Light.xaml", UriKind.Absolute);

        var newDict = new ResourceDictionary { Source = themeUri };

        // Cari dictionary tema yang sedang aktif di MergedDictionaries
        ResourceDictionary? existingTheme = null;
        foreach (var dict in app.Resources.MergedDictionaries)
        {
            if (dict.Source != null && (dict.Source.ToString().Contains("Colors.Dark.xaml") || dict.Source.ToString().Contains("Colors.Light.xaml")))
            {
                existingTheme = dict;
                break;
            }
        }

        if (existingTheme != null)
        {
            int index = app.Resources.MergedDictionaries.IndexOf(existingTheme);
            app.Resources.MergedDictionaries[index] = newDict;
        }
        else
        {
            app.Resources.MergedDictionaries.Insert(0, newDict);
        }
    }
}
