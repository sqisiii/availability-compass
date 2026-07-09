using System.Collections.Specialized;
using AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;
using AvailabilityCompass.MauiClient.Messages;
using AvailabilityCompass.MauiClient.Shared.Theme;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls.Shapes;

namespace AvailabilityCompass.MauiClient.Controls;

public class MultiSelectDropdown : ContentView, IRecipient<CloseDropdownsMessage>
{
    private readonly Label _chevronLabel;
    private readonly Label _clearButton;
    private readonly Border _optionsContainer;
    private readonly Label _summaryLabel;
    private FormElement? _boundElement;

    public MultiSelectDropdown()
    {
        WeakReferenceMessenger.Default.Register(this);

        var layout = new VerticalStackLayout { Spacing = 4 };

        layout.Children.Add(
            SearchTheme.MutedLabel(new Label { FontSize = 12, Opacity = 0.7 })
                .Bind(Label.TextProperty, nameof(FormElement.Label)));

        var optionsList = new ScrollView
        {
            MaximumHeightRequest = 200
        };

        _summaryLabel = new Label
        {
            FontSize = 13,
            LineBreakMode = LineBreakMode.TailTruncation,
            MaxLines = 1,
            Text = "..."
        };
        SearchTheme.MutedLabel(_summaryLabel);

        _chevronLabel = new Label
        {
            Text = "▼",
            FontSize = 12,
            VerticalTextAlignment = TextAlignment.Center
        };

        SearchTheme.PrimaryLabel(_chevronLabel);

        _clearButton = new Label
        {
            Text = "✕",
            FontSize = 14,
            VerticalTextAlignment = TextAlignment.Center,
            IsVisible = false
        };
        SearchTheme.MutedLabel(_clearButton);

        _optionsContainer = SearchTheme.OptionsPanel(new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = 8 },
            Padding = new Thickness(4),
            IsVisible = false,
            Content = optionsList
        });

        var header = SearchTheme.InputBorder(new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = 8 },
            Padding = new Thickness(12, 8),
            Content = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Auto)
                },
                ColumnSpacing = 8,
                Children =
                {
                    _summaryLabel.Column(0),
                    _clearButton.Column(1),
                    _chevronLabel.Column(2)
                }
            }
        });

        var headerTap = new TapGestureRecognizer();
        headerTap.Tapped += (_, _) => ToggleDropdown();
        header.GestureRecognizers.Add(headerTap);

        var clearTap = new TapGestureRecognizer();
        clearTap.Tapped += (_, _) =>
        {
            if (BindingContext is not FormElement fe) return;
            foreach (var option in fe.Options)
                option.IsSelected = false;
        };
        _clearButton.GestureRecognizers.Add(clearTap);

        var itemsLayout = new VerticalStackLayout { Spacing = 0 };
        itemsLayout.SetBinding(BindableLayout.ItemsSourceProperty, nameof(FormElement.Options));
        BindableLayout.SetItemTemplate(itemsLayout, new DataTemplate(() =>
        {
            var row = new HorizontalStackLayout
            {
                Spacing = 4,
                Padding = new Thickness(8, 4),
                Children =
                {
                    new CheckBox { VerticalOptions = LayoutOptions.Center }
                        .Bind(CheckBox.IsCheckedProperty, nameof(FormElementSelectOption.IsSelected),
                            mode: BindingMode.TwoWay),
                    SearchTheme.PrimaryLabel(new Label { FontSize = 13, VerticalTextAlignment = TextAlignment.Center })
                        .Bind(Label.TextProperty, nameof(FormElementSelectOption.Name))
                }
            };
            return row;
        }));

        optionsList.Content = itemsLayout;

        BindingContextChanged += (_, _) =>
        {
            // Unhook the previous element first — the subscription is a strong reference
            // that otherwise keeps every discarded dropdown instance alive.
            UnhookOptions();
            _boundElement = BindingContext as FormElement;
            if (_boundElement is null) return;
            UpdateSummary(_boundElement);
            _boundElement.Options.CollectionChanged += OnOptionsCollectionChanged;
        };

        layout.Children.Add(header);
        layout.Children.Add(_optionsContainer);
        Content = layout;
    }

    private bool IsOpen => _optionsContainer.IsVisible;

    public void Receive(CloseDropdownsMessage message)
    {
        Close();
    }

    private void Close()
    {
        if (!IsOpen) return;
        _optionsContainer.IsVisible = false;
        _chevronLabel.Text = "▼";
    }

    protected override void OnHandlerChanging(HandlerChangingEventArgs args)
    {
        base.OnHandlerChanging(args);
        if (args.NewHandler is null)
        {
            WeakReferenceMessenger.Default.Unregister<CloseDropdownsMessage>(this);
            UnhookOptions();
        }
    }

    private void UnhookOptions()
    {
        if (_boundElement is null)
        {
            return;
        }

        _boundElement.Options.CollectionChanged -= OnOptionsCollectionChanged;
        _boundElement = null;
    }

    private void OnOptionsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_boundElement is not null)
        {
            UpdateSummary(_boundElement);
        }
    }

    private void ToggleDropdown()
    {
        if (IsOpen)
        {
            Close();
        }
        else
        {
            WeakReferenceMessenger.Default.Send(new CloseDropdownsMessage());
            _optionsContainer.IsVisible = true;
            _chevronLabel.Text = "▲";
        }
    }

    private void UpdateSummary(FormElement fe)
    {
        var selected = fe.Options.Where(o => o.IsSelected).Select(o => o.Name).ToList();
        if (selected.Count == 0)
        {
            _summaryLabel.Text = "...";
            _clearButton.IsVisible = false;
            return;
        }

        _clearButton.IsVisible = true;
        var text = string.Empty;
        foreach (var name in selected)
        {
            var next = text.Length == 0 ? name : text + ", " + name;
            if (next.Length > 50)
            {
                text += "...";
                break;
            }

            text = next;
        }

        _summaryLabel.Text = text;
    }
}
