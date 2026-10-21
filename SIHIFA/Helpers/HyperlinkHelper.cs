using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace SIHIFA.Helpers;

public static class HyperlinkHelper
{
    // Regex mendeteksi awalan http://, https://, www., serta domain umum (.com, .org, .net, .id, dll)
    private static readonly Regex UrlRegex = new(
        @"(https?:\/\/[^\s]+|www\.[^\s]+|[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?\.(?:com|org|net|edu|gov|id|co\.id|io|dev|app|ai|me|tv|cc|to|xyz|info|online|tech|site)(?:\/[^\s]*)?)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static readonly DependencyProperty FormattedTextProperty =
        DependencyProperty.RegisterAttached(
            "FormattedText",
            typeof(string),
            typeof(HyperlinkHelper),
            new PropertyMetadata(null, OnFormattedTextChanged));

    public static string? GetFormattedText(DependencyObject obj) =>
        (string?)obj.GetValue(FormattedTextProperty);

    public static void SetFormattedText(DependencyObject obj, string? value) =>
        obj.SetValue(FormattedTextProperty, value);

    private static void OnFormattedTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBlock textBlock) return;

        textBlock.Inlines.Clear();

        var text = e.NewValue as string;
        if (string.IsNullOrEmpty(text)) return;

        int lastIndex = 0;
        var matches = UrlRegex.Matches(text);

        foreach (Match match in matches)
        {
            // Tambahkan teks biasa sebelum link
            if (match.Index > lastIndex)
            {
                var normalText = text.Substring(lastIndex, match.Index - lastIndex);
                textBlock.Inlines.Add(new Run(normalText));
            }

            var rawUrl = match.Value;
            var trailingPunctuation = string.Empty;

            // Pisahkan tanda baca di akhir URL jika ada (seperti titik, koma, tutup kurung)
            while (rawUrl.Length > 0 && (rawUrl.EndsWith(".") || rawUrl.EndsWith(",") || rawUrl.EndsWith(";") || rawUrl.EndsWith(")") || rawUrl.EndsWith("]")))
            {
                trailingPunctuation = rawUrl[^1] + trailingPunctuation;
                rawUrl = rawUrl[..^1];
            }

            var targetUrl = rawUrl;
            if (!targetUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !targetUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                targetUrl = "https://" + targetUrl;
            }

            // Buat elemen Hyperlink interaktif
            var hyperlink = new Hyperlink(new Run(rawUrl))
            {
                ToolTip = $"Klik untuk membuka: {targetUrl}",
                Cursor = Cursors.Hand,
                Foreground = Application.Current?.TryFindResource("Brush.Accent.Primary") as Brush ?? Brushes.DodgerBlue,
                TextDecorations = TextDecorations.Underline
            };

            var finalTargetUrl = targetUrl;
            hyperlink.Click += (sender, args) =>
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = finalTargetUrl,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Tidak dapat membuka peramban web: {ex.Message}", "Kesalahan Tautan",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            };

            textBlock.Inlines.Add(hyperlink);

            if (!string.IsNullOrEmpty(trailingPunctuation))
            {
                textBlock.Inlines.Add(new Run(trailingPunctuation));
            }

            lastIndex = match.Index + match.Length;
        }

        // Tambahkan sisa teks setelah link terakhir
        if (lastIndex < text.Length)
        {
            var remainingText = text.Substring(lastIndex);
            textBlock.Inlines.Add(new Run(remainingText));
        }
    }
}
