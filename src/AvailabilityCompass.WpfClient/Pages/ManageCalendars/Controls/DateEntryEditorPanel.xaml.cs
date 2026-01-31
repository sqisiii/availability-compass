using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AvailabilityCompass.Core.Features.ManageCalendars;

namespace AvailabilityCompass.WpfClient.Pages.ManageCalendars.Controls;

// ReSharper disable once RedundantExtendsListEntry
public partial class DateEntryEditorPanel : UserControl
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(DateEntryEditorPanel),
            new PropertyMetadata("Edit Entry"));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(DateEntryEditorPanel),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty IsRecurringProperty =
        DependencyProperty.Register(nameof(IsRecurring), typeof(bool), typeof(DateEntryEditorPanel),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty FrequencyProperty =
        DependencyProperty.Register(nameof(Frequency), typeof(int?), typeof(DateEntryEditorPanel),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty RepetitionsProperty =
        DependencyProperty.Register(nameof(Repetitions), typeof(int?), typeof(DateEntryEditorPanel),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty FrequencyErrorProperty =
        DependencyProperty.Register(nameof(FrequencyError), typeof(string), typeof(DateEntryEditorPanel),
            new PropertyMetadata(null));

    public static readonly DependencyProperty RepetitionsErrorProperty =
        DependencyProperty.Register(nameof(RepetitionsError), typeof(string), typeof(DateEntryEditorPanel),
            new PropertyMetadata(null));

    public static readonly DependencyProperty DetectedSelectionsProperty =
        DependencyProperty.Register(nameof(DetectedSelections), typeof(IEnumerable<DetectedSelection>), typeof(DateEntryEditorPanel),
            new PropertyMetadata(null));

    public static readonly DependencyProperty IsEditModeProperty =
        DependencyProperty.Register(nameof(IsEditMode), typeof(bool), typeof(DateEntryEditorPanel),
            new PropertyMetadata(false));

    public static readonly DependencyProperty SaveCommandProperty =
        DependencyProperty.Register(nameof(SaveCommand), typeof(ICommand), typeof(DateEntryEditorPanel),
            new PropertyMetadata(null));

    public static readonly DependencyProperty CancelCommandProperty =
        DependencyProperty.Register(nameof(CancelCommand), typeof(ICommand), typeof(DateEntryEditorPanel),
            new PropertyMetadata(null));

    public static readonly DependencyProperty DeleteCommandProperty =
        DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(DateEntryEditorPanel),
            new PropertyMetadata(null));

    public DateEntryEditorPanel()
    {
        InitializeComponent();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public bool IsRecurring
    {
        get => (bool)GetValue(IsRecurringProperty);
        set => SetValue(IsRecurringProperty, value);
    }

    public int? Frequency
    {
        get => (int?)GetValue(FrequencyProperty);
        set => SetValue(FrequencyProperty, value);
    }

    public int? Repetitions
    {
        get => (int?)GetValue(RepetitionsProperty);
        set => SetValue(RepetitionsProperty, value);
    }

    public string? FrequencyError
    {
        get => (string?)GetValue(FrequencyErrorProperty);
        set => SetValue(FrequencyErrorProperty, value);
    }

    public string? RepetitionsError
    {
        get => (string?)GetValue(RepetitionsErrorProperty);
        set => SetValue(RepetitionsErrorProperty, value);
    }

    public IEnumerable<DetectedSelection>? DetectedSelections
    {
        get => (IEnumerable<DetectedSelection>?)GetValue(DetectedSelectionsProperty);
        set => SetValue(DetectedSelectionsProperty, value);
    }

    public bool IsEditMode
    {
        get => (bool)GetValue(IsEditModeProperty);
        set => SetValue(IsEditModeProperty, value);
    }

    public ICommand? SaveCommand
    {
        get => (ICommand?)GetValue(SaveCommandProperty);
        set => SetValue(SaveCommandProperty, value);
    }

    public ICommand? CancelCommand
    {
        get => (ICommand?)GetValue(CancelCommandProperty);
        set => SetValue(CancelCommandProperty, value);
    }

    public ICommand? DeleteCommand
    {
        get => (ICommand?)GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }
}