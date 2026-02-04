using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Guidely.Core.Abstractions;

// ReSharper disable MemberCanBePrivate.Global

namespace Guidely.WPF.Controls;

/// <summary>
/// Tutorial overlay control that displays tooltips and highlights target elements.
/// </summary>
// ReSharper disable once RedundantExtendsListEntry
public partial class TutorialOverlay : UserControl
{
    private readonly List<(string Name, TutorialHighlightAdorner Adorner)> _adorners = [];

    public TutorialOverlay()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
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
        RemoveAllAdorners();
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
                    RemoveAllAdorners();
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
            RemoveAllAdorners();
            Backdrop.SetClickableTargets();
            return;
        }

        var targets = GetTargets();
        RemoveAllAdorners();

        var clickableTargets = new List<FrameworkElement>();

        foreach (var targetName in targets)
        {
            var element = TutorialElementRegistry.GetElement(targetName);
            if (element == null)
                continue;

            clickableTargets.Add(element);

            var adornerLayer = AdornerLayer.GetAdornerLayer(element);
            if (adornerLayer == null)
                continue;

            var adorner = new TutorialHighlightAdorner(element);
            adornerLayer.Add(adorner);
            _adorners.Add((targetName, adorner));
        }

        Backdrop.SetClickableTargets(clickableTargets.ToArray());
    }

    private void RemoveAllAdorners()
    {
        foreach (var (_, adorner) in _adorners)
        {
            if (adorner.AdornedElement is FrameworkElement element)
            {
                var layer = AdornerLayer.GetAdornerLayer(element);
                layer?.Remove(adorner);
            }

            adorner.StopAnimation();
        }

        _adorners.Clear();
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
            // Center the tooltip in the overlay
            TooltipBorder.HorizontalAlignment = HorizontalAlignment.Center;
            TooltipBorder.VerticalAlignment = VerticalAlignment.Center;
            TooltipBorder.Margin = new Thickness(0);
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
            // If transform fails, center the tooltip
            TooltipBorder.HorizontalAlignment = HorizontalAlignment.Center;
            TooltipBorder.VerticalAlignment = VerticalAlignment.Center;
            TooltipBorder.Margin = new Thickness(0);
        }
    }

    private void UpdateActionHint()
    {
        var showNextButton = GetShowNextButton();
        ActionHint.Visibility = showNextButton ? Visibility.Collapsed : Visibility.Visible;
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