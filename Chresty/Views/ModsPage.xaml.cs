using Chresty.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Chresty.Views;

public sealed partial class ModsPage : Page
{
    public ModsViewModel ViewModel
    {
        get;
    }

    private void OnElementClicked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
    }

    public ModsPage()
    {
        ViewModel = App.GetService<ModsViewModel>();
        InitializeComponent();
    }
}
