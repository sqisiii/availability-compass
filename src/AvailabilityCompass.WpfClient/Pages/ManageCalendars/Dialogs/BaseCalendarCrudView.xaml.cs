using System.Windows;
using System.Windows.Controls;

namespace AvailabilityCompass.WpfClient.Pages.ManageCalendars.Dialogs;

public partial class BaseCalendarCrudView : UserControl
{
    public static readonly DependencyProperty ViewHeaderTextProperty = DependencyProperty.Register(
        nameof(ViewHeaderText), typeof(string), typeof(BaseCalendarCrudView), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty IsViewEnabledProperty = DependencyProperty.Register(
        nameof(IsViewEnabled), typeof(bool), typeof(BaseCalendarCrudView), new PropertyMetadata(true));

    public static readonly DependencyProperty SaveCommandPromptProperty = DependencyProperty.Register(
        nameof(SaveCommandPrompt), typeof(string), typeof(BaseCalendarCrudView), new PropertyMetadata(default(string)));

    public BaseCalendarCrudView()
    {
        InitializeComponent();
    }

    public string SaveCommandPrompt
    {
        get => (string)GetValue(SaveCommandPromptProperty);
        set => SetValue(SaveCommandPromptProperty, value);
    }

    public string ViewHeaderText
    {
        get => (string)GetValue(ViewHeaderTextProperty);
        set => SetValue(ViewHeaderTextProperty, value);
    }

    public bool IsViewEnabled
    {
        get => (bool)GetValue(IsViewEnabledProperty);
        set => SetValue(IsViewEnabledProperty, value);
    }
}