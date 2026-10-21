using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SIHIFA.ViewModels;

namespace SIHIFA.Views;

public partial class MediaFormDialog : UserControl
{
    // Regex validasi karakter numerik input: hanya izinkan digit 0-9 dan separator desimal . atau ,
    private static readonly Regex NumericRegex = new(@"^[0-9.,]+$");

    public MediaFormDialog()
    {
        InitializeComponent();
    }

    private void RatingTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        // Cegah input huruf, simbol berbahaya, dan upaya injection (', ;, --, dsb.)
        e.Handled = !NumericRegex.IsMatch(e.Text);
    }

    private void RatingTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
    {
        // Validasi buffer paste: jika mengandung teks non-numerik atau injection, batalkan paste
        if (e.DataObject.GetDataPresent(typeof(string)))
        {
            var text = (string)e.DataObject.GetData(typeof(string));
            if (string.IsNullOrWhiteSpace(text) || !NumericRegex.IsMatch(text))
            {
                e.CancelCommand();
            }
        }
        else
        {
            e.CancelCommand();
        }
    }

    private void RatingTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is MediaFormViewModel vm)
        {
            vm.FormatRatingText();
        }
    }
}
