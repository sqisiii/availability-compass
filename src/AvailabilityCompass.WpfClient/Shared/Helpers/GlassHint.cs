using System.Windows;

namespace AvailabilityCompass.WpfClient.Shared.Helpers;

/// <summary>
/// Attached properties for floating hint labels on input controls.
/// Replaces MaterialDesign's HintAssist.
/// </summary>
public static class GlassHint
{
    #region Hint Property

    /// <summary>
    /// Gets the hint text displayed as a floating label.
    /// </summary>
    public static readonly DependencyProperty HintProperty =
        DependencyProperty.RegisterAttached(
            "Hint",
            typeof(string),
            typeof(GlassHint),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.Inherits));

    public static string GetHint(DependencyObject obj) =>
        (string)obj.GetValue(HintProperty);

    public static void SetHint(DependencyObject obj, string value) =>
        obj.SetValue(HintProperty, value);

    #endregion

    #region HelperText Property

    /// <summary>
    /// Gets the helper text displayed below the input.
    /// </summary>
    public static readonly DependencyProperty HelperTextProperty =
        DependencyProperty.RegisterAttached(
            "HelperText",
            typeof(string),
            typeof(GlassHint),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.Inherits));

    public static string GetHelperText(DependencyObject obj) =>
        (string)obj.GetValue(HelperTextProperty);

    public static void SetHelperText(DependencyObject obj, string value) =>
        obj.SetValue(HelperTextProperty, value);

    #endregion

    #region IsFloating Property

    /// <summary>
    /// Gets whether the hint should float above the input when focused or has content.
    /// </summary>
    public static readonly DependencyProperty IsFloatingProperty =
        DependencyProperty.RegisterAttached(
            "IsFloating",
            typeof(bool),
            typeof(GlassHint),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

    public static bool GetIsFloating(DependencyObject obj) =>
        (bool)obj.GetValue(IsFloatingProperty);

    public static void SetIsFloating(DependencyObject obj, bool value) =>
        obj.SetValue(IsFloatingProperty, value);

    #endregion

    #region ShowSelectedItem Property (for ComboBox)

    /// <summary>
    /// Gets whether to show the selected item in the ComboBox display.
    /// </summary>
    public static readonly DependencyProperty ShowSelectedItemProperty =
        DependencyProperty.RegisterAttached(
            "ShowSelectedItem",
            typeof(bool),
            typeof(GlassHint),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

    public static bool GetShowSelectedItem(DependencyObject obj) =>
        (bool)obj.GetValue(ShowSelectedItemProperty);

    public static void SetShowSelectedItem(DependencyObject obj, bool value) =>
        obj.SetValue(ShowSelectedItemProperty, value);

    #endregion
}
