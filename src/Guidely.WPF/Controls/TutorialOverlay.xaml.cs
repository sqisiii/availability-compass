using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Guidely.Core.Abstractions;

// ReSharper disable MemberCanBePrivate.Global

namespace Guidely.WPF.Controls;

/// <summary>
/// Tutorial overlay control that displays tooltips and highlights target elements.
/// </summary>
// ReSharper disable once RedundantExtendsListEntry
public partial class TutorialOverlay : UserControl
{
    private readonly TooltipPositioner _positioner = new();
    private TooltipDragHandler? _dragHandler;
    private HighlightManager? _highlightManager;

    public TutorialOverlay()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Fallback width for tooltip positioning before layout is complete.
    /// </summary>
    public double TooltipWidth { get; set; } = 380;

    /// <summary>
    /// Fallback height for tooltip positioning before layout is complete.
    /// </summary>
    public double TooltipHeight { get; set; } = 250;

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _highlightManager = new HighlightManager(HighlightCanvas);
        _dragHandler = new TooltipDragHandler(
            TooltipBorder,
            () => new Size(ActualWidth, ActualHeight),
            _positioner);
        _dragHandler.Attach();

        TutorialElementRegistry.ElementChanged += OnElementRegistryChanged;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        TutorialElementRegistry.ElementChanged -= OnElementRegistryChanged;

        if (DataContext is INotifyPropertyChanged vm)
        {
            vm.PropertyChanged -= OnViewModelPropertyChanged;
        }

        _dragHandler?.Detach();
        _highlightManager?.Clear();

        DataContextChanged -= OnDataContextChanged;
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
        RefreshOverlay();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "IsVisible":
                if (GetProperty<bool>("IsVisible"))
                {
                    RefreshOverlay();
                }
                else
                {
                    _highlightManager?.Clear();
                    Backdrop.SetClickableTargets();
                }

                break;

            case "Targets":
            case "CurrentStepId":
                Dispatcher.BeginInvoke(RefreshOverlay, DispatcherPriority.Loaded);
                break;

            case "ShowNextButton":
                UpdateActionHint();
                break;
        }
    }

    private void OnElementRegistryChanged(object? sender, string targetName)
    {
        var targets = GetProperty<IReadOnlyList<string>>("Targets") ?? [];
        if (targets.Contains(targetName))
        {
            Dispatcher.BeginInvoke(RefreshOverlay);
        }
    }

    private void RefreshOverlay()
    {
        UpdateHighlights();
        UpdateTooltipPosition();
        UpdateActionHint();
    }

    private void UpdateHighlights()
    {
        if (!GetProperty<bool>("IsVisible"))
        {
            _highlightManager?.Clear();
            Backdrop.SetClickableTargets();
            return;
        }

        var targets = GetProperty<IReadOnlyList<string>>("Targets") ?? [];
        var window = Window.GetWindow(this);

        if (_highlightManager == null || window == null)
        {
            Backdrop.SetClickableTargets();
            return;
        }

        var clickableTargets = _highlightManager.UpdateHighlights(targets, window, this, _positioner);
        Backdrop.SetClickableTargets(clickableTargets.ToArray());
        Backdrop.InvalidateCutouts();
    }

    private void UpdateTooltipPosition()
    {
        if (!GetProperty<bool>("IsVisible"))
        {
            return;
        }

        TooltipBorder.UpdateLayout();

        var tooltipSize = new Size(
            TooltipBorder.ActualWidth > 0 ? TooltipBorder.ActualWidth : TooltipWidth,
            TooltipBorder.ActualHeight > 0 ? TooltipBorder.ActualHeight : TooltipHeight);
        var overlaySize = new Size(ActualWidth, ActualHeight);

        var targets = GetProperty<IReadOnlyList<string>>("Targets") ?? [];
        var position = GetProperty("TooltipPosition", TooltipPosition.Bottom);
        var targetElement = targets.Count > 0 ? TutorialElementRegistry.GetElement(targets[0]) : null;

        var window = Window.GetWindow(this);
        var calculatedPosition = _positioner.CalculatePosition(
            targetElement,
            tooltipSize,
            overlaySize,
            position,
            window,
            this);

        TooltipBorder.HorizontalAlignment = HorizontalAlignment.Left;
        TooltipBorder.VerticalAlignment = VerticalAlignment.Top;
        TooltipBorder.Margin = new Thickness(calculatedPosition.X, calculatedPosition.Y, 0, 0);
    }

    private void UpdateActionHint()
    {
        var showNextButton = GetProperty<bool>("ShowNextButton");
        ActionHint.Visibility = showNextButton ? Visibility.Collapsed : Visibility.Visible;
    }

    private T? GetProperty<T>(string name, T? defaultValue = default)
    {
        var prop = DataContext?.GetType().GetProperty(name);
        return prop?.GetValue(DataContext) is T value ? value : defaultValue;
    }
}