using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Guidely.Core.Abstractions;

// ReSharper disable MemberCanBePrivate.Global

namespace Guidely.WPF.Controls;

/// <summary>
/// Tutorial overlay control that displays tooltips and highlights target elements.
/// </summary>
// ReSharper disable once RedundantExtendsListEntry
public partial class TutorialOverlay : UserControl
{
    private readonly List<(string Name, Rectangle Highlight, Storyboard Animation)> _highlights = [];
    private Point _dragStartMousePosition;

    // Drag state tracking
    private bool _isDragging;
    private Point _tooltipStartPosition;

    public TutorialOverlay()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        // Wire up drag handlers
        TooltipBorder.MouseLeftButtonDown += OnTooltipMouseDown;
        TooltipBorder.MouseLeftButtonUp += OnTooltipMouseUp;
        TooltipBorder.MouseMove += OnTooltipMouseMove;
    }

    /// <summary>
    /// The approximate width of the tooltip for positioning calculations.
    /// </summary>
    public double TooltipWidth { get; set; } = 380;

    /// <summary>
    /// The approximate height of the tooltip for positioning calculations.
    /// </summary>
    public double TooltipHeight { get; set; } = 250;

    /// <summary>
    /// The margin between tooltip and target element.
    /// </summary>
    public double TooltipMargin { get; set; } = 16;

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        TutorialElementRegistry.ElementChanged += OnElementRegistryChanged;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        TutorialElementRegistry.ElementChanged -= OnElementRegistryChanged;

        if (DataContext is INotifyPropertyChanged vm)
        {
            vm.PropertyChanged -= OnViewModelPropertyChanged;
        }

        TooltipBorder.MouseLeftButtonDown -= OnTooltipMouseDown;
        TooltipBorder.MouseLeftButtonUp -= OnTooltipMouseUp;
        TooltipBorder.MouseMove -= OnTooltipMouseMove;

        DataContextChanged -= OnDataContextChanged;

        ClearHighlights();
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is INotifyPropertyChanged oldVm)
        {
            oldVm.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (e.NewValue is not INotifyPropertyChanged newVm)
        {
            return;
        }

        newVm.PropertyChanged += OnViewModelPropertyChanged;
        UpdateHighlights();
        UpdateTooltipPosition();
        UpdateActionHint();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "IsVisible":
                var isVisible = GetIsVisible();
                if (isVisible)
                {
                    UpdateHighlights();
                    UpdateTooltipPosition();
                }
                else
                {
                    ClearHighlights();
                }

                break;

            case "Targets":
            case "CurrentStepId":
                UpdateHighlights();
                UpdateTooltipPosition();
                break;

            case "ShowNextButton":
                UpdateActionHint();
                break;
        }
    }

    private void OnElementRegistryChanged(object? sender, string targetName)
    {
        // If the changed element is one we're tracking, update highlights
        var currentTargets = GetTargets();
        if (currentTargets.Contains(targetName))
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateHighlights();
                UpdateTooltipPosition();
            }));
        }
    }

    private void UpdateHighlights()
    {
        var isVisible = GetIsVisible();
        if (!isVisible)
        {
            ClearHighlights();
            Backdrop.SetClickableTargets();
            return;
        }

        var targets = GetTargets();
        ClearHighlights();

        var window = Window.GetWindow(this);
        if (window == null)
        {
            Backdrop.SetClickableTargets();
            return;
        }

        var clickableTargets = new List<FrameworkElement>();

        foreach (var targetName in targets)
        {
            var element = TutorialElementRegistry.GetElement(targetName);
            if (element == null)
                continue;

            clickableTargets.Add(element);

            try
            {
                // Get element position relative to overlay (not window)
                var position = element.TransformToVisual(this).Transform(new Point(0, 0));
                var size = element.RenderSize;

                // Create a highlight rectangle
                var (highlight, animation) = CreateHighlightRectangle(position, size);
                HighlightCanvas.Children.Add(highlight);
                _highlights.Add((targetName, highlight, animation));
            }
            catch
            {
                // Transform failed, skip this element
            }
        }

        Backdrop.SetClickableTargets(clickableTargets.ToArray());
        Backdrop.InvalidateCutouts();
    }

    private (Rectangle Highlight, Storyboard Animation) CreateHighlightRectangle(Point position, Size size)
    {
        const double padding = 4;
        const double borderThickness = 3;
        const double cornerRadius = 8;
        var highlightColor = Color.FromRgb(59, 130, 246); // Blue

        var highlight = new Rectangle
        {
            Width = size.Width + padding * 2,
            Height = size.Height + padding * 2,
            Stroke = new SolidColorBrush(highlightColor),
            StrokeThickness = borderThickness,
            RadiusX = cornerRadius,
            RadiusY = cornerRadius,
            Fill = Brushes.Transparent,
            IsHitTestVisible = false
        };

        // Position on canvas
        Canvas.SetLeft(highlight, position.X - padding);
        Canvas.SetTop(highlight, position.Y - padding);

        // Create pulse animation
        var animation = new DoubleAnimation
        {
            From = 1.0,
            To = 0.4,
            Duration = TimeSpan.FromMilliseconds(800),
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever,
            EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
        };

        var storyboard = new Storyboard();
        storyboard.Children.Add(animation);
        Storyboard.SetTarget(animation, highlight);
        Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));
        storyboard.Begin();

        return (highlight, storyboard);
    }

    private void ClearHighlights()
    {
        foreach (var (_, _, animation) in _highlights)
        {
            animation.Stop();
        }

        _highlights.Clear();
        HighlightCanvas.Children.Clear();
    }

    private void UpdateTooltipPosition()
    {
        var isVisible = GetIsVisible();
        if (!isVisible)
        {
            return;
        }

        var targets = GetTargets();
        var position = GetTooltipPosition();

        var targetElement = targets.Count > 0 ? TutorialElementRegistry.GetElement(targets[0]) : null;

        if (targetElement == null || position == TooltipPosition.Center)
        {
            // Calculate center position explicitly (don't use Center alignment to keep a consistent coordinate system)
            var overlaySize = new Size(ActualWidth, ActualHeight);
            var left = (overlaySize.Width - TooltipWidth) / 2;
            var top = (overlaySize.Height - TooltipHeight) / 2;

            // Clamp to visible area
            left = Math.Max(TooltipMargin, Math.Min(left, overlaySize.Width - TooltipWidth - TooltipMargin));
            top = Math.Max(TooltipMargin, Math.Min(top, overlaySize.Height - TooltipHeight - TooltipMargin));

            TooltipBorder.HorizontalAlignment = HorizontalAlignment.Left;
            TooltipBorder.VerticalAlignment = VerticalAlignment.Top;
            TooltipBorder.Margin = new Thickness(left, top, 0, 0);
            return;
        }

        // Get the position of the target element relative to the overlay
        var window = Window.GetWindow(this);
        if (window == null)
        {
            return;
        }

        try
        {
            var targetPosition = targetElement.TransformToAncestor(window).Transform(new Point(0, 0));
            var targetSize = targetElement.RenderSize;
            var overlaySize = new Size(ActualWidth, ActualHeight);

            // Calculate tooltip position
            double left = 0, top = 0;

            switch (position)
            {
                case TooltipPosition.Top:
                    left = targetPosition.X + targetSize.Width / 2 - TooltipWidth / 2;
                    top = targetPosition.Y - TooltipHeight - TooltipMargin;
                    break;

                case TooltipPosition.Bottom:
                    left = targetPosition.X + targetSize.Width / 2 - TooltipWidth / 2;
                    top = targetPosition.Y + targetSize.Height + TooltipMargin;
                    break;

                case TooltipPosition.Left:
                    left = targetPosition.X - TooltipWidth - TooltipMargin;
                    top = targetPosition.Y + targetSize.Height / 2 - TooltipHeight / 2;
                    break;

                case TooltipPosition.Right:
                    left = targetPosition.X + targetSize.Width + TooltipMargin;
                    top = targetPosition.Y + targetSize.Height / 2 - TooltipHeight / 2;
                    break;
            }

            // Clamp to visible area
            left = Math.Max(TooltipMargin, Math.Min(left, overlaySize.Width - TooltipWidth - TooltipMargin));
            top = Math.Max(TooltipMargin, Math.Min(top, overlaySize.Height - TooltipHeight - TooltipMargin));

            // Apply position using margin
            TooltipBorder.HorizontalAlignment = HorizontalAlignment.Left;
            TooltipBorder.VerticalAlignment = VerticalAlignment.Top;
            TooltipBorder.Margin = new Thickness(left, top, 0, 0);
        }
        catch
        {
            // If transform fails, center the tooltip using explicit coordinates
            var overlaySize = new Size(ActualWidth, ActualHeight);
            var left = (overlaySize.Width - TooltipWidth) / 2;
            var top = (overlaySize.Height - TooltipHeight) / 2;

            left = Math.Max(TooltipMargin, Math.Min(left, overlaySize.Width - TooltipWidth - TooltipMargin));
            top = Math.Max(TooltipMargin, Math.Min(top, overlaySize.Height - TooltipHeight - TooltipMargin));

            TooltipBorder.HorizontalAlignment = HorizontalAlignment.Left;
            TooltipBorder.VerticalAlignment = VerticalAlignment.Top;
            TooltipBorder.Margin = new Thickness(left, top, 0, 0);
        }
    }

    private void UpdateActionHint()
    {
        var showNextButton = GetShowNextButton();
        ActionHint.Visibility = showNextButton ? Visibility.Collapsed : Visibility.Visible;
    }

    private void OnTooltipMouseDown(object sender, MouseButtonEventArgs e)
    {
        _isDragging = true;
        _dragStartMousePosition = e.GetPosition(this);
        // Capture the actual tooltip position from its Margin
        _tooltipStartPosition = new Point(TooltipBorder.Margin.Left, TooltipBorder.Margin.Top);
        TooltipBorder.CaptureMouse();
        e.Handled = true;
    }

    private void OnTooltipMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (!_isDragging)
        {
            return;
        }

        _isDragging = false;
        TooltipBorder.ReleaseMouseCapture();
        e.Handled = true;
    }

    private void OnTooltipMouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDragging)
            return;

        var currentPosition = e.GetPosition(this);
        var delta = currentPosition - _dragStartMousePosition;

        // Calculate new position directly from captured start position + delta
        var newLeft = _tooltipStartPosition.X + delta.X;
        var newTop = _tooltipStartPosition.Y + delta.Y;

        // Clamp to visible area
        var overlaySize = new Size(ActualWidth, ActualHeight);
        newLeft = Math.Max(TooltipMargin, Math.Min(newLeft, overlaySize.Width - TooltipWidth - TooltipMargin));
        newTop = Math.Max(TooltipMargin, Math.Min(newTop, overlaySize.Height - TooltipHeight - TooltipMargin));

        // Apply position directly
        TooltipBorder.Margin = new Thickness(newLeft, newTop, 0, 0);
    }

    // Helper methods to get properties from the untyped DataContext
    private bool GetIsVisible()
    {
        var prop = DataContext?.GetType().GetProperty("IsVisible");
        return prop?.GetValue(DataContext) is true;
    }

    private IReadOnlyList<string> GetTargets()
    {
        var prop = DataContext?.GetType().GetProperty("Targets");
        return prop?.GetValue(DataContext) as IReadOnlyList<string> ?? Array.Empty<string>();
    }

    private TooltipPosition GetTooltipPosition()
    {
        var prop = DataContext?.GetType().GetProperty("TooltipPosition");
        return prop?.GetValue(DataContext) is TooltipPosition pos ? pos : TooltipPosition.Bottom;
    }

    private bool GetShowNextButton()
    {
        var prop = DataContext?.GetType().GetProperty("ShowNextButton");
        return prop?.GetValue(DataContext) is true;
    }
}