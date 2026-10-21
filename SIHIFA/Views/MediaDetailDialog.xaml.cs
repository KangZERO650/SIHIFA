using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using SIHIFA.Models;

namespace SIHIFA.Views;

public partial class MediaDetailDialog : UserControl
{
    private int _toastToken = 0;

    public MediaDetailDialog()
    {
        InitializeComponent();
    }

    private void CopyToClipboard(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        try
        {
            Clipboard.SetText(text);
        }
        catch
        {
            try
            {
                Clipboard.SetDataObject(text, true);
            }
            catch { }
        }
    }

    private async void ShowToast(string message)
    {
        if (CopyToastBorder == null || CopyToastText == null) return;

        CopyToastText.Text = message;
        CopyToastBorder.Visibility = Visibility.Visible;
        CopyToastBorder.Opacity = 1.0;

        int currentToken = ++_toastToken;
        await Task.Delay(1300);

        if (currentToken == _toastToken)
        {
            var anim = new DoubleAnimation(1.0, 0.0, TimeSpan.FromMilliseconds(250));
            anim.Completed += (s, e) =>
            {
                if (currentToken == _toastToken)
                {
                    CopyToastBorder.Visibility = Visibility.Collapsed;
                }
            };
            CopyToastBorder.BeginAnimation(UIElement.OpacityProperty, anim);
        }
    }

    private async Task FlashBoldEffectAsync(TextBlock textBlock, Brush? highlightBrush = null)
    {
        if (textBlock == null) return;
        var originalWeight = textBlock.FontWeight;
        var originalBrush = textBlock.Foreground;

        // Jika semula normal (< Bold), ganti jadi Bold. Jika semula sudah Bold, pakai ExtraBold
        textBlock.FontWeight = originalWeight >= FontWeights.Bold ? FontWeights.ExtraBold : FontWeights.Bold;
        if (highlightBrush != null)
        {
            textBlock.Foreground = highlightBrush;
        }

        await Task.Delay(250);

        textBlock.FontWeight = originalWeight;
        textBlock.Foreground = originalBrush;
    }

    private async Task FlashGenreTagEffectAsync(Border border)
    {
        if (border == null) return;
        var textBlock = border.Child as TextBlock;
        var originalBorderBrush = border.BorderBrush;
        var originalBackground = border.Background;
        FontWeight originalWeight = FontWeights.Medium;
        Brush? originalForeground = null;

        if (textBlock != null)
        {
            originalWeight = textBlock.FontWeight;
            originalForeground = textBlock.Foreground;
            // Jika semula normal (< Bold), ganti jadi Bold. Jika semula sudah Bold, pakai ExtraBold
            textBlock.FontWeight = originalWeight >= FontWeights.Bold ? FontWeights.ExtraBold : FontWeights.Bold;
            textBlock.Foreground = TryFindResource("Brush.Accent.Primary") as Brush ?? Brushes.DodgerBlue;
        }

        border.BorderBrush = TryFindResource("Brush.Accent.Primary") as Brush ?? Brushes.DodgerBlue;
        border.Background = TryFindResource("Brush.Accent.Soft") as Brush ?? Brushes.LightSkyBlue;

        await Task.Delay(250);

        border.BorderBrush = originalBorderBrush;
        border.Background = originalBackground;
        if (textBlock != null)
        {
            textBlock.FontWeight = originalWeight;
            textBlock.Foreground = originalForeground;
        }
    }

    private async void CopyJudul_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is TextBlock tb && !string.IsNullOrWhiteSpace(tb.Text))
        {
            CopyToClipboard(tb.Text);
            ShowToast($"Judul disalin: \"{Truncate(tb.Text, 25)}\"");
            await FlashBoldEffectAsync(tb, TryFindResource("Brush.Accent.Primary") as Brush);
        }
    }

    private async void GenreTag_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border border && border.DataContext is string genre && !string.IsNullOrWhiteSpace(genre))
        {
            CopyToClipboard(genre);
            ShowToast($"Genre disalin: \"{genre}\"");
            await FlashGenreTagEffectAsync(border);
        }
    }

    private void CopyNotesContent_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        // Jangan timpa jika user mengklik sebuah hyperlink
        if (e.OriginalSource is Hyperlink || (e.OriginalSource is DependencyObject d && FindParent<Hyperlink>(d) != null))
        {
            return;
        }

        CopyCatatanCore();
    }

    private void CopyCatatan_Click(object sender, RoutedEventArgs e)
    {
        CopyCatatanCore();
    }

    private async void CopyCatatanCore()
    {
        var grid = ContentGrid;
        var item = grid?.DataContext as MediaItem;
        var text = item?.Catatan;
        if (!string.IsNullOrWhiteSpace(text))
        {
            CopyToClipboard(text);
            ShowToast("Catatan pribadi disalin!");
            if (NotesTextBlock != null)
            {
                await FlashBoldEffectAsync(NotesTextBlock, TryFindResource("Brush.Accent.Primary") as Brush);
            }
        }
    }

    private async void CopyTimestamp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is TextBlock tb && !string.IsNullOrWhiteSpace(tb.Text))
        {
            CopyToClipboard(tb.Text);
            ShowToast("Waktu disalin!");
            await FlashBoldEffectAsync(tb, TryFindResource("Brush.Accent.Primary") as Brush);
        }
    }

    private static string Truncate(string text, int maxLen)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxLen) return text;
        return text[..maxLen] + "...";
    }

    private static T? FindParent<T>(DependencyObject? child) where T : DependencyObject
    {
        while (child != null)
        {
            if (child is T parent) return parent;
            if (child is FrameworkContentElement fce)
                child = fce.Parent;
            else
                child = VisualTreeHelper.GetParent(child);
        }
        return null;
    }
}
