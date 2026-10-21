using System.Windows;
using SIHIFA.ViewModels;

namespace SIHIFA;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += async (s, e) =>
        {
            await _viewModel.InitializeAsync();
        };
    }
}