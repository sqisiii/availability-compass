using System.Windows.Media;

namespace Guidely.WPF.Controls;

/// <summary>
/// Configuration for tutorial highlight rectangle appearance and animation.
/// </summary>
public record HighlightStyle
{
    public static double Padding => 4;
    public static double BorderThickness => 3;
    public static double CornerRadius => 8;
    public Color Color { get; } = Color.FromRgb(59, 130, 246);
    public TimeSpan AnimationDuration { get; } = TimeSpan.FromMilliseconds(800);
    public static double OpacityFrom => 1.0;
    public static double OpacityTo => 0.4;

    public static HighlightStyle Default { get; } = new();
}