using System.Windows.Controls;
using System.Windows.Input;

// ReSharper disable once CheckNamespace
namespace AvailabilityCompass.WpfClient.Pages;

public partial class ManageCalendarsView : UserControl
{
    public ManageCalendarsView()
    {
        InitializeComponent();
    }

    private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !int.TryParse(e.Text, out _);
    }
}