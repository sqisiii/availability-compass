using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MaterialDesignThemes.Wpf;

// ReSharper disable once CheckNamespace
namespace AvailabilityCompass.WpfClient.Pages;

public partial class ManageSettingsView : UserControl
{
    public ManageSettingsView()
    {
        InitializeComponent();
    }

    private void DateThemeSwitch_Click(object sender, RoutedEventArgs e)
    {
        PaletteHelper paletteHelper = new();
        var theme = paletteHelper.GetTheme();

        var isDarkTheme = ((ToggleButton)sender).IsChecked ?? false;

        theme.SetBaseTheme(isDarkTheme ? BaseTheme.Dark : BaseTheme.Light);
        paletteHelper.SetTheme(theme);
    }
}