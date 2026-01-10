using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

// ReSharper disable once CheckNamespace
namespace AvailabilityCompass.WpfClient.Pages;

public partial class SearchView
{
    public SearchView()
    {
        InitializeComponent();
    }


    private void UrlIcon_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string url && !string.IsNullOrEmpty(url))
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }

    private void ResultCard_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is Border border && border.Tag is string url && !string.IsNullOrEmpty(url))
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
}