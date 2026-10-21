using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;

namespace SIHIFA.Converters;

public class StatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var status = value?.ToString() ?? string.Empty;
        var app = Application.Current;

        if (status.Contains("Selesai") || status.Contains("Completed"))
            return app.TryFindResource("Brush.Status.Completed") as Brush ?? Brushes.Green;

        if (status.Contains("Sedang") || status.Contains("Ongoing"))
            return app.TryFindResource("Brush.Status.Ongoing") as Brush ?? Brushes.DodgerBlue;

        if (status.Contains("Plan"))
            return app.TryFindResource("Brush.Status.PlanTo") as Brush ?? Brushes.Orange;

        if (status.Contains("On Hold") || status.Contains("Ditunda"))
            return app.TryFindResource("Brush.Status.OnHold") as Brush ?? Brushes.Purple;

        if (status.Contains("Dropped") || status.Contains("Ditinggalkan"))
            return app.TryFindResource("Brush.Status.Dropped") as Brush ?? Brushes.Crimson;

        return app.TryFindResource("Brush.Text.Muted") as Brush ?? Brushes.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class RatingFormattedConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double d)
        {
            return $"★ {d:F1}";
        }
        return "★ -";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class ImagePathToBitmapConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var path = value?.ToString();
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            return null; // Akan menampilkan placeholder di XAML jika null
        }

        try
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(path, UriKind.Absolute);
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
        catch
        {
            return null;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class NullToVisibilityConverter : IValueConverter
{
    public bool Invert { get; set; } = false;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isNull = value == null || string.IsNullOrWhiteSpace(value.ToString());
        bool shouldInvert = Invert || string.Equals(parameter?.ToString(), "invert", StringComparison.OrdinalIgnoreCase);

        if (shouldInvert)
        {
            isNull = !isNull;
        }

        return isNull ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class NoteSnippetConverter : IValueConverter
{
    public int MaxWords { get; set; } = 10;
    public int MaxChars { get; set; } = 65;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var text = value?.ToString();
        if (string.IsNullOrWhiteSpace(text))
        {
            return "—";
        }

        // 1. Bersihkan karakter newline / enter (\r, \n) agar satu baris horizontal rapi
        var cleanText = Regex.Replace(text, @"\s+", " ").Trim();

        int wordLimit = MaxWords;
        if (parameter != null && int.TryParse(parameter.ToString(), out int customLimit))
        {
            wordLimit = customLimit;
        }

        // 2. Potong berdasarkan N kata (delimiter)
        var words = cleanText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string result = cleanText;
        if (words.Length > wordLimit)
        {
            result = string.Join(" ", words.Take(wordLimit)) + "...";
        }

        // 3. Batasi karakter jika ada kata super panjang tanpa spasi (misal URL panjang)
        if (result.Length > MaxChars)
        {
            return result[..MaxChars].TrimEnd() + "...";
        }

        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class StringToGenreListConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var text = value?.ToString();
        if (string.IsNullOrWhiteSpace(text))
        {
            return Array.Empty<string>();
        }

        return text.Split(new[] { ',', ';', '/', '|' }, StringSplitOptions.RemoveEmptyEntries)
                   .Select(g => g.Trim())
                   .Where(g => !string.IsNullOrEmpty(g))
                   .ToList();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

