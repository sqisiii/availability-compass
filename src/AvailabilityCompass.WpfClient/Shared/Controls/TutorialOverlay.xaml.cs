using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using AvailabilityCompass.Core.Features.Tutorial;
using AvailabilityCompass.WpfClient.Shared.Tutorial;

namespace AvailabilityCompass.WpfClient.Shared.Controls;

/// <summary>
/// Tutorial overlay control that displays tooltips and highlights target elements.
/// </summary>
// ReSharper disable once RedundantExtendsListEntry
public partial class TutorialOverlay : UserControl
{
    private TutorialTargetElement _currentPrimaryTarget;
    private TutorialTargetElement? _currentSecondaryTarget;
    private TutorialHighlightAdorner? _primaryAdorner;
    private TutorialHighlightAdorner? _secondaryAdorner;

    public TutorialOverlay()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        TutorialElementRegistry.ElementChanged += OnElementRegistryChanged;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        TutorialElementRegistry.ElementChanged -= OnElementRegistryChanged;
        RemoveAdorners();
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is TutorialViewModel oldVm)
        {
            oldVm.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (e.NewValue is not TutorialViewModel newVm)
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
        if (sender is not TutorialViewModel vm)
        {
            return;
        }

        switch (e.PropertyName)
        {
            case nameof(TutorialViewModel.IsVisible):
                if (vm.IsVisible)
                {
                    UpdateHighlights();
                    UpdateTooltipPosition();
                }
                else
                {
                    RemoveAdorners();
                }

                break;

            case nameof(TutorialViewModel.PrimaryTarget):
            case nameof(TutorialViewModel.SecondaryTarget):
                UpdateHighlights();
                UpdateTooltipPosition();
                break;

            case nameof(TutorialViewModel.ShowNextButton):
                UpdateActionHint();
                break;
        }
    }

    private void OnElementRegistryChanged(object? sender, TutorialTargetElement targetType)
    {
        // If the changed element is one we're tracking, update highlights
        if (targetType == _currentPrimaryTarget || targetType == _currentSecondaryTarget)
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
        if (DataContext is not TutorialViewModel vm || !vm.IsVisible)
        {
            RemoveAdorners();
            Backdrop.SetClickableTargets(null, null);
            return;
        }

        _currentPrimaryTarget = vm.PrimaryTarget;
        _currentSecondaryTarget = vm.SecondaryTarget;

        // Get the target elements
        var primaryElement = TutorialElementRegistry.GetElement(vm.PrimaryTarget);
        var secondaryElement = TutorialElementRegistry.GetElement(vm.SecondaryTarget ?? TutorialTargetElement.None);

        // Update the backdrop's clickable targets for selective hit-testing
        Backdrop.SetClickableTargets(primaryElement, secondaryElement);

        // Update primary adorner
        UpdateAdorner(ref _primaryAdorner, vm.PrimaryTarget);

        // Update secondary adorner
        UpdateAdorner(ref _secondaryAdorner, vm.SecondaryTarget ?? TutorialTargetElement.None);
    }

    private static void UpdateAdorner(ref TutorialHighlightAdorner? adorner, TutorialTargetElement targetType)
    {
        // Remove existing adorner
        if (adorner != null)
        {
            if (adorner.AdornedElement is FrameworkElement oldElement)
            {
                var oldLayer = AdornerLayer.GetAdornerLayer(oldElement);
                oldLayer?.Remove(adorner);
            }

            adorner.StopAnimation();
            adorner = null;
        }

        // Add new adorner if the target is valid
        if (targetType == TutorialTargetElement.None)
        {
            return;
        }

        var element = TutorialElementRegistry.GetElement(targetType);
        if (element == null)
        {
            return;
        }

        var adornerLayer = AdornerLayer.GetAdornerLayer(element);
        if (adornerLayer == null)
        {
            return;
        }

        adorner = new TutorialHighlightAdorner(element);
        adornerLayer.Add(adorner);
    }

    private void RemoveAdorners()
    {
        if (_primaryAdorner != null)
        {
            if (_primaryAdorner.AdornedElement is FrameworkElement primaryElement)
            {
                var layer = AdornerLayer.GetAdornerLayer(primaryElement);
                layer?.Remove(_primaryAdorner);
            }

            _primaryAdorner.StopAnimation();
            _primaryAdorner = null;
        }

        if (_secondaryAdorner == null)
        {
            return;
        }

        if (_secondaryAdorner.AdornedElement is FrameworkElement secondaryElement)
        {
            var layer = AdornerLayer.GetAdornerLayer(secondaryElement);
            layer?.Remove(_secondaryAdorner);
        }

        _secondaryAdorner.StopAnimation();
        _secondaryAdorner = null;
    }

    private void UpdateTooltipPosition()
    {
        if (DataContext is not TutorialViewModel vm || !vm.IsVisible)
        {
            return;
        }

        var targetElement = TutorialElementRegistry.GetElement(vm.PrimaryTarget);

        if (targetElement == null || vm.TooltipPosition == TooltipPosition.Center)
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

            // Calculate tooltip position based on TooltipPosition
            double left = 0, top = 0;
            const double margin = 16;
            const double tooltipWidth = 380; // Approximate max width
            const double tooltipHeight = 250; // Approximate max height

            switch (vm.TooltipPosition)
            {
                case TooltipPosition.Top:
                    left = targetPosition.X + targetSize.Width / 2 - tooltipWidth / 2;
                    top = targetPosition.Y - tooltipHeight - margin;
                    break;

                case TooltipPosition.Bottom:
                    left = targetPosition.X + targetSize.Width / 2 - tooltipWidth / 2;
                    top = targetPosition.Y + targetSize.Height + margin;
                    break;

                case TooltipPosition.Left:
                    left = targetPosition.X - tooltipWidth - margin;
                    top = targetPosition.Y + targetSize.Height / 2 - tooltipHeight / 2;
                    break;

                case TooltipPosition.Right:
                    left = targetPosition.X + targetSize.Width + margin;
                    top = targetPosition.Y + targetSize.Height / 2 - tooltipHeight / 2;
                    break;

                case TooltipPosition.TopLeft:
                    left = targetPosition.X - tooltipWidth - margin;
                    top = targetPosition.Y - tooltipHeight - margin;
                    break;

                case TooltipPosition.TopRight:
                    left = targetPosition.X + targetSize.Width + margin;
                    top = targetPosition.Y - tooltipHeight - margin;
                    break;

                case TooltipPosition.BottomLeft:
                    left = targetPosition.X - tooltipWidth - margin;
                    top = targetPosition.Y + targetSize.Height + margin;
                    break;

                case TooltipPosition.BottomRight:
                    left = targetPosition.X + targetSize.Width + margin;
                    top = targetPosition.Y + targetSize.Height + margin;
                    break;
                case TooltipPosition.Center:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Clamp to visible area
            left = Math.Max(margin, Math.Min(left, overlaySize.Width - tooltipWidth - margin));
            top = Math.Max(margin, Math.Min(top, overlaySize.Height - tooltipHeight - margin));

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
        if (DataContext is TutorialViewModel vm)
        {
            ActionHint.Visibility = vm.ShowNextButton ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}